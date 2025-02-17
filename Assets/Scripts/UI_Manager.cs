using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    public GameObject warningPopupPanelEnergy;
    public GameObject warningPopupPanelSeed;
    public GameObject contentOfPasticidePanel;
    public GameObject contentOfPasticedMsgPanel;
    public GameObject contentOfNotBuyPasticedMsgPanel;
    public GameObject contentOfPasticideNormalHealthPanel;
    public GameObject pasticidePopUpPanel;
    public GameObject warningPasticidePopUpPanel;
    public GameObject LandHealthBarImg;
    public GameObject lockImageForWheatLand;
    public GameObject lockImageForWheatSeed;
    public GameObject lockImageForCarrotLand;
    public GameObject lockImageForCarrotSeed;
    public GameObject lockImageForSuperXp;
    public GameObject wheatFieldArea;
    public GameObject carrotFieldArea;
    public GameObject shotGun;
    public GameObject shotGunSpawnPoint;
    public GameObject joystick;


    [Header("Transforms")]
    public Transform lhHolderTransform;

    [Header("Effects")]
    public GameObject waterEffect;
    public GameObject cleanigEffect;


    [Header("Buttons")]
    public Button tomatoSeedBT;
    public Button wheatSeedBT;
    public Button strawberriesSeedBT;
    public Button buyInventoryBT;
    public Button sellInventoryBT;
    public Button starterPackBuyBT;
    public Button pasticideUseBT;
    public Button pasticideNormalHealthUseBT;
    public Button pasticideNotBoughtBT;
    public Button pasticideNotBoughNormalHealthtBT;
    public Button pasticideBuyBT;
    public Button wheatlandBuyBT;
    public Button carrotlandBuyBT;
    internal Button cleaningWeaponBT;
    internal Button wateringWeaponBT;
    internal Button sickleWeaponBT;
    internal Button waterBuyBT;
    internal Button superXpBuyBT;
    internal Button energyBuyBT;


    [Header("Sliders")]
    public Slider waterSlider;
    public Slider energySlider;

    [Header("List")]
    public List<GameObject> inventoryTabsList;

    [Header("Text")]
    public TextMeshProUGUI score;
    public TextMeshProUGUI notEnoughMoneyText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI warningTextEnergy;
    public TextMeshProUGUI warningTextForSeed;
    public TextMeshProUGUI playerXpTxt;
    public TextMeshProUGUI pasticideCount;
    public TextMeshProUGUI contentOfNotBuyPasticedMsgTxt;
    public TextMeshProUGUI currentplayerLevelTxt;

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
    [SerializeField]
    private TriggerForStoppingTheRun _triggerForStoppingTheRun;
    [SerializeField]
    private PlayerXp _playerXP;
    [SerializeField]
    private LandHealth _landHealth;
    [SerializeField]
    private SliderControls _sliderControls;
    [SerializeField]
    private RewardsForLevel _rewardsForLevel;
    [SerializeField]
    private TabGroup _tabGroup;
    [SerializeField]
    private InventoryManager _inventoryManager;
    [SerializeField]
    private UIAnimationM _uIAnimationM;

    private PlayerLevel _playerLevel;

    internal int oldcurrentStep = -1;
    public int currentIndex;
    public bool isPlanted;
    public bool waveStarted;
    public bool isPlantGrowthCompleted;
    public bool isPlayerInField = false;
    internal bool isTimerOn = false;
    internal bool isinitialgrowStarted = false;
    internal bool isSuperXpEnable = false;
    internal bool isTomatoHealthBarspawn = false;
    internal bool isWheatHealthBarspawn = false;
    internal bool isCarrotHealthBarspawn = false;
    internal bool isButtonsInitialized = false;

    internal bool IsPlayerInSecondZone = false;
    internal List<GameObject> spawnTomatosForGrowth = new List<GameObject>();
    internal List<GameObject> spawnPlantsForInitialGrowth = new List<GameObject>();
    internal List<GameObject> spawnedSeed = new List<GameObject>();
    internal Dictionary<GameObject, List<GameObject>> spawnPlantsForGrowth = new Dictionary<GameObject, List<GameObject>>();
    internal Dictionary<List<GameObject>, GameObject> spawnPlantsAndTile = new Dictionary<List<GameObject>, GameObject>();
    internal Dictionary<int, List<int>> ListOfHarvestCount = new Dictionary<int, List<int>>();
    internal List<GameObject> GrowthStartedPlants = new List<GameObject>();
    internal List<GameObject> GrowthStartedOnThisTile = new List<GameObject>();
    internal Dictionary<int, List<GameObject>> GrownPlantsToCut = new Dictionary<int, List<GameObject>>();
    [SerializeField]
    internal List<ShopItem> shopItems = new List<ShopItem>();


    #region Fields
    internal int scoreIn = 500;
    #endregion

    #region Properties

    public UIAnimationM UIAnimationM
    {
        get => _uIAnimationM;
        set => _uIAnimationM = value;
    }
    public InventoryManager InventoryManager
    {
        get=>_inventoryManager; 
        set=>_inventoryManager = value;
    }
    public TabGroup TabGroup
    {
        get => _tabGroup;
        set => _tabGroup = value;
    }

    public RewardsForLevel RewardsForLevel
    {
        get => _rewardsForLevel;
        set => _rewardsForLevel = value;
    }
    public SliderControls SliderControls
    {
        get => _sliderControls;
        set => _sliderControls = value;
    }
    public PlayerLevel PlayerLevel
    {
        get
        {
            return _playerLevel = _characterMovements.gameObject.GetComponent<PlayerLevel>();
        }
        set => _playerLevel = value;
    }
    public LandHealth LandHealth
    {
        get => _landHealth;
        set => _landHealth = value;
    }
    public PlayerXp PlayerXp
    {
        get => _playerXP;
        set => _playerXP = value;
    }
    public TriggerForStoppingTheRun TriggerForStoppingTheRun
    {
        get => _triggerForStoppingTheRun;
        set => _triggerForStoppingTheRun = value;
    }
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
        //  score.text = scoreIn.ToString();
        GameManager.Instance.CurrentEnergyCount = 500;
        GameManager.Instance.CurrentWaterCount = 500;
        GameManager.Instance.CurrentScore = 500;
        energyText.text = GameManager.Instance.CurrentEnergyCount.ToString();
        waterText.text = GameManager.Instance.CurrentWaterCount.ToString();
        playerXpTxt.text = PlayerXp.CurrentPlayerXpPoints.ToString();
        CallBackEvents();

    }


    void Update()
    {

        InventorySetUp();
        MakeButtonsInteractable();
        if (currentPopupIndex >= 0)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (PopupImg[currentPopupIndex].activeSelf)
                    HandleSelection(currentPlayerAction, currentPopupIndex, currentSelectionFunctionality);
            }
        }
        HideShowPopUpNotEnoughPoints();
    }

    void MakeButtonsInteractable()
    {

            if (GameManager.Instance.CurrentEnergyCount == 500)
            {
                energyBuyBT.interactable = false;
            }
            else
            {
                energyBuyBT.interactable = true;
            }
            if (GameManager.Instance.CurrentWaterCount == 500)
            {
                waterBuyBT.interactable = false;
            }
            else
            {
                waterBuyBT.interactable = true;
            }
        if (isButtonsInitialized)
        {
            if (SliderControls.gameObject.activeSelf)
            {
                superXpBuyBT.interactable = false;
            }
            else
            {
                superXpBuyBT.interactable = true;
            }
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
            /* if (!GameManager.Instance.isHarvestCompleted)
             {
                 marketPopUp.SetActive(true);
             }
             else
             {
                 sellPopupPanel.SetActive(true);
             }*/
            marketPopUp.SetActive(true);
        };
        TriggerZoneCallBacks.onPlayerExit += (e) =>
        {
            marketPopUp.SetActive(false);
            sellPopupPanel.SetActive(false);
        };
        pasticideNotBoughtBT.onClick.AddListener(() =>
        {
            HideFieldPopup();
            GameManager.Instance.StopCurrentAction();
        });
        pasticideNormalHealthUseBT.onClick.AddListener(() => {
            GameManager.Instance.ToIncreaseLandHealthUsePasticide(UI_Manager.Instance.FieldManager.CurrentFieldID, 100);
            pasticidePopUpPanel.SetActive(false);
        });
        pasticideBuyBT.onClick.AddListener(() => { ShopManager.ToBuyPasticide(); });
        pasticideUseBT.onClick.AddListener(() =>
        {
            GameManager.Instance.ToIncreaseLandHealthUsePasticide(UI_Manager.Instance.FieldManager.CurrentFieldID,100);
            pasticidePopUpPanel.SetActive(false);
        });
        buyInventoryBT.onClick.AddListener(() =>
        {
            marketPopUp.SetActive(true);
            sellPopupPanel.SetActive(false);
        });
        sellInventoryBT.onClick.AddListener(() =>
        {
            // sellPopupPanel.SetActive(true);
            SellHarvest();
        });
        wheatSeedBT.onClick.AddListener(() =>
        {
            ShopManager.ToBuyWheat();
        });
        tomatoSeedBT.onClick.AddListener(() =>
        {
            ShopManager.ToBuyTomato();
            seedBought = true;
        });
        strawberriesSeedBT.onClick.AddListener(() => { ShopManager.ToBuyStrawberries(); });
        starterPackBuyBT.onClick.AddListener(() => { GameManager.Instance.StartPackToBuy(); });
      
        wheatlandBuyBT.onClick.AddListener(() =>
        {
            ShopManager.ToBuyWheatField();
        });
        carrotlandBuyBT.onClick.AddListener(() =>
        {
            ShopManager.ToBuyCarrotField();
        });
        
    }

    POPSelectionFunctionality currentSelectedPopUp;
    internal int oldcurrentAction = -1;
    internal bool isWentInsideOnce;
    private int currentPopupIndex = -1;
    private PlayerAction currentPlayerAction;
    private POPSelectionFunctionality currentSelectionFunctionality;
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
            currentPopupIndex = popupIndex;
            currentPlayerAction = currentAction;
            currentSelectionFunctionality = selectionFunctionality;
            selectionFunctionality.onClick = null;
            if (popupIndex == 2 && !isWentInsideOnce)
            {
                GameManager.Instance.BeforeWaterTile();
                isWentInsideOnce = true;

            }
            if (oldcurrentStep != -1 && FieldManager.fieldSteps.ContainsKey(FieldManager.CurrentFieldID) && !GameManager.Instance.isOneWorkingActionCompleted)
            {
                GameManager.Instance.StartPlayerAction(currentAction);
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (popup.activeSelf)
                        HandleSelection(currentAction, popupIndex, selectionFunctionality);
                }
                selectionFunctionality.onClick = (selected) =>
                {
                    HandleSelection(currentAction, popupIndex, selectionFunctionality);
                };
            }
        }
    }
    private void HandleSelection(PlayerAction currentAction, int popupIndex, POPSelectionFunctionality selectionFunctionality)
    {
        if (popupIndex == 3)
        {
            if (isPlantGrowthCompleted && !WeaponAttackEvent.isGunActive)
            {
                SelectPopup(currentAction, popupIndex, selectionFunctionality);
            }
        }
        else
        {
            if (!WeaponAttackEvent.isGunActive)
            {
                SelectPopup(currentAction, popupIndex, selectionFunctionality);
            }
        }
    }
    private void SelectPopup(PlayerAction currentAction, int popupIndex, POPSelectionFunctionality selectionFunctionality)
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
        oldcurrentStep = popupIndex;
    }
    internal void HideFieldPopup()
    {
        for (int i = 0; i < PopupImg.Length; i++)
        {
            PopupImg[i].SetActive(false);
        }
        sickleWeapon.SetActive(false);
        wateringTool.SetActive(false);
        cleaningTool.SetActive(false);
        cleanigEffect.SetActive(false);
        seedsBag.gameObject.SetActive(false);
        GameManager.Instance.StopCurrentAnimations(); // Stop any active animations
    }
    public void ShowPopUpNotEnoughPoints(string text)
    {
        warningPopupPanelEnergy.SetActive(true);
        warningTextEnergy.text = text;
        PanelManager.RegisterPanel(warningPopupPanelEnergy);
    }
    void HideShowPopUpNotEnoughPoints()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            PanelManager.HideAllPanels();
        }
    }
  
   internal void ToInstantiateLandHealthbar(int fieldID)
    {
        string landName = "";
        ref bool isSpawned = ref isCarrotHealthBarspawn;

        switch (fieldID)
        {
            case 0:
                landName = "CarrotField";
                isSpawned = ref isCarrotHealthBarspawn;
                break;
            case 1:
                landName = "WheatField";
                isSpawned = ref isWheatHealthBarspawn;
                break;
            case 2:
                landName = "TomatoField";
                isSpawned = ref isTomatoHealthBarspawn;
                break;
            default:
                Debug.LogWarning("Invalid fieldID");
                return;
        }

        if (!isSpawned)
        {
            isSpawned = true;
            var go = Instantiate(LandHealthBarImg, lhHolderTransform);
            var rectTransform = go.GetComponent<RectTransform>();
            rectTransform.position = new Vector3(80, 0, 0);
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            go.GetComponent<LandHealth>().CurrentLandName = landName;
        }

    }
    internal void ShowLandHealthBar(int fieldID)
    {
        if (fieldID == 0)
        {
            lhHolderTransform.GetChild(2).gameObject.SetActive(true);
        }
        else if (fieldID == 1)
        {
            lhHolderTransform.GetChild(1).gameObject.SetActive(true);

        }
        else
        {
            lhHolderTransform.GetChild(0).gameObject.SetActive(true);
        }
    }
    internal void HideLandHealthBar()
    {
        for (int i = 0; i < lhHolderTransform.childCount; i++)
        {
            lhHolderTransform.GetChild(i).gameObject.SetActive(false);
        }
    }

    internal void SellHarvest()
    {
        foreach (var item in ListOfHarvestCount)
        {
            GameManager.Instance.CounttheHarvest(item.Value.Count);
        }

        sellPopupPanel.SetActive(false);
        marketPopUp.SetActive(true);
        GameManager.Instance.isHarvestCompleted = false;
        ListOfHarvestCount.Clear();
        GameManager.Instance.HarvestCount = 0;
        GrowthStartedPlants.Clear();
    }
    #endregion

}
