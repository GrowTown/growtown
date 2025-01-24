using UnityEngine;
using UnityEngine.EventSystems;

public class ShowPopUpForPasticide : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    GameObject PopUpPanel;
    bool clicked = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        ShowPopUp();
    }

    void ShowPopUp()
    {
        if (!clicked)
        {
            PopUpPanel.SetActive(true);
            this.gameObject.GetComponent<LandHealth>().ShowPasticidePop();
            clicked = true;
        }
        else
        {
            PopUpPanel.SetActive(false);
            clicked = false;
        }
    }

}
