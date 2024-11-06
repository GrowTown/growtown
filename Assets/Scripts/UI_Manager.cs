using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UI_Manager : MonoBehaviour
{
  
    [Header("Panels")]
    public GameObject marketPopUp;
    public GameObject inventoryPanel;
    public GameObject[] PopupImg;
    public GameObject sickleWeapon;
    public GameObject timerPanel;
    

    [Header("Buttons")]
    public Button wheatSeedBT;
    public Button carrotsSeedBT;
    public Button strawberriesSeedBT;

    [Header("Text")]
    public TextMeshProUGUI score;

    [Header("References")]

    [SerializeField]
    private TriggerZoneCallBacks _triggerCallBacks;
    [SerializeField]
    private ShopManager _shopManager;
    [SerializeField]
    private CharacterMovements _characterMovements;
    [SerializeField]
    private FieldGrid _fieldGrid;

    InventoryNames[] inventoryNames;
    #region Fields
    internal int scoreIn = 100;
    #endregion

    #region Properties
    public ShopManager ShopManager
    {
        get => _shopManager;
        set => _shopManager = value;
    }

    public TriggerZoneCallBacks TriggerZoneCallBacks
    {
        get => _triggerCallBacks;
        set => _triggerCallBacks = value;
    }

   public  CharacterMovements CharacterMovements
    {
        get => _characterMovements;
        set => _characterMovements = value;
    }

    public FieldGrid FieldGrid
    {
        get => _fieldGrid;
        set => _fieldGrid = value;
    }

    #endregion
    public static UI_Manager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        score.text = scoreIn.ToString();
        CallBackEvents();
    }

    // Update is called once per frame
    void Update()
    {
       
        InventorySetUp();
    }

    #region Functions

    SelectionFunctionality currentSelected;
    void InventorySetUp()
    {
        for (int i = 0; i < inventoryPanel.transform.childCount; i++)
        { 
            var item=inventoryPanel.transform.GetChild(i).gameObject.GetComponent<SelectionFunctionality>();
            item.onClick += (i) =>
            {
                if (currentSelected != null)
                {
                    currentSelected.IsSelected = false;
                }
                    
                item.IsSelected = true;
                currentSelected = item;
            };
        }
    }

    void CallBackEvents()
    {
        TriggerZoneCallBacks.onPlayerEnter+=(a)=>marketPopUp.SetActive(true);
        TriggerZoneCallBacks.onPlayerExit+=(e)=>marketPopUp.SetActive(false);
        
        carrotsSeedBT.onClick.AddListener(() => { _shopManager.ToBuyCarrots(); });
        wheatSeedBT.onClick.AddListener(() => { _shopManager.ToBuyWheat(); });
        strawberriesSeedBT.onClick.AddListener(() => { _shopManager.ToBuyStrawberries(); });

    }

    /*internal void HideFieldPopup()
    {
        // Hide the current popup image
        if (TriggerZoneCallBacks.currentStep <= PopupImg.Length - 1)
        {
           PopupImg[TriggerZoneCallBacks.currentStep].SetActive(false);
        }
        sickleWeapon.SetActive(false);
        CharacterMovements.animator.SetLayerWeight(2, 0);
        CharacterMovements.animator.SetLayerWeight(1, 0);

    }

    /// <summary>
    /// Show the popup based on the current action
    /// </summary>
    public void ShowPopup(PlayerAction currentAction)
    {
        HideFieldPopup();  // Ensure all other popups are hidden first

        int popupIndex = (int)currentAction;
        if (popupIndex >= 0 && popupIndex < PopupImg.Length)
        {
            var popup = PopupImg[popupIndex];
            popup.SetActive(true);

            // Set the onClick action for this popup's SelectionFunctionality
            var selectionFunctionality = popup.GetComponent<SelectionFunctionality>();
            if (selectionFunctionality != null)
            {
                selectionFunctionality.onClick = (selected) =>
                {
                    GameManager.Instance.StartPlayerAction(currentAction);
                };
            }
        }
    }
*/


    public void ShowPopup(PlayerAction currentAction)
    {
        HideFieldPopup();  // Ensure all other popups are hidden first
        int popupIndex = (int)currentAction;

        if (popupIndex >= 0 && popupIndex < PopupImg.Length)
        {
            var popup = PopupImg[popupIndex];
            popup.SetActive(true);
            var selectionFunctionality = popup.GetComponent<SelectionFunctionality>();
            selectionFunctionality.onClick = null;
            selectionFunctionality.onClick = (selected) =>
            {
                GameManager.Instance.StartPlayerAction(currentAction);
            };
        }
    }

    internal void HideFieldPopup()
    {
        if (_triggerCallBacks.currentStep <= PopupImg.Length - 1)
        {
            PopupImg[_triggerCallBacks.currentStep].SetActive(false);
        }
        sickleWeapon.SetActive(false);
        StopCurrentAction(); // Stop any active animations
    }

    public void StartActionAnimation(PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.Clean:
                // _characterMovements.animator.SetLayerWeight(1, 1);
                Debug.Log("Cleanig");
                break;
            case PlayerAction.Seed:
                //_characterMovements.animator.SetLayerWeight(2, 1);
                Debug.Log("Seeding");
                break;
            case PlayerAction.Water:
               _characterMovements.animator.SetLayerWeight(1, 1);
                break; 
            case PlayerAction.Harvest:
                _characterMovements.animator.SetLayerWeight(2, 1);
                sickleWeapon.SetActive(true) ;
                break;
        }
    }

    public void StopCurrentAction()
    {
        _characterMovements.animator.SetLayerWeight(1, 0);
        _characterMovements.animator.SetLayerWeight(2, 0);
       // _characterMovements.animator.SetLayerWeight(3, 0);
      //  _characterMovements.animator.SetLayerWeight(4, 0);
    }


    #endregion

}
