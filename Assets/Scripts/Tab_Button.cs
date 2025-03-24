using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;


public class Tab_Button : MonoBehaviour, IPointerClickHandler
{
    public TabGroup tabGroup;

    void Start()
    {
        //tabGroup.Subscribe(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
        AudioManager.Instance.PlaySFX();
    }

    public void ScaleButton(float targetScale, float duration)
    {
        this.GetComponent<RectTransform>().DOScale(targetScale, duration).SetEase(Ease.OutBack);
    }
}
