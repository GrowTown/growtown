using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{

    [Header("GameObjects")]
    public GameObject marketPopUp;
    public GameObject inventoryPanel;
    public GameObject[] PopupImg;
    public GameObject sickleWeapon;
    public GameObject timerPanel;
    public GameObject seed;
    public GameObject seedSpawnPoint;
    public GameObject seedsBag;
    public GameObject tileSeedSpawnPoint;
    public GameObject plantHolder;
    public GameObject cleaningTool;
    public GameObject tomato;
    public GameObject wateringTool;
    public GameObject starterPackInfoPopUpPanel;
    public GameObject sellPopupPanel;


    [Header("Buttons")]
    public Button wheatSeedBT;
    public Button carrotsSeedBT;
    public Button strawberriesSeedBT;
    public Button cleaningWeaponBT;
    public Button wateringWeaponBT;
    public Button sickleWeaponBT;
    public Button buyInventoryBT;
    public Button sellInventoryBT;

    [Header("Text")]
    public TextMeshProUGUI score;
    public TextMeshProUGUI notEnoughMoneyText;

    [Header("References")]

    [SerializeField]
    private TriggerZoneCallBacks _triggerCallBacks;
    [SerializeField]
    private ShopManager _shopManager;
    [SerializeField]
    private CharacterMovements _characterMovements;
    [SerializeField]
    private FieldGrid _fieldGrid;
    [SerializeField]
    private FieldManager _fieldManager;

    internal int oldcurrentStep = -1;
    InventoryNames[] inventoryNames;
    public bool isPlanted;
    public bool waveStarted;
    #region Fields
    internal int scoreIn = 100;
    #endregion

    #region Properties
    public FieldManager FieldManager
    {
        get => _fieldManager;
        set => _fieldManager = value;
    }
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

    public CharacterMovements CharacterMovements
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
        starterPackInfoPopUpPanel.SetActive(true);
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
            var item = inventoryPanel.transform.GetChild(i).gameObject.GetComponent<SelectionFunctionality>();
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
        TriggerZoneCallBacks.onPlayerEnter += (a) =>
        {
            if (!GameManager.Instance.isHarvestCompleted)
            {
                marketPopUp.SetActive(true);
            }
            else
            {
                sellPopupPanel.SetActive(true);
            }

        };


        TriggerZoneCallBacks.onPlayerExit += (e) => 
        {
            marketPopUp.SetActive(false);
            sellPopupPanel.SetActive(false);
        };

        buyInventoryBT.onClick.AddListener(() =>
        {
            marketPopUp.SetActive(true);
            sellPopupPanel.SetActive(false);
        });
        sellInventoryBT.onClick.AddListener(() => 
        {
            scoreIn += 12;
            score.text = scoreIn.ToString();
            sellPopupPanel.SetActive(false);
            GameManager.Instance.isHarvestCompleted = false;
        });

        carrotsSeedBT.onClick.AddListener(() => { ShopManager.ToBuyCarrots(); });
        wheatSeedBT.onClick.AddListener(() => { ShopManager.ToBuyWheat(); });
        strawberriesSeedBT.onClick.AddListener(() => { ShopManager.ToBuyStrawberries(); });
        cleaningWeaponBT.onClick.AddListener(() =>
        {
            if (!ShopManager.isCleningToolBought)
            {
                ShopManager.ToBuyCleaningTool();
                cleaningWeaponBT.interactable = false;
            }

        });
        wateringWeaponBT.onClick.AddListener(() =>
        {
            if (!ShopManager.isWateringToolBought)
            {
                ShopManager.ToBuyWateringTool();
                wateringWeaponBT.interactable = false;
            }
        });
        sickleWeaponBT.onClick.AddListener(() =>
        {
            if (!ShopManager.isCuttingToolBought)
            {
                ShopManager.ToBuyCuttingTool();
                sickleWeaponBT.interactable = false;
            }
        });

    }

    POPSelectionFunctionality currentSelectedPopUp;
    internal int oldcurrentAction = -1;
    public void ShowPopup(PlayerAction currentAction)
    {
        HideFieldPopup();  // Ensure all other popups are hidden first
        int popupIndex = (int)currentAction;

        if (popupIndex >= 0 && popupIndex < PopupImg.Length)
        {
            var popup = PopupImg[popupIndex];
            popup.SetActive(true);
            var selectionFunctionality = popup.GetComponent<POPSelectionFunctionality>();
            selectionFunctionality.onClick = null;
            if (oldcurrentStep != -1 && UI_Manager.Instance.FieldManager.fieldSteps.ContainsKey(UI_Manager.Instance.TriggerZoneCallBacks.fieldID))
            {
                GameManager.Instance.StartPlayerAction(currentAction);
            }
            else
            {
                selectionFunctionality.onClick = (selected) =>
                {
                    oldcurrentAction = popupIndex;
                    selectionFunctionality.IsSelected = true;
                    GameManager.Instance.StartPlayerAction(currentAction);
                };

            }


        }
    }

    internal void HideFieldPopup()
    {
        if (TriggerZoneCallBacks.currentStep <= PopupImg.Length - 1)
        {
            PopupImg[TriggerZoneCallBacks.currentStep].SetActive(false);
        }
        sickleWeapon.SetActive(false);
        wateringTool.SetActive(false);
        GameManager.Instance.StopCurrentAnimations(); // Stop any active animations
    }




    #endregion

}
