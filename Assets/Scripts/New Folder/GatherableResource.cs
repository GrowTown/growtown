using System;
using System.Collections.Generic;
using System;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ResourceType
{
    Grass,
    Stone,
    Ore,
    Tree
}

public enum ToolType
{
    None,
    Sickle,
    Pickaxe,
    Axe,
    Hammer
}

[Serializable]
public sealed class ResourceDrop
{
    public GameObject prefab;
    public int amount = 1;
    public Vector3 spawnOffset = Vector3.zero;
    [Range(0f, 1f)] public float chance = 1f;
}

[DisallowMultipleComponent]
public sealed class GatherableResource : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string resourceName = "Resource Node";
    [SerializeField] private ResourceType category = ResourceType.Stone;
    [SerializeField] private ToolType requiredTool = ToolType.Pickaxe;

    [Header("Durability")]
    [Min(1)] [SerializeField] private int hitsToBreak = 3;
    [SerializeField] private float respawnDelay = -1f;

    [Header("Visual State")]
    [SerializeField] private GameObject intactVisual;
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
    [SerializeField] private GameObject interactionUIRoot;
    [SerializeField] private Image interactionFillImage;
    [SerializeField] private bool debugLogging = false;

    [Header("Events")]
    public UnityEvent onHit;
    public UnityEvent onHarvested;
    public UnityEvent onRespawned;

    private int hitsRemaining;
    private bool isHarvested;
    private float respawnTime;
    private Collider currentInteractor;
    private bool interactorInRange;

    public ResourceType Category => category;
    public ToolType RequiredTool => requiredTool;
    public string ResourceName => resourceName;

    void Awake()
    {
        hitsRemaining = Mathf.Max(1, hitsToBreak);
        ApplyVisualState();
        UpdateInteractionVisuals(false, 0f);
    }

    void Update()
    {
        HandleRespawn();

        if (Input.GetKeyDown(interactKey))
            HandleInteraction();
    }

    // ---------------- Interaction ----------------
    private void HandleInteraction()
    {
        if (!interactorInRange)
        {
            LogDebug("Cannot start interaction: player not inside trigger.");
            return;
        }

        if (isHarvested)
        {
            LogDebug("Cannot interact: resource already harvested.");
            return;
        }

        bool hitApplied = ApplyHit(requiredTool);
        if (hitApplied)
        {
            float progress = 1f - (float)hitsRemaining / hitsToBreak;
            UpdateInteractionVisuals(true, progress);
            LogDebug($"Hit applied! Remaining hits: {hitsRemaining}");
        }
    }

    // ---------------- Hammer Auto-Break ----------------
 private void OnCollisionEnter(Collision collision)
{
    // Only break if the resource category is Stone AND it's hit by a hammer
    if (category == ResourceType.Stone && collision.gameObject.CompareTag("Hammer"))
    {
        LogDebug($"Hammer hit {resourceName} — breaking instantly!");
        BreakInstantly();
    }
}

    // If you’re using triggers instead of collisions:
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            currentInteractor = other;
            interactorInRange = true;
            if (!isHarvested)
                UpdateInteractionVisuals(true, 0f);
            LogDebug($"Interactor '{other.name}' entered range.");
        }

        // Also check if a Hammer entered as a trigger collider
        if (other.CompareTag("Hammer"))
        {
            LogDebug($"Hammer trigger detected with {other.name} — breaking instantly.");
            BreakInstantly();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == currentInteractor)
        {
            currentInteractor = null;
            interactorInRange = false;
            UpdateInteractionVisuals(false, 0f);
            LogDebug($"Interactor '{other.name}' left range.");
        }
    }

    // ---------------- Hit Logic ----------------
    public bool ApplyHit(ToolType toolUsed)
    {
        if (isHarvested)
            return false;

        if (requiredTool != ToolType.None && toolUsed != requiredTool)
        {
            LogDebug($"Wrong tool! Requires {requiredTool}, used {toolUsed}");
            return false;
        }

        hitsRemaining = Mathf.Max(0, hitsRemaining - 1);
        PlayHitFeedback();
        onHit?.Invoke();

        if (hitsRemaining == 0)
            HandleHarvested();

        return true;
    }

    // ---------------- Break / Respawn / Visuals ----------------
    private void BreakInstantly()
    {
        if (isHarvested)
            return;

        hitsRemaining = 0;
        HandleHarvested();
        LogDebug("BreakInstantly triggered by hammer.");
    }

    private void HandleHarvested()
    {
        isHarvested = true;
        ApplyVisualState();
        SpawnDrops();
        UpdateInteractionVisuals(false, 0f);

        if (audioSource != null && harvestClip != null)
            audioSource.PlayOneShot(harvestClip);
        if (harvestParticles != null)
            harvestParticles.Play();

        onHarvested?.Invoke();

        if (respawnDelay >= 0f)
            respawnTime = Time.time + respawnDelay;

        LogDebug("Resource harvested!");
    }

    private void HandleRespawn()
    {
        if (!isHarvested || respawnDelay < 0f)
            return;

        if (Time.time >= respawnTime)
            Respawn();
    }

    private void Respawn()
    {
        isHarvested = false;
        hitsRemaining = Mathf.Max(1, hitsToBreak);
        ApplyVisualState();
        UpdateInteractionVisuals(true, 0f);
        onRespawned?.Invoke();
        LogDebug("Resource respawned and ready!");
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

    private void PlayHitFeedback()
    {
        if (hitParticles != null)
            hitParticles.Play();

        if (audioSource != null && hitClip != null)
            audioSource.PlayOneShot(hitClip);
    }

    private void UpdateInteractionVisuals(bool visible, float fill)
    {
        if (interactionUIRoot != null)
            interactionUIRoot.SetActive(visible);
        if (interactionFillImage != null)
            interactionFillImage.fillAmount = Mathf.Clamp01(fill);
    }

    private void LogDebug(string message)
    {
        if (!debugLogging) return;
        Debug.Log($"[GatherableResource] {resourceName}: {message}", this);
    }
}
