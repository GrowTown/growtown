using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PanelAnimation : MonoBehaviour
{
    private RectTransform prt;
    private RectTransform rt;
    private bool opened = false;

    private void Start()
    {
        prt = transform.parent.GetComponent<RectTransform>();
        rt = GetComponent<RectTransform>();

    }

    public void LandPropertiesBTClick()
    {
        float time = 0.2f;

        if (!opened)
        {
            prt.DOAnchorPosX(prt.anchoredPosition.x - rt.sizeDelta.x, time)
                .OnStart(() => gameObject.SetActive(true));
            opened = true;
        }
        else
        {

            prt.DOAnchorPosX(prt.anchoredPosition.x + rt.sizeDelta.x, time)
                .OnComplete(() => gameObject.SetActive(false));
            opened = false;
        }
    }
}



