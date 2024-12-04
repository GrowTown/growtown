using System.Collections.Generic;
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
    public Button tomatoSeedBT;
    public Button carrotsSeedBT;
    public Button strawberriesSeedBT;
    public Button cleaningWeaponBT;
    public Button wateringWeaponBT;
    public Button sickleWeaponBT;
    public Button buyInventoryBT;
    public Button sellInventoryBT;
    public Button starterPackBuyBT;
    public Button energyBuyBT;
    public Button waterBuyBT;

    [Header("Text")]
    public TextMeshProUGUI score;
    public TextMeshProUGUI notEnoughMoneyText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI waterText;

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

    [SerializeField]
    private WeaponAttackEvent _weaponAttackEvent;
    [SerializeField]
    private EnemyPool _enemyPool;

    internal int oldcurrentStep = -1;
    InventoryNames[] inventoryNames;
    public bool isPlanted;
    public bool waveStarted;
    public bool isPlantGrowthCompleted;
    public bool isPlayerInField=false;
    internal bool isTimerOn = false;
    internal bool isinitialgrowStarted = false;
    public int currentIndex;

    internal List<GameObject> spawnTomatosForGrowth = new List<GameObject>();
    internal List<GameObject> spawnPlantsForInitialGrowth = new List<GameObject>();
    internal List<GameObject> spawnedSeed= new List<GameObject>();
    internal Dictionary<GameObject,List<GameObject>> spawnPlantsForGrowth = new Dictionary<GameObject,List< GameObject>>();
    internal Dictionary<int,List<int>> ListOfHarvestCount = new Dictionary<int,List< int>>();
    internal List<GameObject> GrowthStartedPlants = new List<GameObject>();
    internal List<GameObject> GrowthStartedOnThisTile = new List<GameObject>();
    internal List<GameObject> GrownPlantsToCut = new List<GameObject>();
    [SerializeField]internal List<ShopItem> shopItems=new List<ShopItem>();

    #region Fields
    internal int scoreIn = 100;
    #endregion

    #region Properties

    public EnemyPool EnemyPool
    {
        get => _enemyPool;
        set => _enemyPool = value;
    }
    public WeaponAttackEvent WeaponAttackEvent
    {
        get => _weaponAttackEvent;
        set => _weaponAttackEvent = value;
    }
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
        GameManager.Instance.CurrentEnergyCount = 500;
        GameManager.Instance.CurrentWaterCount = 500;
        energyText.text= GameManager.Instance.CurrentEnergyCount.ToString();
        waterText.text=GameManager.Instance.CurrentWaterCount.ToString();
        CallBackEvents();
    }

    // Update is called once per frame
    void Update()
    {

        InventorySetUp();
        if(GameManager.Instance.CurrentEnergyCount < 100)
        {
            energyBuyBT.interactable=true;
        }
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
    public bool seedBought;
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
            foreach(var item in UI_Manager.Instance.ListOfHarvestCount)
            {
                GameManager.Instance.CounttheHarvest(item.Value.Count);
            }
            
            sellPopupPanel.SetActive(false);
            GameManager.Instance.isHarvestCompleted = false;
        });

        carrotsSeedBT.onClick.AddListener(() => {
            ShopManager.ToBuyWheat();
          });
        tomatoSeedBT.onClick.AddListener(() => {
            ShopManager.ToBuyTomato();
            seedBought=true;
        });
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
        starterPackBuyBT.onClick.AddListener(() => { GameManager.Instance.StartPackToBuy(); });
        energyBuyBT.onClick.AddListener(() => { GameManager.Instance.ToBuyEnergyPoints(); });
        waterBuyBT.onClick.AddListener(() => { GameManager.Instance.ToBuyWaterPoints(); });

    }

    POPSelectionFunctionality currentSelectedPopUp;
    internal int oldcurrentAction = -1;
    internal bool isWentInsideOnce;
    public void ShowPopup(PlayerAction currentAction)
    {
        HideFieldPopup();  
        int popupIndex = (int)currentAction;
        
        if (popupIndex >= 0 && popupIndex < PopupImg.Length)
        {
            var popup = PopupImg[popupIndex];
            popup.SetActive(true);
            var selectionFunctionality = popup.GetComponent<POPSelectionFunctionality>();
            currentIndex = popupIndex;
            selectionFunctionality.onClick = null;
            if (popupIndex == 2&&!isWentInsideOnce)
            {
                GameManager.Instance.BeforeWaterTile();
                isWentInsideOnce = true;
                
            }
            if (oldcurrentStep != -1 && UI_Manager.Instance.FieldManager.fieldSteps.ContainsKey(UI_Manager.Instance.FieldManager.CurrentFieldID)&&!GameManager.Instance.isOneWorkingActionCompleted)
            {
                GameManager.Instance.StartPlayerAction(currentAction);
            }
            else
            {
                selectionFunctionality.onClick = (selected) =>
                {
                    if (popupIndex == 3)
                    {
                        if (isPlantGrowthCompleted)
                        {
                            if (!WeaponAttackEvent.isHammerActive)
                            {
                                if (currentSelectedPopUp != null)
                                {
                                    currentSelectedPopUp.IsSelected = false;
                                }
                                oldcurrentAction = popupIndex;
                                selectionFunctionality.IsSelected = true;
                                currentSelectedPopUp = selectionFunctionality;
                                GameManager.Instance.StartPlayerAction(currentAction);
                                GameManager.Instance.isOneWorkingActionCompleted = false;
                                UI_Manager.Instance.oldcurrentStep = popupIndex;
                            }

                        }
                    }
                    else
                    {
                        if (!WeaponAttackEvent.isHammerActive)
                        {
                            if (currentSelectedPopUp != null)
                            {
                                currentSelectedPopUp.IsSelected = false;
                            }
                            oldcurrentAction = popupIndex;
                            selectionFunctionality.IsSelected = true;
                            currentSelectedPopUp = selectionFunctionality;
                            GameManager.Instance.StartPlayerAction(currentAction);
                            GameManager.Instance.isOneWorkingActionCompleted = false;
                            UI_Manager.Instance.oldcurrentStep = popupIndex;
                        }
                    }
                   
                 
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
        cleaningTool.SetActive(false);
        seedsBag.gameObject.SetActive(false);
        GameManager.Instance.StopCurrentAnimations(); // Stop any active animations
    }

  
    #endregion

}
