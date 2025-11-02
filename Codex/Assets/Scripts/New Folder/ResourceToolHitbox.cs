using UnityEngine;

/// <summary>
/// Detects physical collisions between a gathering tool (e.g., hammer) and a resource node,
/// forwarding the hit to <see cref="GatherableResource"/> when appropriate.
/// Attach this to the tool's collider GameObject.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public sealed class ResourceToolHitbox : MonoBehaviour
{
    [Header("Tool")]
    [SerializeField] private ResourceToolType toolType = ResourceToolType.Hammer;
    [Tooltip("If enabled, the equipped tool on the linked interactor overrides the Tool Type setting.")]
    [SerializeField] private bool useInteractorTool = false;

    [Header("Timing")]
    [Tooltip("Local cooldown to avoid multiple hits from a single contact.")]
    [SerializeField] private float localHitCooldown = 0.1f;
    [Tooltip("Skip registering hits while the interactor is still on cooldown.")]
    [SerializeField] private bool respectInteractorCooldown = true;

    [Header("References")]
    [SerializeField] private PlayerResourceInteractor interactor;
    [SerializeField] private bool debugLogging = false;

    private float nextLocalHitTime;

    void Awake()
    {
        if (interactor == null)
            interactor = GetComponentInParent<PlayerResourceInteractor>();
    }

    void OnCollisionEnter(Collision collision)
    {
        HandlePotentialHit(collision.collider);
    }

    void OnTriggerEnter(Collider other)
    {
        HandlePotentialHit(other);
    }

    private void HandlePotentialHit(Collider other)
    {
        if (Time.time < nextLocalHitTime)
            return;

        if (interactor != null && respectInteractorCooldown && !interactor.CanRegisterHit())
            return;

        GatherableResource resource = other.GetComponentInParent<GatherableResource>();
        if (resource == null)
            return;

        ResourceToolType appliedTool = DetermineToolType();
        if (!resource.ApplyHit(appliedTool))
        {
            LogDebug($"Resource '{resource.ResourceName}' ignored hit. RequiredTool={resource.RequiredTool}, AppliedTool={appliedTool}");
            return;
        }

        nextLocalHitTime = Time.time + localHitCooldown;

        if (interactor != null)
            interactor.NotifyHitRegistered();

        LogDebug($"Registered hit on '{resource.ResourceName}' using {appliedTool}.");
    }

    private ResourceToolType DetermineToolType()
    {
        if (useInteractorTool && interactor != null)
            return interactor.EquippedTool;

        return toolType;
    }

    private void LogDebug(string message)
    {
        if (!debugLogging)
            return;

        Debug.Log($"[ResourceToolHitbox] {message}", this);
    }
}
