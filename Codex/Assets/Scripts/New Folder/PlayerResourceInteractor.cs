using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Handles player interactions with gatherable resources (rocks, grass, ore, trees).
/// Attach to the player alongside the existing controller.
/// </summary>
[DisallowMultipleComponent]
public sealed class PlayerResourceInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera lookCamera;
    [SerializeField] private Animator animator;

    [Header("Interaction")]
    [SerializeField] private float interactDistance = 2.5f;
    [SerializeField] private LayerMask resourceMask = ~0;
    [SerializeField] private float hitCooldown = 0.65f;
    [Tooltip("Animator trigger fired when a tool swing should play.")]
    [SerializeField] private string swingTrigger = "SwingTool";

    [Header("Equipped Tool")]
    [SerializeField] private ResourceToolType equippedTool = ResourceToolType.Pickaxe;
    [SerializeField] private KeyCode equipHammerKey = KeyCode.H;
    [Header("Tool Presentation")]
    [SerializeField] private string equippedToolAnimatorParameter = "ToolIndex";
    [SerializeField] private ToolVisualMapping[] toolVisuals = new ToolVisualMapping[0];
    [SerializeField] private bool logToolSwitches = false;
#if ENABLE_INPUT_SYSTEM
    [Header("Input Actions (Optional)")]
    [SerializeField] private InputActionReference swingAction;
    [SerializeField] private InputActionReference equipHammerAction;
#endif

    private float nextAllowedHitTime;
    private int swingTriggerHash = -1;
    private int equippedToolParamHash = -1;

    public ResourceToolType EquippedTool => equippedTool;

    [System.Serializable]
    private struct ToolVisualMapping
    {
        public ResourceToolType tool;
        public GameObject visualRoot;
    }

    void Awake()
    {
        if (lookCamera == null)
            lookCamera = Camera.main;

        CacheTriggerHash();
        CacheToolParamHash();
        ApplyEquippedTool(equippedTool, true);
    }

    void OnEnable()
    {
#if ENABLE_INPUT_SYSTEM
        EnableActionIfNeeded(swingAction);
        EnableActionIfNeeded(equipHammerAction);
#endif
    }

    void OnDisable()
    {
#if ENABLE_INPUT_SYSTEM
        DisableActionIfNeeded(swingAction);
        DisableActionIfNeeded(equipHammerAction);
#endif
    }

    void Update()
    {
        if (IsEquipHammerRequested())
            SetEquippedTool(ResourceToolType.Hammer);

        if (IsSwingRequested())
            TryStartSwing();
    }

    /// <summary>
    /// Entry point for UI buttons or touch controls.
    /// </summary>
    public void TriggerHit()
    {
        TryStartSwing();
    }

    /// <summary>
    /// Update the equipped tool at runtime (e.g., when the player swaps tools in the inventory).
    /// </summary>
    public void SetEquippedTool(ResourceToolType toolType)
    {
        if (equippedTool == toolType)
            return;

        equippedTool = toolType;
        ApplyEquippedTool(toolType, false);
    }

    /// <summary>
    /// Call this if you change the animator trigger name at runtime.
    /// </summary>
    public void SetSwingTrigger(string triggerName)
    {
        swingTrigger = triggerName;
        CacheTriggerHash();
    }

    private bool IsSwingReady => Time.time >= nextAllowedHitTime;

    private bool TryStartSwing()
    {
        if (!IsSwingReady)
            return false;

        PlaySwingAnimation();
        nextAllowedHitTime = Time.time + hitCooldown;
        return true;
    }

    /// <summary>
    /// Called by external hitboxes when a successful resource hit has been registered.
    /// Ensures cooldown enforcement for physics-driven impacts.
    /// </summary>
    public void NotifyHitRegistered()
    {
        nextAllowedHitTime = Mathf.Max(nextAllowedHitTime, Time.time + hitCooldown);
    }

    /// <summary>
    /// Returns true if the hit cooldown has elapsed and another swing can register a collision hit.
    /// </summary>
    public bool CanRegisterHit()
    {
        return Time.time >= nextAllowedHitTime;
    }

    private void PlaySwingAnimation()
    {
        if (animator == null || swingTriggerHash == -1)
            return;

        animator.SetTrigger(swingTriggerHash);
    }

    private void CacheTriggerHash()
    {
        swingTriggerHash = string.IsNullOrEmpty(swingTrigger) ? -1 : Animator.StringToHash(swingTrigger);
    }

    private void CacheToolParamHash()
    {
        equippedToolParamHash = string.IsNullOrEmpty(equippedToolAnimatorParameter)
            ? -1
            : Animator.StringToHash(equippedToolAnimatorParameter);
    }

    private void ApplyEquippedTool(ResourceToolType toolType, bool forceRefresh)
    {
        if (!forceRefresh && logToolSwitches)
            Debug.Log($"Equipped tool switched to {toolType}", this);

        UpdateToolVisuals(toolType);
        UpdateAnimatorToolParameter(toolType);
    }

    private void UpdateToolVisuals(ResourceToolType toolType)
    {
        if (toolVisuals == null || toolVisuals.Length == 0)
            return;

        for (int i = 0; i < toolVisuals.Length; i++)
        {
            ToolVisualMapping mapping = toolVisuals[i];
            if (mapping.visualRoot == null)
                continue;

            bool shouldBeActive = mapping.tool == toolType;
            if (mapping.visualRoot.activeSelf != shouldBeActive)
                mapping.visualRoot.SetActive(shouldBeActive);
        }
    }

    private void UpdateAnimatorToolParameter(ResourceToolType toolType)
    {
        if (animator == null || equippedToolParamHash == -1)
            return;

        animator.SetInteger(equippedToolParamHash, (int)toolType);
    }

    private bool IsEquipHammerRequested()
    {
#if ENABLE_INPUT_SYSTEM
        if (WasPressedThisFrame(equipHammerAction))
            return true;
#endif
        return Input.GetKeyDown(equipHammerKey);
    }

    private bool IsSwingRequested()
    {
#if ENABLE_INPUT_SYSTEM
        if (WasPressedThisFrame(swingAction))
            return true;
#endif
        return Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E);
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
#endif
}
