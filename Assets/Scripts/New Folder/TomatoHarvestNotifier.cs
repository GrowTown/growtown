using UnityEngine;

/// <summary>
/// Attach to tomato GatherableResource objects to report harvests to TomatoHarvestGoal.
/// </summary>
[RequireComponent(typeof(GatherableResource))]
public class TomatoHarvestNotifier : MonoBehaviour
{
    [SerializeField] private TomatoHarvestGoal harvestGoal;
    [SerializeField] private int tomatoesAwarded = 1;

    private GatherableResource gatherable;

    private void Awake()
    {
        gatherable = GetComponent<GatherableResource>();
    }

    private void OnEnable()
    {
        if (gatherable != null && gatherable.onHarvested != null)
            gatherable.onHarvested.AddListener(OnHarvested);
    }

    private void OnDisable()
    {
        if (gatherable != null && gatherable.onHarvested != null)
            gatherable.onHarvested.RemoveListener(OnHarvested);
    }

    private void OnHarvested()
    {
        if (harvestGoal == null)
        {
            Debug.LogWarning($"{nameof(TomatoHarvestNotifier)} on {name} is missing the TomatoHarvestGoal reference.");
            return;
        }

        harvestGoal.RegisterTomatoHarvest(tomatoesAwarded);
    }
}
