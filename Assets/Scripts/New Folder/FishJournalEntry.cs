using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishJournalEntry : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private CanvasGroup canvasGroup;

    private Fish boundFish;
    private float uncaughtAlpha = 0.45f;

    // ==============================
    // Initial setup when spawning
    // ==============================
    public void Setup(Fish fish, float alphaWhenUncaught)
    {
        boundFish = fish;
        uncaughtAlpha = alphaWhenUncaught;

        if (nameText != null) nameText.text = fish.fishName;
        if (rarityText != null) rarityText.text = fish.rarity.ToString();
        if (valueText != null) valueText.text = $"{fish.value} Coins";
        if (icon != null) icon.sprite = fish.icon;

        Refresh(fish.caught, uncaughtAlpha);
    }

    // ==============================
    // Full refresh (preferred)
    // ==============================
    public void Refresh(bool isCaught, float alphaWhenUncaught)
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = isCaught ? 1f : alphaWhenUncaught;
        canvasGroup.interactable = isCaught;
        canvasGroup.blocksRaycasts = isCaught;
    }

    // ==============================
    // Fallback refresh (no params)
    // Uses boundFish data
    // ==============================
    public void Refresh()
    {
        if (boundFish == null) return;
        Refresh(boundFish.caught, uncaughtAlpha);
    }
}
