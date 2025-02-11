using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponAttackEvent : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] private GameObject _backGround;
    [SerializeField] private GameObject hammer;


    private bool _isSelected;
    internal bool isHammerActive;

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
        if (!UI_Manager.Instance.isPlayerInField&& !UI_Manager.Instance.starterPackInfoPopUpPanel.activeSelf)
        {
            // Toggle hammer and selection state
            isHammerActive = !isHammerActive;
            hammer.SetActive(isHammerActive);
            IsSelected = isHammerActive;
        }

    }

    internal void ToMakeHammerInactive()
    {
        isHammerActive=false;
        hammer.SetActive(isHammerActive);
        IsSelected = isHammerActive;
    }

    private void Update()
    {
        if (isHammerActive)
        {
            PlayHammerAttackAnimation();
        }
    }

    private void PlayHammerAttackAnimation()
    {
        if (UI_Manager.Instance.CharacterMovements.animator != null)
        {
            UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(1, 1);
            
        }
    }

    private void WaitForAnimationComplete()
    {
        if (UI_Manager.Instance.CharacterMovements.animator != null)
        {
            UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(1, 0); 
        }
    }

 
}


