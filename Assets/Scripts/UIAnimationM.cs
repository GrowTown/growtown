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

    public void PlayMoveToUIAnimation(Sprite itemSprite, RectTransform startTransform, RectTransform targetUIPosition, int count)
    {
        Canvas mainCanvasComponent = mainCanvas.GetComponent<Canvas>();
        RectTransform mainCanvasRect = mainCanvas.GetComponent<RectTransform>();

        for (int i = 0; i < count; i++)
        {
            GameObject movingItem = Instantiate(movingItemPrefab, mainCanvas.transform);
            Image itemImage = movingItem.transform.GetChild(0).GetComponent<Image>();
            RectTransform movingItemRect = movingItem.GetComponent<RectTransform>();

            itemImage.sprite = itemSprite;

            // Set initial position inside UI Canvas
            movingItemRect.position = startTransform.position;
            Vector2 startOffset = new Vector2(Random.Range(-20f, 20f), Random.Range(-10f, 10f));
            movingItemRect.anchoredPosition += startOffset;
            // Calculate a curved path (Bezier-like motion)
            Vector3 midPoint = (startTransform.position + targetUIPosition.position) / 2;
            midPoint.y += Random.Range(80f, 120f);
            /*midPoint.y += Random.Range(50f, 100f); // Add height for arc effect
            midPoint.x += Random.Range(-30f, 30f); // Add variation in x-axis
*/
            Vector3[] path = new Vector3[] { startTransform.position, midPoint, targetUIPosition.position };

            // Animate using a curved path
            movingItemRect.DOPath(path, 0.8f, PathType.CatmullRom).SetEase(Ease.InOutQuad).SetDelay(i * 0.1f) .OnComplete(() => Destroy(movingItem));
        }
    }


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

            Vector3 midPoint = (screenPos + targetUIPosition.position) / 2;
            midPoint.y += Random.Range(80f, 120f);
            /*midPoint.y += Random.Range(50f, 100f); // Add height for arc effect
            midPoint.x += Random.Range(-30f, 30f); // Add variation in x-axis
*/
            Vector3[] path = new Vector3[] { screenPos, midPoint, targetUIPosition.position };
            Vector2 randomOffset = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));

            // Move the UI element
            //movingItemRect.DOMove(targetUIPosition.position, 0.8f).SetEase(Ease.InOutQuad).SetDelay(i * 0.1f) .OnComplete(() => Destroy(movingItem));
            movingItemRect.DOPath(path, 0.8f, PathType.CatmullRom).SetEase(Ease.InOutQuad).SetDelay(i * 0.1f).OnComplete(() => Destroy(movingItem));
        }
    }

  /*  public void PlayMoveToUIAnimation(Sprite itemSprite, RectTransform startTransform, RectTransform targetUIPosition, int count)
    {
        Canvas mainCanvasComponent = mainCanvas.GetComponent<Canvas>();
        RectTransform mainCanvasRect = mainCanvas.GetComponent<RectTransform>();

        for (int i = 0; i < count; i++)
        {
            GameObject movingItem = Instantiate(movingItemPrefab, mainCanvas.transform);
            Image itemImage = movingItem.transform.GetChild(0).GetComponent<Image>();
            RectTransform movingItemRect = movingItem.GetComponent<RectTransform>();
            itemImage.sprite = itemSprite;
            movingItemRect.position = startTransform.position;
            Vector2 randomOffset = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));

            // Move the UI element
            movingItemRect.DOMove(targetUIPosition.position, 0.8f).SetEase(Ease.InOutQuad).SetDelay(i * 0.1f).OnComplete(() => Destroy(movingItem));
        }
    }*/


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
