using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionFunctionality : MonoBehaviour, IPointerClickHandler
{
    internal Action<SelectionFunctionality> OnClick;
    public TextMeshProUGUI productCount;
    [SerializeField]
    GameObject _backGround;
    bool _isSelected;

    public InventoryNames InventoryNames;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (value == true)
            {
                _isSelected = value;
                _backGround.SetActive(true);
            }
            else
            {
                _isSelected = false;
                _backGround.SetActive(false);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick.Invoke(this);
    }
}
public enum InventoryNames
{
    Wheat,
    Carrots,
    Strawberries,
    Energy,
    Water,

}
