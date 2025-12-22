using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Player controller – movement plus fishing flow.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class pPlayerController : MonoBehaviour
{
    public static pPlayerController Instance;
    public bool canMove = true;

    void Awake()
    {
        Instance = this;
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }
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
    // Gathering / Mining System
    // =========================
    [Header("Gathering Settings")]
    public KeyCode interactKey = KeyCode.E;
    public float interactHoldTime = 1.0f;
    public ToolType equippedTool = ToolType.Pickaxe; // Your enum (e.g., Axe, Pickaxe, etc.)
    public Animator gatherAnimator; // Can be same as main animator
    public string gatherAnimationTrigger = "Hit"; // Trigger name in Animator
    [Min(1)] public int smashCyclesRequired = 2;

    [Header("Gathering UI Prompt")]
    public GameObject interactUI; // “Press E to mine” prompt

    private GatherableResource currentResource;
    private float holdTimer;
    private bool isHoldingE;
    private int completedCycles;

    // =========================
    // Fishing – Core Settings
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
    [Range(0f, 1f)][SerializeField] private float castSpawnNormalizedTime = 0.45f;
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
    [SerializeField] private float fishCatchJumpHeight = 2.0f;
    [SerializeField] private float fishCatchJumpDuration = 0.7f;
    [SerializeField] private float fishEscapeDuration = 1.2f;
    [SerializeField] private Transform fishCatchLandingPoint;

    [Header("Fish Visual Tuning")]
    [Tooltip("Uniform scale applied to the spawned fish visual.")]
    [SerializeField] private float fishVisualScale = 1.4f;
    [Tooltip("If true, spawns the fish on the bobber side that faces the player.")]
    [SerializeField] private bool spawnFishTowardsPlayer = true;
    [SerializeField] private float fishApproachDuration = 1.25f;
    [Header("Fishing Line Tension")]
    [SerializeField] private float slackLineSagValue = 0.6f;
    [SerializeField] private float tensionSagMinValue = 0.05f;
    [SerializeField] private float tensionSagMaxValue = 0.2f;
    [SerializeField] private float lineTensionRecoverSpeed = 2.5f;
    [SerializeField] private float tensionPulseInterval = 0.6f;
    [SerializeField] private float tensionPulseAmplitude = 0.9f;
    [SerializeField] private float tensionPulseFrequency = 2.4f;

    // =========================
    // Internal State
    // =========================
    private CharacterController controller;
    private Animator animator;
    private float rotationVelocity;
    private bool mobileRun = false;
    private bool isFishing = false;
    private bool canFishHere = false;
    private GameObject spawnedRod = null;

    private GameObject activeBobberGO = null;
    private FishingBobber activeBobberComponent = null;

    private GameObject activeFishVisualGO = null;
    private FishFightActor activeFishActor = null;
    private bool fishFightResultPending = false;
    private bool fishFightWasSuccess = false;

    private bool pendingCastRelease = false;
    private bool hasBobberSpawnedThisCast = false;
    private FishingZone activeFishingZone = null;
    private readonly List<FishingZone> overlappingFishingZones = new List<FishingZone>();
    private int castAnimationStateHash = -1;
    private int idleAnimationStateHash = -1;

    private float currentLineSagMultiplier = 1f;
    private float targetLineSagMultiplier = 1f;
    private Coroutine tensionPulseRoutine = null;

    void OnEnable()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        CacheAnimationStateHash();
        ResetFishingAnimatorState(forceIdle: true, resetCastTrigger: true);
    }

    void Start()
    {
        if (fishJournal == null)
            fishJournal = FindObjectOfType<FishJournalUI>(true);
        if (fishingUI == null)
            fishingUI = FindObjectOfType<FishingMinigameUI>(true);

        if (fishDatabase == null)
            fishDatabase = FindObjectOfType<FishDatabase>(true);

        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager>(true);

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        CacheAnimationStateHash();
        ResetFishingAnimatorState(forceIdle: true, resetCastTrigger: true);

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

        float slackMultiplier = ComputeSagMultiplier(slackLineSagValue);
        currentLineSagMultiplier = slackMultiplier;
        targetLineSagMultiplier = slackMultiplier;

        if (fishJournal != null && fishDatabase != null)
            fishJournal.PopulateJournal(fishDatabase.fishList);
    }

    void Update()
    {


   
        
        if (!isFishing)
            HandleMovement();

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Fishing: F pressed");
            TryStartFishing();
        }

        if (Input.GetKeyDown(KeyCode.J) && fishJournal != null)
            fishJournal.ToggleJournal();

        currentLineSagMultiplier = Mathf.MoveTowards(currentLineSagMultiplier, targetLineSagMultiplier, lineTensionRecoverSpeed * Time.deltaTime);

        if (line != null && line.enabled)
            UpdateFishingLine();

        // --- Resource Gathering ---
        HandleResourceInteraction();

    }

    void HandleResourceInteraction()
    {
        if (currentResource == null)
        {
            isHoldingE = false;
            holdTimer = 0f;
            completedCycles = 0;

            if (interactUI != null && interactUI.activeSelf)
                interactUI.SetActive(false);
            return;
        }

        if (interactUI != null && !interactUI.activeSelf)
            interactUI.SetActive(true);

        if (Input.GetKey(interactKey))
        {
            isHoldingE = true;
            holdTimer += Time.deltaTime;

            while (holdTimer >= interactHoldTime)
            {
                holdTimer -= interactHoldTime;
                completedCycles++;

                if (completedCycles >= smashCyclesRequired)
                {
                    completedCycles = 0;
                    currentResource.ApplyHit(equippedTool);
                }

                if (currentResource != null)
                    PlayGatherAnimation();
            }
        }

        if (Input.GetKeyUp(interactKey))
        {
            isHoldingE = false;
            holdTimer = 0f;
            completedCycles = 0;
        }
    }

    public void PlayGatherAnimation()
    {
        if (gatherAnimator != null)
            gatherAnimator.SetTrigger(gatherAnimationTrigger);
    }

    // --- Movement ---------------------------------------------------------

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
            float yAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
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

    // --- Rod handling -----------------------------------------------------

    public void EquipRod()
    {
        if (rodPrefab == null || rodSocket == null)
            return;

        if (spawnedRod != null)
            Destroy(spawnedRod);

        line = null;
        rodTip = null;

        spawnedRod = Instantiate(rodPrefab, rodSocket);
        spawnedRod.transform.localPosition = Vector3.zero;
        spawnedRod.transform.localRotation = Quaternion.identity;
        spawnedRod.transform.localPosition += rodOffsetPosition;
        spawnedRod.transform.localRotation *= Quaternion.Euler(rodOffsetRotation);

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

    // --- Fishing flow -----------------------------------------------------

    public void TryStartFishing()
    {
        if (isFishing)
        {
            Debug.Log("Fishing: already in progress");
            return;
        }

        if (fishingUI == null)
        {
            Debug.LogWarning("Fishing: fishingUI reference not set on pPlayerController");
            return;
        }

        if (fishDatabase == null)
        {
            Debug.LogWarning("Fishing: fishDatabase reference not set on pPlayerController");
            return;
        }

        if (fishDatabase.fishList == null || fishDatabase.fishList.Count == 0)
        {
            Debug.LogWarning("Fishing: fishDatabase has no fish entries");
            return;
        }

        if (requireFishingZone && !canFishHere)
        {
            Debug.Log("Fishing: not in a FishingZone (enable gizmo/ensure trigger enter)");
            return;
        }

        Fish selectedFish = fishDatabase.GetRandomFish();
        if (selectedFish == null)
        {
            Debug.LogWarning("Fishing: GetRandomFish() returned null");
            return;
        }

        Debug.Log($"Fishing: starting cast for {selectedFish.fishName}");
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

        yield return FishApproachSequence(selectedFish);
        if (fishingUI != null)
        {
            fishingUI.RunMinigame(
                selectedFish,
                (success, fish) => { OnFishingResult(success, fish); },
                biteTimeRange,
                duration =>
                {
                    PrepareFishFightForMinigame(selectedFish);
                    TriggerBobberBiteCue(duration);
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
    }

    void OnFishingResult(bool success, Fish fish)
    {
        EndLineTension();
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
        activeBobberComponent.TriggerBite(
            biteBobberAmplitudeMultiplier,
            biteBobberFrequencyMultiplier,
            duration,
            biteBobberAddImpulse);
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

    // --- Fish fight visuals -----------------------------------------------

    private void BeginFishFightVisual(float struggleDuration, Fish fish, bool markResultPending = true)
    {
        if (activeBobberComponent == null)
            return;

        CleanupFishVisual();

        GameObject prefab = (fish != null && fish.fightVisualPrefab != null)
            ? fish.fightVisualPrefab
            : defaultFishFightPrefab;

        Vector3 bobberPos = activeBobberComponent.transform.position;
        Vector3 spawnPos;

        if (spawnFishTowardsPlayer)
        {
            Transform target = fishCatchLandingPoint != null ? fishCatchLandingPoint : (rodSocket != null ? rodSocket : transform);
            Vector3 toPlayer = (target != null ? (target.position - bobberPos) : transform.forward);
            Vector3 dir = new Vector3(toPlayer.x, 0f, toPlayer.z).normalized;
            if (dir.sqrMagnitude < 0.0001f)
                dir = Vector3.forward;
            spawnPos = bobberPos - dir * Mathf.Max(0.25f, fishSpawnRadius);
            Vector2 jitter = UnityEngine.Random.insideUnitCircle * 0.25f;
            spawnPos += new Vector3(jitter.x, 0f, jitter.y);
            spawnPos.y = bobberPos.y + fishSpawnDepthOffset;
        }
        else
        {
            Vector2 circle = UnityEngine.Random.insideUnitCircle;
            if (circle.sqrMagnitude < 0.0001f)
                circle = Vector2.right;
            circle = circle.normalized * Mathf.Max(0.25f, fishSpawnRadius);
            spawnPos = bobberPos + new Vector3(circle.x, 0f, circle.y);
            spawnPos.y = bobberPos.y + fishSpawnDepthOffset;
        }

        Transform parent = fishVisualParent != null ? fishVisualParent : transform;

        if (prefab != null)
        {
            activeFishVisualGO = Instantiate(prefab, spawnPos, Quaternion.identity, parent);
            if (fishVisualScale > 0f)
                activeFishVisualGO.transform.localScale = Vector3.one * fishVisualScale;
        }
        else
        {
            Debug.LogWarning("pPlayerController: No fight visual prefab assigned. Using placeholder capsule.", this);
            activeFishVisualGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            activeFishVisualGO.transform.SetParent(parent, worldPositionStays: false);
            activeFishVisualGO.transform.localScale = Vector3.one * (fishVisualScale > 0f ? fishVisualScale : 0.6f);
            Collider placeholderCollider = activeFishVisualGO.GetComponent<Collider>();
            if (placeholderCollider != null)
                Destroy(placeholderCollider);
            activeFishVisualGO.transform.position = spawnPos;
        }

        activeFishActor = activeFishVisualGO.GetComponent<FishFightActor>();
        if (activeFishActor == null)
            activeFishActor = activeFishVisualGO.AddComponent<FishFightActor>();

        Transform landing = fishCatchLandingPoint != null ? fishCatchLandingPoint : rodSocket;
        activeFishActor.Configure(activeBobberComponent.transform,
                                  landing != null ? landing : transform,
                                  spawnPos,
                                  bobberPos.y,
                                  fishSwimSpeed,
                                  fishSpawnDepthOffset);

        if (markResultPending)
        {
            fishFightResultPending = true;
            fishFightWasSuccess = false;
        }

        activeFishActor.UpdateFightProgress(0f);
    }

    private IEnumerator FishApproachSequence(Fish fish)
    {
        float waitTimer = 0f;
        while (activeBobberComponent == null)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer > 2f)
                break;
            yield return null;
        }

        BeginFishFightVisual(0f, fish, markResultPending: false);

        if (activeFishActor == null || activeBobberComponent == null)
            yield break;

        float duration = Mathf.Max(0.1f, fishApproachDuration);
        float timer = 0f;
        while (timer < duration && activeFishActor != null && activeBobberComponent != null)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            activeFishActor.UpdateFightProgress(t);
            yield return null;
        }

        if (activeFishActor != null)
            activeFishActor.UpdateFightProgress(1f);

        if (activeBobberComponent != null)
            TriggerBobberBiteCue(0.35f);

        BeginLineTension();
    }

    private void BeginLineTension()
    {
        float minMultiplier = ComputeSagMultiplier(tensionSagMinValue);
        targetLineSagMultiplier = minMultiplier;
        currentLineSagMultiplier = minMultiplier;

        if (tensionPulseRoutine != null)
            StopCoroutine(tensionPulseRoutine);
        tensionPulseRoutine = StartCoroutine(LineTensionPulseRoutine());
    }

    private void EndLineTension()
    {
        targetLineSagMultiplier = ComputeSagMultiplier(slackLineSagValue);
        currentLineSagMultiplier = targetLineSagMultiplier;
        if (tensionPulseRoutine != null)
        {
            StopCoroutine(tensionPulseRoutine);
            tensionPulseRoutine = null;
        }
    }

    private IEnumerator LineTensionPulseRoutine()
    {
        bool toggle = false;
        while (true)
        {
            if (activeBobberComponent != null)
                activeBobberComponent.TriggerBite(tensionPulseAmplitude, tensionPulseFrequency, 0.3f, true);

            float minMultiplier = ComputeSagMultiplier(tensionSagMinValue);
            float maxMultiplier = ComputeSagMultiplier(tensionSagMaxValue);
            targetLineSagMultiplier = toggle ? maxMultiplier : minMultiplier;
            toggle = !toggle;

            yield return new WaitForSeconds(Mathf.Max(0.1f, tensionPulseInterval));
        }
    }

    private void PrepareFishFightForMinigame(Fish fish)
    {
        EndLineTension();

        if (activeFishActor == null)
            BeginFishFightVisual(0f, fish, markResultPending: false);

        fishFightResultPending = true;
        fishFightWasSuccess = false;

        if (activeFishActor != null)
            activeFishActor.UpdateFightProgress(0f);
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
        EndLineTension();
        if (activeFishVisualGO != null)
        {
            Destroy(activeFishVisualGO);
            activeFishVisualGO = null;
        }
        activeFishActor = null;
    }

    // --- Bobber/line helpers ----------------------------------------------

    public void HandleCastReleaseEvent()
    {
        if (!pendingCastRelease || hasBobberSpawnedThisCast)
            return;

        SpawnFishingBall();
    }

    public void AnimationEvent_SpawnFishingBobber()
    {
        HandleCastReleaseEvent();
    }

    private void SpawnFishingBall()
    {
        pendingCastRelease = false;
        targetLineSagMultiplier = ComputeSagMultiplier(slackLineSagValue);
        currentLineSagMultiplier = targetLineSagMultiplier;


        if (hasBobberSpawnedThisCast)
            return;

        hasBobberSpawnedThisCast = true;
        activeBobberGO = null;
        activeBobberComponent = null;

        if (fishingBobberPrefab != null)
        {
            float waterSurfaceY = ResolveWaterSurfaceHeight();
            float bobberOffset = ResolveBobberHeightOffset();
            Vector3 spawnPosition = bobberSpawnPoint != null ? bobberSpawnPoint.position :
                                    (rodTip != null ? rodTip.position : transform.position);
            Quaternion spawnRotation = bobberSpawnPoint != null ? bobberSpawnPoint.rotation : Quaternion.identity;

            activeBobberGO = Instantiate(fishingBobberPrefab, spawnPosition, spawnRotation);

            activeBobberComponent = activeBobberGO.GetComponent<FishingBobber>();
            if (activeBobberComponent != null)
            {
                activeBobberComponent.ConfigureSurface(waterSurfaceY, bobberOffset);
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
        float sagStrength = lineSagAmount * currentLineSagMultiplier * normalizedDistance;

        for (int i = 0; i < segments; i++)
        {
            float t = (segments <= 1) ? 0f : i / (segments - 1f);
            Vector3 point = Vector3.Lerp(start, end, t);
            float sagFactor = Mathf.Sin(Mathf.PI * t);
            point.y -= sagFactor * sagStrength;
            line.SetPosition(i, point);
        }
    }

    // --- Animation helpers ------------------------------------------------

    IEnumerator WaitForCastReleaseWindow()
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

    IEnumerator WaitForCastDurationFallback()
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

    bool TryGetCastStateInfo(out AnimatorStateInfo stateInfo, bool allowNextState)
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

    private float ComputeSagMultiplier(float sagValue)
    {
        float baseSag = Mathf.Max(0.01f, lineSagAmount);
        return Mathf.Clamp(sagValue / baseSag, 0.01f, 5f);
    }


    float ResolveWaterSurfaceHeight()
    {
        if (activeFishingZone != null)
            return activeFishingZone.GetWaterSurfaceHeight();

        if (waterSurfaceReference != null)
            return waterSurfaceReference.position.y;

        return defaultWaterSurfaceHeight;
    }

    float ResolveBobberHeightOffset()
    {
        if (activeFishingZone != null)
            return activeFishingZone.bobberHeightOffset;

        return defaultBobberHeightOffset;
    }

    int GetLineSegmentCount() => Mathf.Max(2, lineSegments);

    void CacheAnimationStateHash()
    {
        castAnimationStateHash = !string.IsNullOrEmpty(castAnimationStateName)
            ? Animator.StringToHash(castAnimationStateName)
            : -1;

        idleAnimationStateHash = !string.IsNullOrEmpty(idleAnimationStateName)
            ? Animator.StringToHash(idleAnimationStateName)
            : -1;
    }

    void ResetFishingAnimatorState(bool forceIdle = false, bool resetCastTrigger = false)
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

    void AssignRodReferences(Transform rodRoot)
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

        var rodController = rodRoot.GetComponentInChildren<FishingRodController>();
        if (rodController != null)
        {
            rodController.ConfigureLineStart(rodTip);
        }

        if (bobberSpawnPoint == null)
            bobberSpawnPoint = rodTip;
    }

    Transform FindChildByName(Transform parent, string targetName)
    {
        if (parent == null || string.IsNullOrEmpty(targetName))
            return null;

        string trimmedTarget = targetName.Trim();

        foreach (Transform child in parent)
        {
            string childName = child.name;
            if (string.Equals(childName, targetName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(childName?.Trim(), trimmedTarget, StringComparison.OrdinalIgnoreCase))
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
    void OnTriggerEnter(Collider other)
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


        if (other.TryGetComponent(out GatherableResource resource))
        {
            currentResource = resource;
            holdTimer = 0f;
            completedCycles = 0;
            isHoldingE = false;
        }
    }



    void OnTriggerExit(Collider other)
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

        if (other.TryGetComponent(out GatherableResource resource) && resource == currentResource)
        {
            currentResource = null;
            isHoldingE = false;
            holdTimer = 0f;
            completedCycles = 0;

            if (interactUI != null)
                interactUI.SetActive(false);
        }

    }
}










































