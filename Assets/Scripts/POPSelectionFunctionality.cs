using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class POPSelectionFunctionality : MonoBehaviour, IPointerClickHandler
{
    internal Action<POPSelectionFunctionality> onClick;
   [SerializeField]
    GameObject _backGround;
    bool _isSelected;
    string _name;
    bool _hasBeenClicked;

    // public InventoryNames InventoryNames;
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
        if (_hasBeenClicked) return;
        onClick?.Invoke(this);
        _hasBeenClicked = true;
    }
}
