using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class pPlayerController : MonoBehaviour
{
    // =========================
    // Movement
    // =========================
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSmoothTime = 0.1f;
    public float extraGravity = 9.81f;
    [Range(0f, 1f)] public float inputDeadZone = 0.2f;

    [Header("Movement References")]
    public Transform cameraTransform;
    public Joystick joystick;
    public ParticleSystem runDust;
    public Button runButton;

    // =========================
    // Fishing - Core Settings
    // =========================
    [Header("Fishing Settings")]
    public FishingMinigameUI fishingUI;
    public FishDatabase fishDatabase;
    public FishJournalUI fishJournal;
    [SerializeField] private InventoryManager inventoryManager;
    public GameObject rodPrefab;
    public Transform rodSocket;
    public Vector3 rodOffsetPosition;
    public Vector3 rodOffsetRotation;
    public bool requireFishingZone = true;

    [Header("Fishing Cast/Bobber/Line")]
    public Transform bobberSpawnPoint;
    public GameObject fishingBobberPrefab;
    public Transform rodTip;
    public LineRenderer line;
    public float castForce = 12f;
    public Vector3 castDirection = new Vector3(0f, 0.3f, 1f);

    [Header("Fishing Line Curve")]
    [Min(2)] public int lineSegments = 12;
    public float lineSagAmount = 0.4f;
    public float lineSagDistanceScale = 6f;

    [Header("Water Surface")]
    public float defaultWaterSurfaceHeight = 0.2f;
    public float defaultBobberHeightOffset = 0f;
    public Transform waterSurfaceReference;

    [Header("Fishing Timing/Feel")]
    public Vector2 biteTimeRange = new Vector2(1.5f, 4.0f);

    [Header("Fishing Animations")]
    [SerializeField] private string castAnimationTrigger = "CastRod";
    [SerializeField] private string catchAnimationTrigger = "FishCatch";
    [SerializeField] private string missAnimationTrigger = "FishEscape";
    [SerializeField] private float castAnimationDuration = 0.8f;
    [SerializeField] private float postCastDelay = 0.35f;
    [SerializeField] private string idleAnimationStateName = "Idle";
    [SerializeField] private string castAnimationStateName = "CastRod";
    [SerializeField] private int castAnimationLayer = 0;
    [Range(0f, 1f)] [SerializeField] private float castSpawnNormalizedTime = 0.45f;
    [SerializeField] private float castStateEntryTimeout = 0.3f;

    [Header("Fishing Feedback")]
    [SerializeField] private float biteBobberAmplitudeMultiplier = 2.2f;
    [SerializeField] private float biteBobberFrequencyMultiplier = 1.6f;
    [SerializeField] private float biteBobberExtraDuration = 0.4f;
    [SerializeField] private bool biteBobberAddImpulse = true;
    [SerializeField] private float catchBobberAmplitudeMultiplier = 2.8f;
    [SerializeField] private float catchBobberFrequencyMultiplier = 2.1f;
    [SerializeField] private float catchBobberDuration = 1.2f;

    [Header("Fishing Fight Visuals")]
    [SerializeField] private GameObject defaultFishFightPrefab;
    [SerializeField] private Transform fishVisualParent;
    [SerializeField] private float fishSpawnRadius = 2f;
    [SerializeField] private float fishSpawnDepthOffset = -0.2f;
    [SerializeField] private float fishSwimSpeed = 2f;
    [SerializeField] private float fishCatchJumpHeight = 1.2f;
    [SerializeField] private float fishCatchJumpDuration = 0.7f;
    [SerializeField] private float fishEscapeDuration = 1.2f;
    [SerializeField] private Transform fishCatchLandingPoint;

    // =========================
    // Internal State
    // =========================
    private CharacterController controller;
    private Animator animator;
    private float rotationVelocity;
    private bool mobileRun = false;
    private bool isFishing = false;
    private bool canFishHere = false;
    private GameObject activeBobberGO = null;
    private FishingBobber activeBobberComponent = null;
    private GameObject activeFishVisualGO = null;
    private FishFightActor activeFishActor = null;
    private bool fishFightResultPending = false;
    private bool fishFightWasSuccess = false;
    private GameObject spawnedRod = null;
    private bool pendingCastRelease = false;
    private bool hasBobberSpawnedThisCast = false;
    private FishingZone activeFishingZone = null;
    private readonly List<FishingZone> overlappingFishingZones = new List<FishingZone>();
    private int castAnimationStateHash = -1;
    private int idleAnimationStateHash = -1;

    void OnEnable()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        CacheAnimationStateHash();
        ResetFishingAnimatorState(forceIdle: true, resetCastTrigger: true);
        EnsureRodSocketAssigned();
    }

    void Start()
    {
        if (fishJournal == null)
            fishJournal = FindObjectOfType<FishJournalUI>(true);

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        CacheAnimationStateHash();
        ResetFishingAnimatorState(forceIdle: true, resetCastTrigger: true);

        EnsureRodSocketAssigned();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (runDust != null)
            runDust.Stop();
        if (runButton != null)
            runButton.onClick.AddListener(ToggleRun);

        if (line != null)
        {
            line.positionCount = 0;
            line.enabled = false;
        }

        if (fishJournal != null && fishDatabase != null)
            fishJournal.PopulateJournal(fishDatabase.fishList);
    }

    void Update()
    {
        if (!isFishing)
            HandleMovement();

        if (Input.GetKeyDown(KeyCode.F))
            TryStartFishing();

        if (Input.GetKeyDown(KeyCode.J) && fishJournal != null)
            fishJournal.ToggleJournal();

        if (line != null && line.enabled)
            UpdateFishingLine();
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (joystick != null && (Mathf.Abs(joystick.Horizontal) > 0.1f || Mathf.Abs(joystick.Vertical) > 0.1f))
        {
            h = joystick.Horizontal;
            v = joystick.Vertical;
        }

        Vector2 rawInput = new Vector2(h, v);
        if (rawInput.sqrMagnitude < inputDeadZone * inputDeadZone)
            rawInput = Vector2.zero;

        Vector3 worldInput = new Vector3(rawInput.x, 0f, rawInput.y);
        bool hasInput = rawInput != Vector2.zero;
        float currentSpeed = 0f;

        if (hasInput)
        {
            Vector3 moveDir;
            if (cameraTransform != null)
            {
                Vector3 forward = cameraTransform.forward;
                forward.y = 0f;
                forward.Normalize();
                Vector3 right = cameraTransform.right;
                right.y = 0f;
                right.Normalize();
                moveDir = (forward * rawInput.y + right * rawInput.x).normalized;
            }
            else
            {
                moveDir = worldInput.normalized;
            }

            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float yAngle = Mathf.SmoothDampAngle(transform.eulerANGLES.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, yAngle, 0f);

            bool running = Input.GetKey(KeyCode.LeftShift) || mobileRun;
            float speed = running ? runSpeed : walkSpeed;

            if (controller != null)
                controller.Move(moveDir * speed * Time.deltaTime);
            else
                transform.position += moveDir * speed * Time.deltaTime;

            currentSpeed = speed;
        }

        if (controller != null && !controller.isGrounded)
            controller.Move(Vector3.down * extraGravity * Time.deltaTime);

        if (animator != null)
        {
            float animSpeed = Mathf.Clamp01(currentSpeed / runSpeed);
            animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);
        }

        HandleRunDust(currentSpeed);
    }

    void HandleRunDust(float speed)
    {
        if (runDust == null || controller == null)
            return;

        bool runningGrounded = speed >= runSpeed - 0.1f && controller.isGrounded;
        if (runningGrounded)
        {
            if (!runDust.isPlaying)
                runDust.Play();
        }
        else if (runDust.isPlaying)
        {
            runDust.Stop();
        }
    }

    void ToggleRun() => mobileRun = !mobileRun;

    private void EnsureRodSocketAssigned()
    {
        if (rodSocket != null)
            return;

        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null && animator.isHuman)
        {
            Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            if (rightHand != null)
            {
                rodSocket = rightHand;
                return;
            }
        }

        Transform locatedSocket = FindChildByName(transform, "RodSocket");
        if (locatedSocket == null)
            locatedSocket = FindChildByName(transform, "RightHandSocket");
        if (locatedSocket == null)
            locatedSocket = FindChildByName(transform, "RightHand");

        rodSocket = locatedSocket;
    }

    public void EquipRod()
    {
        EnsureRodSocketAssigned();

        if (rodPrefab == null)
        {
            Debug.LogWarning("pPlayerController: Cannot equip rod because rodPrefab is not assigned.", this);
            return;
        }

        if (rodSocket == null)
        {
            Debug.LogWarning("pPlayerController: Cannot equip rod because rodSocket is not assigned or could not be found.", this);
            return;
        }

        if (spawnedRod != null)
            Destroy(spawnedRod);

        line = null;
        rodTip = null;

        spawnedRod = Instantiate(rodPrefab, rodSocket);
        spawnedRod.transform.localPosition = Vector3.zero;
        spawnedRod.transform.localRotation = Quaternion.identity;
        spawnedRod.transform.localPosition += rodOffsetPosition;
        spawnedRod.transform.localRotation *= Quaternion.Euler(rodOffsetRotation);
        spawnedRod.SetActive(true);

        AssignRodReferences(spawnedRod.transform);
    }

    public void UnequipRod()
    {
        if (spawnedRod != null)
        {
            Destroy(spawnedRod);
            spawnedRod = null;
        }

        if (line != null)
        {
            line.enabled = false;
            line.positionCount = 0;
        }

        line = null;
        rodTip = null;
        CleanupFishVisual();
    }

    public void TryStartFishing()
    {
        if (isFishing)
            return;

        if (fishingUI == null || fishDatabase == null || fishDatabase.fishList == null || fishDatabase.fishList.Count == 0)
            return;

        if (requireFishingZone && !canFishHere)
            return;

        Fish selectedFish = fishDatabase.GetRandomFish();
        if (selectedFish == null)
            return;

        EquipRod();

        StartCoroutine(FishingRoutine(selectedFish));
    }

    IEnumerator FishingRoutine(Fish selectedFish)
    {
        isFishing = true;
        hasBobberSpawnedThisCast = false;
        pendingCastRelease = true;

        if (animator != null)
        {
            animator.ResetTrigger(catchAnimationTrigger);
            animator.ResetTrigger(missAnimationTrigger);
            animator.SetTrigger(castAnimationTrigger);
        }

        yield return WaitForCastReleaseWindow();

        if (postCastDelay > 0f)
            yield return new WaitForSeconds(postCastDelay);

        if (fishingUI != null)
        {
            fishingUI.RunMinigame(
                selectedFish,
                (success, fish) => { OnFishingResult(success, fish); },
                biteTimeRange,
                duration =>
                {
                    TriggerBobberBiteCue(duration);
                    BeginFishFightVisual(duration, selectedFish);
                },
                progress => UpdateFishFightProgress(progress)
            );
        }

        while (isFishing)
        {
            if (line != null && line.enabled)
                UpdateFishingLine();

            yield return null;
        }

        if (fishFightResultPending)
        {
            yield return HandleFishFightOutcomeRoutine(fishFightWasSuccess);
            fishFightResultPending = false;
        }

        if (line != null)
            line.enabled = false;

        if (activeBobberGO != null)
            Destroy(activeBobberGO);

        activeBobberGO = null;
        activeBobberComponent = null;
        hasBobberSpawnedThisCast = false;

        UnequipRod();
    }

    void OnFishingResult(bool success, Fish fish)
    {
        pendingCastRelease = false;

        if (animator != null)
        {
            animator.ResetTrigger(castAnimationTrigger);
            animator.SetTrigger(success ? catchAnimationTrigger : missAnimationTrigger);
        }

        fishFightResultPending = true;
        fishFightWasSuccess = success;

        isFishing = false;

        if (success)
        {
            if (fish != null)
                fish.caught = true;

            if (fishJournal != null && fish != null)
                fishJournal.AddFish(fish);

            if (inventoryManager != null && fish != null)
                inventoryManager.AddFishToInventory(fish);

            Debug.Log($"🎣 You caught a {(fish != null ? fish.fishName : "fish")}!");
        }
        else
        {
            Debug.Log($"🐟 The {(fish != null ? fish.fishName : "fish")} got away...");
        }
    }

    private void TriggerBobberBiteCue(float struggleDuration)
    {
        if (activeBobberComponent == null)
            return;

        float duration = Mathf.Max(struggleDuration + biteBobberExtraDuration, 0.1f);
        activeBobberComponent.TriggerBite(biteBobberAmplitudeMultiplier, biteBobberFrequencyMultiplier, duration, biteBobberAddImpulse);
    }

    private void TriggerCatchBobberReaction(bool success)
    {
        if (activeBobberComponent == null)
            return;

        float amplitude = success ? catchBobberAmplitudeMultiplier : Mathf.Max(1f, biteBobberAmplitudeMultiplier);
        float frequency = success ? catchBobberFrequencyMultiplier : Mathf.Max(1f, biteBobberFrequencyMultiplier);
        float duration = success ? catchBobberDuration : Mathf.Max(catchBobberDuration * 0.5f, 0.35f);

        activeBobberComponent.TriggerBite(amplitude, frequency, duration, true);
    }

private void UpdateFishFightProgress(float progress)
    {
        if (activeFishActor == null)
            return;

        activeFishActor.UpdateFightProgress(progress);
    }

    private IEnumerator HandleFishFightOutcomeRoutine(bool success)
    {
        if (activeFishActor == null)
        {
            CleanupFishVisual();
            yield break;
        }

        if (success)
        {
            Transform landing = fishCatchLandingPoint != null ? fishCatchLandingPoint : rodSocket;
            yield return activeFishActor.PlayCatchAnimation(landing, fishCatchJumpHeight, fishCatchJumpDuration);
        }
        else
        {
            yield return activeFishActor.PlayEscapeAnimation(fishEscapeDuration, fishSpawnDepthOffset);
        }

        CleanupFishVisual();
    }

    private void CleanupFishVisual()
    {
        if (activeFishVisualGO != null)
        {
            Destroy(activeFishVisualGO);
            activeFishVisualGO = null;
        }
        activeFishActor = null;
    }


    // Animation event hook: called from the CastRod clip when the bobber should spawn.
    public void HandleCastReleaseEvent()
    {
        if (!pendingCastRelease || hasBobberSpawnedThisCast)
            return;

        SpawnFishingBall();
    }

    // Animation Event wrapper for clarity when wiring in the Animator timeline
    public void AnimationEvent_SpawnFishingBobber()
    {
        HandleCastReleaseEvent();
    }

    private void SpawnFishingBall()
    {
        pendingCastRelease = false;

        if (hasBobberSpawnedThisCast)
            return;

        hasBobberSpawnedThisCast = true;
        activeBobberGO = null;
        activeBobberComponent = null;

        if (fishingBobberPrefab != null)
        {
            float waterSurfaceY = ResolveWaterSurfaceHeight();
            float bobberOffset = ResolveBobberHeightOffset();
            Vector3 spawnPosition = bobberSpawnPoint != null ? bobberSpawnPoint.position : (rodTip != null ? rodTip.position : transform.position);
            Quaternion spawnRotation = bobberSpawnPoint != null ? bobberSpawnPoint.rotation : Quaternion.identity;

            activeBobberGO = Instantiate(fishingBobberPrefab, spawnPosition, spawnRotation);

            FishingBobber bobberComponent = activeBobberGO.GetComponent<FishingBobber>();
            if (bobberComponent != null)
            {
                bobberComponent.ConfigureSurface(waterSurfaceY, bobberOffset);
            }

            Rigidbody rb = activeBobberGO.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.position = spawnPosition;
                Vector3 aimForward = (cameraTransform != null ? cameraTransform.forward : transform.forward);
                Vector3 dir = (aimForward + castDirection).normalized;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(dir * castForce, ForceMode.VelocityChange);
            }
        }

        if (line != null && rodTip != null)
        {
            line.enabled = true;
            UpdateFishingLine();
        }
    }

    private void UpdateFishingLine()
    {
        if (line == null || rodTip == null)
            return;

        Vector3 start = rodTip.position;
        Vector3 end = activeBobberGO != null ? activeBobberGO.transform.position : start + transform.forward * 2f;

        int segments = GetLineSegmentCount();
        if (line.positionCount != segments)
            line.positionCount = segments;

        float distance = Vector3.Distance(start, end);
        float normalizedDistance = Mathf.Clamp01(distance / Mathf.Max(0.001f, lineSagDistanceScale));
        float sagStrength = lineSagAmount * normalizedDistance;

        for (int i = 0; i < segments; i++)
        {
            float t = (segments <= 1) ? 0f : i / (segments - 1f);
            Vector3 point = Vector3.Lerp(start, end, t);

            float sagFactor = Mathf.Sin(Mathf.PI * t); // zero at ends, 1 at midpoint
            point.y -= sagFactor * sagStrength;

            line.SetPosition(i, point);
        }
    }

    private IEnumerator WaitForCastReleaseWindow()
    {
        if (!pendingCastRelease)
            yield break;

        if (animator == null)
        {
            yield return WaitForCastDurationFallback();
            yield break;
        }

        if (castAnimationStateHash == -1)
            CacheAnimationStateHash();
        ResetFishingAnimatorState();

        float enterTimer = 0f;
        bool castStateDetected = false;

        while (pendingCastRelease && enterTimer < castStateEntryTimeout)
        {
            if (TryGetCastStateInfo(out _, true))
            {
                castStateDetected = true;
                break;
            }

            enterTimer += Time.deltaTime;
            yield return null;
        }

        if (!castStateDetected)
        {
            yield return WaitForCastDurationFallback();
            yield break;
        }

        while (pendingCastRelease)
        {
            if (!TryGetCastStateInfo(out AnimatorStateInfo stateInfo, false))
            {
                HandleCastReleaseEvent();
                yield break;
            }

            float normalizedTime = stateInfo.normalizedTime % 1f;
            if (normalizedTime >= castSpawnNormalizedTime)
            {
                HandleCastReleaseEvent();
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator WaitForCastDurationFallback()
    {
        if (!pendingCastRelease)
            yield break;

        float fallbackDuration = castAnimationDuration > 0f ? castAnimationDuration : 0.2f;
        float elapsed = 0f;
        while (pendingCastRelease && elapsed < fallbackDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (pendingCastRelease)
            HandleCastReleaseEvent();
    }

    private bool TryGetCastStateInfo(out AnimatorStateInfo stateInfo, bool allowNextState)
    {
        stateInfo = default;

        if (animator == null || animator.layerCount == 0 || castAnimationStateHash == -1)
            return false;

        int layerIndex = Mathf.Clamp(castAnimationLayer, 0, animator.layerCount - 1);

        AnimatorStateInfo current = animator.GetCurrentAnimatorStateInfo(layerIndex);
        if (current.shortNameHash == castAnimationStateHash)
        {
            stateInfo = current;
            return true;
        }

        if (allowNextState)
        {
            AnimatorStateInfo next = animator.GetNextAnimatorStateInfo(layerIndex);
            if (next.shortNameHash == castAnimationStateHash)
            {
                stateInfo = next;
                return true;
            }
        }

        return false;
    }

    private float ResolveWaterSurfaceHeight()
    {
        if (activeFishingZone != null)
            return activeFishingZone.GetWaterSurfaceHeight();

        if (waterSurfaceReference != null)
            return waterSurfaceReference.position.y;

        return defaultWaterSurfaceHeight;
    }

    private float ResolveBobberHeightOffset()
    {
        if (activeFishingZone != null)
            return activeFishingZone.bobberHeightOffset;

        return defaultBobberHeightOffset;
    }

    private int GetLineSegmentCount()
    {
        return Mathf.Max(2, lineSegments);
    }

    private void CacheAnimationStateHash()
    {
        if (!string.IsNullOrEmpty(castAnimationStateName))
            castAnimationStateHash = Animator.StringToHash(castAnimationStateName);
        else
            castAnimationStateHash = -1;

        if (!string.IsNullOrEmpty(idleAnimationStateName))
            idleAnimationStateHash = Animator.StringToHash(idleAnimationStateName);
        else
            idleAnimationStateHash = -1;
    }

    private void ResetFishingAnimatorState(bool forceIdle = false, bool resetCastTrigger = false)
    {
        if (animator == null)
            return;

        if (resetCastTrigger && !string.IsNullOrEmpty(castAnimationTrigger))
            animator.ResetTrigger(castAnimationTrigger);
        if (!string.IsNullOrEmpty(catchAnimationTrigger))
            animator.ResetTrigger(catchAnimationTrigger);
        if (!string.IsNullOrEmpty(missAnimationTrigger))
            animator.ResetTrigger(missAnimationTrigger);

        if (!forceIdle || string.IsNullOrEmpty(idleAnimationStateName))
            return;

        int targetHash = idleAnimationStateHash;

        bool hasState = targetHash >= 0 && animator.HasState(castAnimationLayer, targetHash);
        if (!hasState)
        {
            int fallbackHash = Animator.StringToHash($"Base Layer.{idleAnimationStateName}");
            if (animator.HasState(castAnimationLayer, fallbackHash))
            {
                idleAnimationStateHash = fallbackHash;
                targetHash = fallbackHash;
                hasState = true;
            }
        }

        if (hasState)
            animator.Play(targetHash, castAnimationLayer, 0f);
    }

    private void AssignRodReferences(Transform rodRoot)
    {
        if (rodRoot == null)
            return;

        var instantiatedLine = rodRoot.GetComponentInChildren<LineRenderer>();
        if (instantiatedLine != null)
        {
            line = instantiatedLine;
            line.useWorldSpace = true;
            line.positionCount = GetLineSegmentCount();
            line.enabled = false;
        }
        else
        {
            Debug.LogWarning("pPlayerController: No LineRenderer found on spawned rod prefab.");
        }

        Transform locatedRodTip = FindChildByName(rodRoot, "Linerenderpoint");
        if (locatedRodTip != null)
        {
            rodTip = locatedRodTip;
        }
        else if (rodTip == null)
        {
            rodTip = rodRoot;
        }

        if (bobberSpawnPoint == null)
        {
            bobberSpawnPoint = rodTip;
        }
    }

    private Transform FindChildByName(Transform parent, string targetName)
    {
        if (parent == null || string.IsNullOrEmpty(targetName))
            return null;

        foreach (Transform child in parent)
        {
            if (string.Equals(child.name, targetName, StringComparison.OrdinalIgnoreCase))
                return child;

            Transform nested = FindChildByName(child, targetName);
            if (nested != null)
                return nested;
        }

        return null;
    }


    // =========================
    // Fishing Zone Detection
    // =========================
    private void OnTriggerEnter(Collider other)
    {
        FishingZone zone = other.GetComponent<FishingZone>();
        if (zone != null)
        {
            if (!overlappingFishingZones.Contains(zone))
                overlappingFishingZones.Add(zone);

            activeFishingZone = overlappingFishingZones[overlappingFishingZones.Count - 1];
            canFishHere = true;
            return;
        }

        if (requireFishingZone && other.CompareTag("FishingZone"))
            canFishHere = true;
    }

    private void OnTriggerExit(Collider other)
    {
        FishingZone zone = other.GetComponent<FishingZone>();
        if (zone != null)
        {
            overlappingFishingZones.Remove(zone);

            if (overlappingFishingZones.Count > 0)
            {
                activeFishingZone = overlappingFishingZones[overlappingFishingZones.Count - 1];
                canFishHere = true;
            }
            else
            {
                activeFishingZone = null;
                canFishHere = false;
            }

            return;
        }

        if (requireFishingZone && other.CompareTag("FishingZone"))
            canFishHere = false;
    }
}

























































