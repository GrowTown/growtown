using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class POPSelectionFunctionality : MonoBehaviour, IPointerClickHandler
{
 /*   internal Action<POPSelectionFunctionality> onClick;
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
    }*/

    internal Action<POPSelectionFunctionality> onClick;
    [SerializeField] private GameObject _backGround;
    private bool _isSelected;
    private string _name;
    bool _hasBeenClicked;

    /// <summary>
    /// Property to manage the background's active state
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            _backGround.SetActive(value);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (UI_Manager.Instance.currentIndex == 3)
        {
            if (!UI_Manager.Instance.WeaponAttackEvent.isHammerActive&& UI_Manager.Instance.isPlantGrowthCompleted)
            {
                Check();
            }
        }
        else
        {
            Check();
        }

    }

    void Check()
    {
        if (_hasBeenClicked) return;
        onClick?.Invoke(this);
        _hasBeenClicked = true;
    }
}