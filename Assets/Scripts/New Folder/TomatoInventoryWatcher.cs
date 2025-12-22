using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Watches the player's inventory and shows a notification when a tomato threshold is reached.
/// </summary>
public class TomatoInventoryWatcher : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private string tomatoItemName = "Tomato";
    [SerializeField] private int targetCount = 10;

    [Header("Popup UI")]
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private TMP_Text popupLabel;
    [SerializeField] private string popupMessage = "Trade with the farmer";
    [SerializeField] private bool hidePopupOnStart = true;

    [Header("Events")]
    public UnityEvent onThresholdReached;

    private bool hasNotified = false;

    private void Start()
    {
        if (hidePopupOnStart)
            SetPopupActive(false);
    }

    private void Update()
    {
        if (hasNotified || inventoryManager == null)
            return;

        int currentCount = GetTomatoCount();
        if (currentCount >= targetCount)
        {
            hasNotified = true;
            ShowPopup();
            onThresholdReached?.Invoke();
        }
    }

    private int GetTomatoCount()
    {
        if (inventoryManager == null || inventoryManager.inventoryItems == null)
            return 0;

        if (inventoryManager.inventoryItems.TryGetValue(tomatoItemName, out var item))
            return item.Itemcount;

        return 0;
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
