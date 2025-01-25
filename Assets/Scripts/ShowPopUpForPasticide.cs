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
            UI_Manager.Instance.pasticidePopUpPanel.SetActive(true);
            this.gameObject.GetComponent<LandHealth>().ShowPasticidePop();
            clicked = true;
        }
        else
        {
            UI_Manager.Instance.pasticidePopUpPanel.SetActive(false);
            clicked = false;
        }
    }

}
