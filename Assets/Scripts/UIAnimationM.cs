using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class UIAnimationM : MonoBehaviour
{

    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas backgroundCanvas;
    [SerializeField] private GameObject movingItemPrefab;
    [SerializeField] private GameObject particleEffectPrefab;
    [SerializeField] private Camera uICam;
    [SerializeField] private Material material;
    [SerializeField] private Transform pParent;

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
            movingItemRect.DOPath(path, 0.8f, PathType.CatmullRom).SetEase(Ease.InOutQuad).SetDelay(i * 0.1f).OnComplete(() =>
            {
                Destroy(movingItem);

                if (particleEffect == null)
                {
                    GameObject atvparticleEffect = Instantiate(particleEffectPrefab, targetUIPosition.position, Quaternion.identity);

                    particleEffect = atvparticleEffect;

                    ParticleSystemRenderer particleRenderer = particleEffect.GetComponent<ParticleSystemRenderer>();
                    if (particleRenderer != null)
                    {
                        Material particleMaterial = particleRenderer.material;
                        if (particleMaterial != null)
                        {
                            particleMaterial.mainTexture = itemSprite.texture;
                        }
                    }
                }

                Destroy(particleEffect, 0.5f);
                particleEffect = null;
            });
        }
    }

    GameObject particleEffect;

    /*  public void PlayMoveToUIAnimation(Sprite itemSprite, Transform startTransform, RectTransform targetUIPosition, int count)
      {
          Canvas mainCanvasComponent = mainCanvas.GetComponent<Canvas>();
          RectTransform mainCanvasRect = mainCanvas.GetComponent<RectTransform>();

          for (int i = 0; i < count; i++)
          {
              GameObject movingItem = Instantiate(movingItemPrefab, mainCanvas.transform);
              Image itemImage = movingItem.transform.GetChild(0).GetComponent<Image>();
              RectTransform movingItemRect = movingItem.GetComponent<RectTransform>();
              itemImage.sprite = itemSprite;

              //  Convert Player’s World Position to Viewport Space using Main Camera
              Vector3 screenPos = Camera.main.WorldToScreenPoint(startTransform.position);

              //  Convert Viewport Position to Screen Space using the UI Camera
              Vector3 screenPos = uICam.ViewportToScreenPoint(viewportPos);

              movingItemRect.position = screenPos;
              //  Convert Screen Space to Local UI Space
              Vector2 localStartPos;
              if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRect, screenPos, uICam, out localStartPos))
              {
                  //movingItemRect.position = localStartPos; // Set correct start position
              }

              // Convert target position to local UI space
              Vector2 localTargetPos;
              RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRect, targetUIPosition.position, uICam, out localTargetPos);

              // Midpoint for smooth animation
              Vector2 midPoint = (screenPos + targetUIPosition.position) / 2;
              midPoint.y += Random.Range(80f, 120f);

              Vector3[] path = new Vector3[] { screenPos, midPoint, targetUIPosition.position };

              // Animate using DoTween
              movingItemRect.DOPath(path, 0.8f, PathType.CatmullRom)
                  .SetEase(Ease.InOutQuad)
                  .SetDelay(i * 0.1f)
                  .OnComplete(() =>
                  {
                      Destroy(movingItem);

                      // Instantiate particle effect at the target position
                      if (particleEffect == null)
                      {
                          GameObject atvparticleEffect = Instantiate(particleEffectPrefab, targetUIPosition.position, Quaternion.identity);
                          particleEffect = atvparticleEffect.transform.GetChild(0).gameObject;

                          // Assign the item sprite to the particle material texture
                          ParticleSystemRenderer particleRenderer = particleEffect.GetComponent<ParticleSystemRenderer>();
                          if (particleRenderer != null)
                          {
                              Material particleMaterial = particleRenderer.material;
                              if (particleMaterial != null)
                              {
                                  particleMaterial.mainTexture = itemSprite.texture;
                              }
                          }
                      }

                      // Destroy particle after some time
                      Destroy(particleEffect, 1.5f);
                      particleEffect = null;
                  });
          }
      }
  */


    /*  public void PlayMoveToUIAnimation(Sprite itemSprite, Transform startTransform, RectTransform targetUIPosition, int count)
      {
          Canvas mainCanvasComponent = mainCanvas.GetComponent<Canvas>();
          RectTransform mainCanvasRect = mainCanvas.GetComponent<RectTransform>();

          for (int i = 0; i < count; i++)
          {
              GameObject movingItem = Instantiate(movingItemPrefab, mainCanvas.transform);
              Image itemImage = movingItem.transform.GetChild(0).GetComponent<Image>();
              RectTransform movingItemRect = movingItem.GetComponent<RectTransform>();
              itemImage.sprite = itemSprite;

              // Convert world position to screen position
              Vector3 screenPos = Camera.main.WorldToScreenPoint(startTransform.position);

              // Convert screen position to UI local position (because Canvas is in Screen Space - Camera mode)
              Vector2 localStartPos;
              RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRect, screenPos, mainCanvasComponent.worldCamera, out localStartPos);

              movingItemRect.position = screenPos; // Set correct start position inside the canvas

              // Convert target UI position to local canvas position
              Vector2 localTargetPos;
              RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRect, targetUIPosition.position, mainCanvasComponent.worldCamera, out localTargetPos);

              // Midpoint for smooth animation
              Vector2 midPoint = (localStartPos + localTargetPos) / 2;
              midPoint.y += Random.Range(80f, 120f);

              Vector3[] path = new Vector3[] { localStartPos, midPoint, localTargetPos };

              // Animate using DoTween
              movingItemRect.DOPath(path, 0.8f, PathType.CatmullRom)
                  .SetEase(Ease.InOutQuad)
                  .SetDelay(i * 0.1f)
                  .OnComplete(() =>
                  {
                      Destroy(movingItem);

                      // Instantiate particle effect at the target position
                      if (particleEffect == null)
                      {
                          GameObject atvparticleEffect = Instantiate(particleEffectPrefab, targetUIPosition.position, Quaternion.identity);
                          particleEffect = atvparticleEffect;

                          // Assign the item sprite to the particle material texture
                         *//* ParticleSystemRenderer particleRenderer = particleEffect.GetComponent<ParticleSystemRenderer>();
                          if (particleRenderer != null)
                          {
                              Material particleMaterial = particleRenderer.material;
                              if (particleMaterial != null)
                              {
                                  particleMaterial.mainTexture = itemSprite.texture;
                              }
                          }*//*
                      }

                      // Destroy particle after some time
                      Destroy(particleEffect, 1.5f);
                      particleEffect = null;
                  });
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
            //Vector3 screenPos1=uICam.WorldToScreenPoint(screenPos);
            //movingItemRect.position = screenPos;
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRect, screenPos, uICam, out localPoint);
            movingItemRect.localPosition = localPoint;
            //Vector2 targetPos = targetUIPosition.anchoredPosition;
            Vector2 targetPos = mainCanvasRect.InverseTransformPoint(targetUIPosition.position);

            Vector2 localTargetPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvasRect, targetPos, uICam, out localTargetPoint);

            Vector3 midPoint = (localPoint + targetPos) / 2;
            //midPoint.y += Random.Range(80f, 120f);
           
            Vector3[] path = new Vector3[] { localPoint, midPoint, targetPos };
            //Vector2 randomOffset = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));

            //movingItemRect.DOMove(targetUIPosition.position, 0.8f).SetEase(Ease.InOutQuad).SetDelay(i * 0.1f) .OnComplete(() => Destroy(movingItem));
            //movingItemRect.DOPath(path, 0.8f, PathType.CatmullRom).SetEase(Ease.InOutQuad).SetDelay(i * 0.1f).OnComplete(() => Destroy(movingItem));

            movingItemRect.DOLocalPath(path, 1f, PathType.CatmullRom)
                 .SetEase(Ease.InOutQuad)
                 .SetDelay(i * 0.1f)
                 .OnComplete(() =>
                 {
                     Destroy(movingItem);

                     if (particleEffect == null)
                     {
                         GameObject atvparticleEffect = Instantiate(particleEffectPrefab, targetUIPosition.position, Quaternion.identity);

                         particleEffect = atvparticleEffect;

                         ParticleSystemRenderer particleRenderer = particleEffect.GetComponent<ParticleSystemRenderer>();
                         if (particleRenderer != null)
                         {
                             Material particleMaterial = particleRenderer.material;
                             if (particleMaterial != null)
                             {
                                 particleMaterial.mainTexture = itemSprite.texture;
                             }
                         }
                     }

                     Destroy(particleEffect, 0.5f);
                     particleEffect = null;
                 });
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


    public void PlayMoveToUIAnimation(Sprite itemSprite, RectTransform startRectTransform, RectTransform targetUIPosition, int count, GameObject GO)
    {
        GameObject movingItem = Instantiate(movingItemPrefab, mainCanvas.transform);
        Image itemImage = movingItem.transform.GetChild(0).GetComponent<Image>();
        RectTransform movingItemRect = movingItem.GetComponent<RectTransform>();

        itemImage.sprite = itemSprite;
        movingItemRect.position = startRectTransform.position;

        movingItemRect.DOMove(targetUIPosition.position, 0.8f).SetEase(Ease.InOutQuad).OnComplete(() => Destroy(movingItem));
    }

}
