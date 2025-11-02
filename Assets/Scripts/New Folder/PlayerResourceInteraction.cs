// ------------------------------------------------------------------------------
//  PlayerResourceInteraction.cs
//  Handles detecting nearby resource nodes, validating equipped tools, playing
//  animations, and rewarding the player for harvesting in GrowTown.
// ------------------------------------------------------------------------------

using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerResourceInteraction : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField, Min(0.5f)] private float interactionRadius = 2f;
    [SerializeField] private LayerMask resourceLayerMask = ~0;

    [Header("Timing")]
    [SerializeField, Range(0.1f, 2f)] private float harvestDelay = 0.65f;

    [Header("Rewards & Costs")]
    [SerializeField, Min(0)] private int energyCostPerHit = 3;
    [SerializeField, Min(0)] private int xpRewardPerHit = 3;

    [Header("References")]
    [SerializeField] private ToolManager toolManager;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip harvestClip;

    private bool isHarvesting;
    private Coroutine harvestRoutine;

    private void Awake()
    {
        if (toolManager == null)
            toolManager = GetComponent<ToolManager>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            TryStartHarvest();
    }

    private void TryStartHarvest()
    {
        if (isHarvesting)
            return;

        ResourceNode targetNode = FindClosestNode();
        if (targetNode == null)
            return;

        if (toolManager == null || !toolManager.HasToolFor(targetNode.ResourceType))
            return;

        if (!HasEnoughEnergy())
            return;

        harvestRoutine = StartCoroutine(HarvestRoutine(targetNode));
    }

    private ResourceNode FindClosestNode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius, resourceLayerMask, QueryTriggerInteraction.Collide);
        ResourceNode closest = null;
        float closestSqr = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit == null)
                continue;

            ResourceNode node = hit.GetComponent<ResourceNode>() ?? hit.GetComponentInParent<ResourceNode>();
            if (node == null || !node.IsAvailable)
                continue;

            float sqr = (node.transform.position - transform.position).sqrMagnitude;
            if (sqr < closestSqr)
            {
                closest = node;
                closestSqr = sqr;
            }
        }

        return closest;
    }

    private IEnumerator HarvestRoutine(ResourceNode node)
    {
        isHarvesting = true;

        string triggerName = GetAnimationTrigger(toolManager != null ? toolManager.CurrentTool : ToolType.None);
        if (!string.IsNullOrEmpty(triggerName) && animator != null)
            animator.SetTrigger(triggerName);

        yield return new WaitForSeconds(harvestDelay);

        if (node != null && node.IsAvailable)
        {
            if (node.Harvest(out ShopItemHolder reward))
            {
                SpendEnergy();
                GrantXp();

                if (inventoryManager != null && reward != null)
                    inventoryManager.AddToInventory(reward);

                PlayHarvestAudio();
            }
        }

        isHarvesting = false;
        harvestRoutine = null;
    }

    private bool HasEnoughEnergy()
    {
        if (GameManager.Instance == null)
            return true;

        return GameManager.Instance.CurrentEnergyCount >= energyCostPerHit;
    }

    private void SpendEnergy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.CurrentEnergyCount = Mathf.Max(0, GameManager.Instance.CurrentEnergyCount - energyCostPerHit);
    }

    private void GrantXp()
    {
        if (UI_Manager.Instance != null && UI_Manager.Instance.PlayerXp != null)
            UI_Manager.Instance.PlayerXp.SuperXp(xpRewardPerHit);
    }

    private void PlayHarvestAudio()
    {
        if (audioSource != null && harvestClip != null)
            audioSource.PlayOneShot(harvestClip);
    }

    private string GetAnimationTrigger(ToolType tool)
    {
        switch (tool)
        {
            case ToolType.Sickle:
                return "CutGrass";
            case ToolType.Pickaxe:
            case ToolType.Hammer:
                return "MineRock";
            case ToolType.Axe:
                return "ChopTree";
            default:
                return string.Empty;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
#endif
}
