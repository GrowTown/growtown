using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponAttackEvent : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] private GameObject _backGround; // Background image
    [SerializeField] private GameObject hammer; // Hammer GameObject to activate/deactivate


    private bool _isSelected;
    internal bool isHammerActive;

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


    bool isMouseClick;
   

   

    private void Update()
    {
        // Check for left mouse button click when hammer is active
        if (isHammerActive &&Input.GetMouseButtonDown(1))
        {
            PlayHammerAttackAnimation();
        
        }
    }

    private void PlayHammerAttackAnimation()
    {
        if (UI_Manager.Instance.CharacterMovements.animator != null)
        {
            UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(1, 1); // Trigger the attack animation
            StartCoroutine(WaitForAnimationComplete());
        }
    }

    private IEnumerator WaitForAnimationComplete()
    {
        yield return new WaitForSeconds(20f); // Adjust based on animation length
        if (UI_Manager.Instance.CharacterMovements.animator != null)
        {
            UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(1, 0); // Reset layer weight after animation
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy"))
        {
            other.gameObject.SetActive(false);
        }
    }

}


