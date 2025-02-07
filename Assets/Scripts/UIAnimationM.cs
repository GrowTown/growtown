using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimationM : MonoBehaviour
{

    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private GameObject movingItemPrefab;

    public void PlayMoveToInventoryAnimation(Sprite itemSprite, RectTransform startRectTransform, RectTransform targetUIPosition)
    {
        GameObject movingItem = Instantiate(movingItemPrefab, mainCanvas.transform);
        Image itemImage = movingItem.transform.GetChild(0).GetComponent<Image>();
        RectTransform movingItemRect = movingItem.GetComponent<RectTransform>();

        itemImage.sprite = itemSprite;
        movingItemRect.position = startRectTransform.position;

        movingItemRect.DOMove(targetUIPosition.position, 0.8f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => Destroy(movingItem));
    }


}
