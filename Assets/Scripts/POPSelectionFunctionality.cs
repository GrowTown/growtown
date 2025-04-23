using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class POPSelectionFunctionality : MonoBehaviour, IPointerClickHandler
{

    internal Action<POPSelectionFunctionality> onClick;
    [SerializeField] private GameObject _backGround;
    private bool _isSelected;
    private string _name;
    internal bool _hasBeenClicked;

    internal FieldGrid fieldGrid;

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
            if (!UI_Manager.Instance.WeaponAttackEvent.isGunActive && fieldGrid.isPlantGrowthCompleted || fieldGrid.isAllPlantsWithered)
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
        AudioManager.Instance.PlaySFX();
        if (_hasBeenClicked) return;
        onClick?.Invoke(this);
        _hasBeenClicked = true;
    }
}