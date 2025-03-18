using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimationM : MonoBehaviour
{

    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas backgroundCanvas;   
    [SerializeField] private GameObject movingItemPrefab;

    public void PlayMoveToUIAnimation(Sprite itemSprite, RectTransform startRectTransform, RectTransform targetUIPosition)
    {
        GameObject movingItem = Instantiate(movingItemPrefab, mainCanvas.transform);
        Image itemImage = movingItem.transform.GetChild(0).GetComponent<Image>();
        RectTransform movingItemRect = movingItem.GetComponent<RectTransform>();

        itemImage.sprite = itemSprite;
        movingItemRect.position = startRectTransform.position;

        movingItemRect.DOMove(targetUIPosition.position, 0.8f).SetEase(Ease.InOutQuad).OnComplete(() => Destroy(movingItem));
    }

    /*  public void PlayMoveToUIAnimation(Sprite itemSprite, RectTransform startRectTransform, RectTransform targetUIPosition, int count)
      {
          Canvas canvas = mainCanvas.GetComponent<Canvas>();
          RectTransform canvasRectTransform = mainCanvas.GetComponent<RectTransform>();

          for (int i = 0; i < count; i++)
          {
              GameObject movingItem = Instantiate(movingItemPrefab, mainCanvas.transform);
              Image itemImage = movingItem.transform.GetChild(0).GetComponent<Image>();
              RectTransform movingItemRect = movingItem.GetComponent<RectTransform>();

              itemImage.sprite = itemSprite;
              movingItemRect.anchoredPosition = startRectTransform.anchoredPosition;
              // Add slight random offset for better effect
              Vector2 randomOffset = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));

              // Move the UI element
              movingItemRect.DOAnchorPos(targetUIPosition.anchoredPosition , 0.8f)
                  .SetEase(Ease.InOutQuad)
                  .SetDelay(i * 0.1f) // Delay effect for staggered animation
                  .OnComplete(() => Destroy(movingItem));
          }
      }*/

    public void PlayMoveToUIAnimation(Sprite itemSprite, Transform startTransform, RectTransform targetUIPosition, int count)
    {
        Canvas mainCanvasComponent = mainCanvas.GetComponent<Canvas>();
        RectTransform mainCanvasRect = mainCanvas.GetComponent<RectTransform>();

        for (int i = 0; i < count; i++)
        {
            GameObject movingItem = Instantiate(movingItemPrefab, mainCanvas.transform);
            Image itemImage = movingItem.transform.GetChild(0).GetComponent<Image>();
            RectTransform movingItemRect = movingItem.GetComponent<RectTransform>();
            itemImage.sprite = itemSprite;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(startTransform.position);
            movingItemRect.position = screenPos;
            Vector2 randomOffset = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));

            // Move the UI element
            movingItemRect.DOMove(targetUIPosition.position, 0.8f).SetEase(Ease.InOutQuad).SetDelay(i * 0.1f) .OnComplete(() => Destroy(movingItem));
        }
    }




    public void PlayMoveToUIAnimation(Sprite itemSprite, RectTransform startRectTransform, RectTransform targetUIPosition,int count,GameObject GO)
    {
        GameObject movingItem = Instantiate(movingItemPrefab, mainCanvas.transform);
        Image itemImage = movingItem.transform.GetChild(0).GetComponent<Image>();
        RectTransform movingItemRect = movingItem.GetComponent<RectTransform>();

        itemImage.sprite = itemSprite;
        movingItemRect.position = startRectTransform.position;

        movingItemRect.DOMove(targetUIPosition.position, 0.8f).SetEase(Ease.InOutQuad).OnComplete(() => Destroy(movingItem));
    }

}
