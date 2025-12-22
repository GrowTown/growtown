using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Tracks how many tomatoes the player has harvested and shows a popup once the goal is met.
/// </summary>
public class TomatoHarvestGoal : MonoBehaviour
{
    [Header("Goal")]
    [SerializeField] private int targetTomatoCount = 25;

    [Header("Popup UI")]
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private TMP_Text popupLabel;
    [SerializeField] private string popupMessage = "Trade with the farmer";
    [SerializeField] private bool hidePopupOnStart = true;

    [Header("Events")]
    public UnityEvent onGoalReached;

    private int harvestedTomatoes = 0;
    private bool goalReached = false;

    private void Start()
    {
        if (hidePopupOnStart)
            SetPopupActive(false);
    }

    public void RegisterTomatoHarvest(int amount = 1)
    {
        if (goalReached)
            return;

        harvestedTomatoes += Mathf.Max(0, amount);

        if (harvestedTomatoes >= targetTomatoCount)
        {
            goalReached = true;
            ShowPopup();
            onGoalReached?.Invoke();
            Debug.Log($"TomatoHarvestGoal: Reached {harvestedTomatoes} tomatoes. Prompting player to trade.");
        }
    }

    private void ShowPopup()
    {
        if (popupLabel != null)
            popupLabel.text = popupMessage;

        SetPopupActive(true);
    }

    private void SetPopupActive(bool value)
    {
        if (popupRoot != null)
            popupRoot.SetActive(value);
    }
}
