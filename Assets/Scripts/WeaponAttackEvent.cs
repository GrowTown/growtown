using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponAttackEvent : MonoBehaviour,IPointerClickHandler
{
  internal Action<WeaponAttackEvent> onClick;
    [SerializeField] private GameObject _backGround; // Background image
    [SerializeField] private GameObject hammer; // Hammer GameObject to activate/deactivate
   // [SerializeField] private Animator hammerAnimator;

    private bool _isSelected;
    private bool _hasBeenClicked;
    private bool isHammerActive;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            _backGround.SetActive(value); // Show/hide background based on selection
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Toggle hammer and selection state
        isHammerActive = !isHammerActive;
        hammer.SetActive(isHammerActive);
        IsSelected = isHammerActive;

        // Invoke the click event if needed for other listeners
        onClick?.Invoke(this);
    }


    private void Update()
    {
        // Check for left mouse button click when hammer is active
        if (isHammerActive && Input.GetMouseButtonDown(0))
        {
            PlayHammerAttackAnimation();
        }
         else
        {
            UI_Manager.Instance.CharacterMovements.animator.SetBool("Attack", false);
        }
    }

    private void PlayHammerAttackAnimation()
    {
        if (UI_Manager.Instance.CharacterMovements.animator != null)
        {
            UI_Manager.Instance.CharacterMovements.animator.SetBool("Attack",true); // Trigger the attack animation
        }
        
    }
}


