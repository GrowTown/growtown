using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Types of tools the player can equip for gathering. Extend this enum if you add more tool categories.
/// </summary>
public enum ResourceToolType
{
    Any = 0,
    Axe = 1,
    Pickaxe = 2,
    Sickle = 3,
    Hammer = 4
}

/// <summary>
/// High level resource categories for analytics, drops, VFX mapping, etc.
/// </summary>
public enum ResourceCategory
{
    Grass,
    Tree,
    Rock,
    Ore
}

/// <summary>
/// Data for a single drop spawned when the resource is harvested.
/// </summary>
[Serializable]
public sealed class ResourceDrop
{
    [Tooltip("Prefab spawned when the node is harvested. Leave empty to skip spawning.")]
    public GameObject prefab;

    [Tooltip("How many prefabs to spawn. Set to 1 for a single item.")]
    public int amount = 1;

    [Tooltip("Optional offset applied to each spawned prefab.")]
    public Vector3 spawnOffset = Vector3.zero;

    [Range(0f, 1f)]
    [Tooltip("Chance that this drop will be spawned (1 = always, 0.25 = 25% chance).")]
    public float chance = 1f;
}

/// <summary>
/// Generic gatherable resource node (rocks, trees, grass, ore veins, etc).
/// Attach to a resource prefab and configure visuals & drops from the inspector.
/// </summary>
[DisallowMultipleComponent]
public sealed class GatherableResource : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string resourceName = "Resource Node";
    [SerializeField] private ResourceCategory category = ResourceCategory.Rock;
    [SerializeField] private ResourceToolType requiredTool = ResourceToolType.Any;

    [Header("Durability")]
    [Min(1)] [SerializeField] private int hitsToBreak = 3;
    [Tooltip("Delay before the node respawns. Use a negative value to disable respawn.")]
    [SerializeField] private float respawnDelay = -1f;

    [Header("Visual State")]
    [Tooltip("Root object that represents the intact node. It will be disabled when harvested.")]
    [SerializeField] private GameObject intactVisual;
    [Tooltip("Optional shattered/harvested root that activates once the node is depleted.")]
    [SerializeField] private GameObject harvestedVisual;

    [Header("Drops & Feedback")]
    [SerializeField] private List<ResourceDrop> drops = new List<ResourceDrop>();
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private ParticleSystem harvestParticles;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip harvestClip;

    [Header("Interaction")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private KeyCode interactKey = KeyCode.E;
#if ENABLE_INPUT_SYSTEM
    [SerializeField] private InputActionReference interactAction;
#endif
    [Tooltip("How long the player must hold the interact key to break the resource.")]
    [SerializeField] private float interactHoldDuration = 1.5f;
    [Tooltip("Root object for the interaction UI (e.g. world space canvas).")]
    [SerializeField] private GameObject interactionUIRoot;
    [Tooltip("Image whose fill amount reflects the hold progress. Optional.")]
    [SerializeField] private Image interactionFillImage;
    [Tooltip("Enable this while developing to print interaction events to the console.")]
    [SerializeField] private bool debugLogging = false;

    [Header("Events")]
    public UnityEvent onHit;
    public UnityEvent onHarvested;
    public UnityEvent onRespawned;

    private int hitsRemaining;
    private bool isHarvested;
    private float respawnTime;
    private float interactTimer;
    private Collider currentInteractor;
    private bool interactorInRange;
    private bool isHoldingInteraction;

    /// <summary>
    /// Returns true if this resource can be harvested via the interact key hold mechanic.
    /// Resources that require a specific tool (e.g. hammer) should only break from tool hits.
    /// </summary>
    private bool AllowsManualInteraction()
    {
        return requiredTool == ResourceToolType.Any;
    }

    public ResourceCategory Category => category;
    public ResourceToolType RequiredTool => requiredTool;
    public string ResourceName => resourceName;

    void Awake()
    {
        hitsRemaining = Mathf.Max(1, hitsToBreak);
        ApplyVisualState();
        UpdateInteractionVisuals(false, 0f);
    }

    void OnEnable()
    {
#if ENABLE_INPUT_SYSTEM
        EnableActionIfNeeded(interactAction);
#endif
    }

    void OnDisable()
    {
#if ENABLE_INPUT_SYSTEM
        DisableActionIfNeeded(interactAction);
#endif
    }

    void Update()
    {
        HandleRespawn();

        bool keyDown = GetInteractPressedThisFrame();
        bool keyHeld = GetInteractHeld();
        bool keyUp = GetInteractReleasedThisFrame();

        if (keyDown)
            LogDebug($"Interact key pressed. InRange={interactorInRange}, Harvested={isHarvested}");

        HandleInteraction(keyDown, keyHeld, keyUp);
    }

    /// <summary>
    /// Applies a hit from the given tool type. Returns true if the hit was accepted.
    /// </summary>
    public bool ApplyHit(ResourceToolType toolUsed)
    {
        if (isHarvested)
            return false;

        if (requiredTool != ResourceToolType.Any && toolUsed != requiredTool)
            return false;

        hitsRemaining = Mathf.Max(0, hitsRemaining - 1);
        PlayHitFeedback();
        onHit?.Invoke();

        if (hitsRemaining == 0)
            HandleHarvested();

        return true;
    }

    private void HandleRespawn()
    {
        if (!isHarvested || respawnDelay < 0f)
            return;

        if (Time.time >= respawnTime)
            Respawn();
    }

    private void HandleInteraction(bool keyDown, bool keyHeld, bool keyUp)
    {
        if (!AllowsManualInteraction())
        {
            if (keyDown)
                LogDebug("Manual interaction disabled. Use the correct tool to harvest.");
            return;
        }

        if (!interactorInRange)
        {
            if (keyDown)
                LogDebug("Cannot start interaction: player not inside trigger.");
            return;
        }

        if (isHarvested)
        {
            if (keyDown)
                LogDebug("Cannot interact: resource already harvested.");
            return;
        }

        if (keyHeld)
        {
            if (!isHoldingInteraction)
            {
                isHoldingInteraction = true;
                interactTimer = 0f;
                UpdateInteractionVisuals(true, 0f);
                LogDebug("Interact key pressed. Starting hold timer.");
            }

            if (interactHoldDuration <= 0f)
            {
                BreakInstantly();
                isHoldingInteraction = false;
                return;
            }

            interactTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(interactTimer / interactHoldDuration);
            UpdateInteractionVisuals(true, progress);

            if (interactTimer >= interactHoldDuration)
            {
                BreakInstantly();
                interactTimer = 0f;
                isHoldingInteraction = false;
                LogDebug("Hold timer completed. Resource will break.");
            }
        }
        else
        {
            if (isHoldingInteraction && keyUp)
            {
                LogDebug("Interact key released before completion.");
            }

            if (keyUp || isHoldingInteraction)
            {
                isHoldingInteraction = false;
                interactTimer = 0f;
            }

            if (interactTimer > 0f)
            {
                interactTimer = 0f;
                UpdateInteractionVisuals(true, 0f);
            }
        }
    }

    private void PlayHitFeedback()
    {
        if (hitParticles != null)
            hitParticles.Play();

        if (audioSource != null && hitClip != null)
            audioSource.PlayOneShot(hitClip);
    }

    private void HandleHarvested()
    {
        isHarvested = true;
        ApplyVisualState();
        SpawnDrops();
        UpdateInteractionVisuals(false, 0f);
        interactTimer = 0f;
        isHoldingInteraction = false;
        LogDebug("Resource harvested.");

        if (audioSource != null && harvestClip != null)
            audioSource.PlayOneShot(harvestClip);
        if (harvestParticles != null)
            harvestParticles.Play();

        onHarvested?.Invoke();

        if (respawnDelay >= 0f)
            respawnTime = Time.time + respawnDelay;
    }

    private void Respawn()
    {
        isHarvested = false;
        hitsRemaining = Mathf.Max(1, hitsToBreak);
        ApplyVisualState();
        interactTimer = 0f;
        if (currentInteractor != null)
            UpdateInteractionVisuals(AllowsManualInteraction(), 0f);
        onRespawned?.Invoke();
        LogDebug("Resource respawned and ready.");
    }

    private void ApplyVisualState()
    {
        if (intactVisual != null)
            intactVisual.SetActive(!isHarvested);
        if (harvestedVisual != null)
            harvestedVisual.SetActive(isHarvested);
    }

    private void SpawnDrops()
    {
        if (drops == null || drops.Count == 0)
            return;

        foreach (ResourceDrop drop in drops)
        {
            if (drop.prefab == null || drop.amount <= 0)
                continue;

            if (drop.chance <= 0f)
                continue;

            if (drop.chance < 1f && UnityEngine.Random.value > drop.chance)
                continue;

            for (int i = 0; i < drop.amount; i++)
            {
                Vector3 spawnPos = transform.position + drop.spawnOffset;
                Instantiate(drop.prefab, spawnPos, Quaternion.identity);
            }

            LogDebug($"Spawned drop '{drop.prefab.name}' x{drop.amount}.");
        }
    }

    private void BreakInstantly()
    {
        if (isHarvested)
            return;

        hitsRemaining = 0;
        HandleHarvested();
        LogDebug("BreakInstantly triggered.");
    }

    private void UpdateInteractionVisuals(bool visible, float fill)
    {
        if (interactionUIRoot != null)
            interactionUIRoot.SetActive(visible);

        if (interactionFillImage != null)
            interactionFillImage.fillAmount = Mathf.Clamp01(fill);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        currentInteractor = other;
        interactorInRange = true;
        interactTimer = 0f;

        if (!isHarvested)
            UpdateInteractionVisuals(AllowsManualInteraction(), 0f);

        LogDebug($"Interactor '{other.name}' entered range.");
    }

    void OnTriggerExit(Collider other)
    {
        if (other != currentInteractor)
            return;

        currentInteractor = null;
        interactorInRange = false;
        interactTimer = 0f;
        UpdateInteractionVisuals(false, 0f);
        isHoldingInteraction = false;
        LogDebug($"Interactor '{other.name}' left range.");
    }

    private void LogDebug(string message)
    {
        if (!debugLogging)
            return;

        Debug.Log($"[GatherableResource] {resourceName}: {message}", this);
    }

    private bool GetInteractPressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        if (WasPressedThisFrame(interactAction))
            return true;
#endif
        return Input.GetKeyDown(interactKey);
    }

    private bool GetInteractHeld()
    {
#if ENABLE_INPUT_SYSTEM
        if (IsPressed(interactAction))
            return true;
#endif
        return Input.GetKey(interactKey);
    }

    private bool GetInteractReleasedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        if (WasReleasedThisFrame(interactAction))
            return true;
#endif
        return Input.GetKeyUp(interactKey);
    }

#if ENABLE_INPUT_SYSTEM
    private static void EnableActionIfNeeded(InputActionReference reference)
    {
        InputAction action = reference == null ? null : reference.action;
        if (action != null && !action.enabled)
            action.Enable();
    }

    private static void DisableActionIfNeeded(InputActionReference reference)
    {
        InputAction action = reference == null ? null : reference.action;
        if (action != null && action.enabled)
            action.Disable();
    }

    private static bool WasPressedThisFrame(InputActionReference reference)
    {
        InputAction action = reference == null ? null : reference.action;
        return action != null && action.WasPressedThisFrame();
    }

    private static bool WasReleasedThisFrame(InputActionReference reference)
    {
        InputAction action = reference == null ? null : reference.action;
        return action != null && action.WasReleasedThisFrame();
    }

    private static bool IsPressed(InputActionReference reference)
    {
        InputAction action = reference == null ? null : reference.action;
        return action != null && action.IsPressed();
    }
#endif
}
