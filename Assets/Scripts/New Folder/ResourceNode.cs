// ------------------------------------------------------------------------------
//  ResourceNode.cs
//  Controls individual harvestable objects: tracks health, handles break FX,
//  awards rewards, and manages respawn timing for the GrowTown gathering system.
// ------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ResourceNode : MonoBehaviour
{
    [Header("Resource Settings")]
    [SerializeField] private ResourceType resourceType = ResourceType.Grass;
    [SerializeField, Min(1)] private int maxHealth = 3;
    [SerializeField] private ShopItemHolder rewardItem;
    [SerializeField] private ParticleSystem breakEffect;
    [SerializeField, Min(0f)] private float respawnTime = 30f;

    [Header("Hit Feedback")]
    [SerializeField] private ParticleSystem hitEffect;

    private int currentHealth;
    private bool isRespawning;
    private Coroutine respawnRoutine;
    private readonly List<Collider> cachedColliders = new List<Collider>();
    private readonly List<Renderer> cachedRenderers = new List<Renderer>();

    public ResourceType ResourceType => resourceType;
    public bool IsAvailable => !isRespawning;

    private void Awake()
    {
        CacheNodeComponents();
        ResetHealth();
    }

    /// <summary>
    /// Apply a harvest hit to the node. Returns true when the hit was accepted and outputs the reward when the node breaks.
    /// </summary>
    public bool Harvest(out ShopItemHolder reward)
    {
        reward = null;

        if (isRespawning)
            return false;

        currentHealth = Mathf.Max(0, currentHealth - 1);

        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);

        if (currentHealth > 0)
            return true;

        reward = rewardItem;
        TriggerBreakEffect();
        BeginRespawnCountdown();
        return true;
    }

    private void TriggerBreakEffect()
    {
        if (breakEffect != null)
        {
            ParticleSystem fx = Instantiate(breakEffect, transform.position, transform.rotation);
            Destroy(fx.gameObject, fx.main.duration + fx.main.startLifetime.constantMax + 1f);
        }

        SetNodeActive(false);
    }

    private void BeginRespawnCountdown()
    {
        if (respawnRoutine != null)
            StopCoroutine(respawnRoutine);

        isRespawning = true;
        respawnRoutine = StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        if (respawnTime > 0f)
            yield return new WaitForSeconds(respawnTime);

        ResetHealth();
        SetNodeActive(true);
        isRespawning = false;
        respawnRoutine = null;
    }

    private void ResetHealth()
    {
        currentHealth = Mathf.Max(1, maxHealth);
    }

    private void CacheNodeComponents()
    {
        cachedColliders.Clear();
        cachedRenderers.Clear();

        cachedColliders.AddRange(GetComponentsInChildren<Collider>(true));
        cachedRenderers.AddRange(GetComponentsInChildren<Renderer>(true));
    }

    private void SetNodeActive(bool active)
    {
        foreach (var col in cachedColliders)
        {
            if (col != null)
                col.enabled = active;
        }

        foreach (var rend in cachedRenderers)
        {
            if (rend != null)
                rend.enabled = active;
        }
    }
}
