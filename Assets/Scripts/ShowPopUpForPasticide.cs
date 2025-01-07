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
            UI_Manager.Instance.ShowPasticidePop();
            clicked = true;
        }
        else
        {
            PopUpPanel.SetActive(false);
            clicked = false;
        }
    }

}
