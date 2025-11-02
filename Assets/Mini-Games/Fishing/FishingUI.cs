using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishingUI : MonoBehaviour
{
    public static FishingUI Instance;

    [Header("References")]
    public CanvasGroup canvasGroup;
    public Image tensionRing;
    public Image reelProgress;
    public TextMeshProUGUI hookPopup;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    public void SetTension(float value)
    {
        if (tensionRing) tensionRing.fillAmount = Mathf.Clamp01(value);
    }

    public void SetProgress(float value)
    {
        if (reelProgress) reelProgress.fillAmount = Mathf.Clamp01(value);
    }

    public void ShowHookPopup()
    {
        if (hookPopup)
        {
            hookPopup.gameObject.SetActive(true);
            CancelInvoke(nameof(HideHookPopup));
            Invoke(nameof(HideHookPopup), 1f);
        }
    }

    void HideHookPopup()
    {
        hookPopup.gameObject.SetActive(false);
    }
}
