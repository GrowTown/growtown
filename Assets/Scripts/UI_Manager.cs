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

    [Header("Transforms")]
    public RectTransform lhHolderTransform;

    [Header("Effects")]
    public GameObject waterEffect;
    public GameObject cleanigEffect;


    [Header("Buttons")]
    public Button tomatoSeedBT;
    public Button wheatSeedBT;
    public Button strawberriesSeedBT;
    public Button cleaningWeaponBT;
    public Button wateringWeaponBT;
    public Button sickleWeaponBT;
    public Button buyInventoryBT;
    public Button sellInventoryBT;
    public Button starterPackBuyBT;
    public Button energyBuyBT;
    public Button waterBuyBT;
    public Button pasticideUseBT;
    public Button pasticideBuyBT;
    public Button superXpBuyBT;
    public Button wheatlandBuyBT;
    public Button carrotlandBuyBT;

    [Header("Text")]
    public TextMeshProUGUI score;
    public TextMeshProUGUI notEnoughMoneyText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI warningTextEnergy;
    public TextMeshProUGUI warningTextForSeed;
    public TextMeshProUGUI playerXpTxt;
    public TextMeshProUGUI pasticideCount;
    public TextMeshProUGUI pasticideMsgTxt;
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

    private PlayerLevel _playerLevel;

    internal int oldcurrentStep = -1;
    InventoryNames[] inventoryNames;
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
        if (SliderControls.gameObject.activeSelf)
        {
            superXpBuyBT.interactable = false;
        }
        else
        {
            superXpBuyBT.interactable = true;
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
        pasticideBuyBT.onClick.AddListener(() => { ShopManager.ToBuyPasticide(); });
        pasticideUseBT.onClick.AddListener(() =>
        {
            GameManager.Instance.ToIncreaseLandHealthUsePasticide();
            pasticidePopUpPanel.SetActive(false);
        });
        buyInventoryBT.onClick.AddListener(() =>
        {
            marketPopUp.SetActive(true);
            sellPopupPanel.SetActive(false);
        });
        sellInventoryBT.onClick.AddListener(() =>
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
        superXpBuyBT.onClick.AddListener(() =>
        {
            ShopManager.ToBuySuperXp();
        });
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
            if (isPlantGrowthCompleted && !WeaponAttackEvent.isHammerActive)
            {
                SelectPopup(currentAction, popupIndex, selectionFunctionality);
            }
        }
        else
        {
            if (!WeaponAttackEvent.isHammerActive)
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
    internal void ShowPasticidePop()
    {
        if (!ShopManager.isPasticidsBought)
        {
            contentOfPasticidePanel.SetActive(false);
            contentOfPasticedMsgPanel.SetActive(true);
            if (LandHealth.CurrentLandHealth <= 70)
            {
                pasticideMsgTxt.text = "your land is not good enough to Harvest and you didn't bought pasticide";
                pasticideMsgTxt.color = Color.red;
            }
            else
            {
                pasticideMsgTxt.text = "your land is good enough to Harvest";
            }
        }
        else
        {
            if (LandHealth.CurrentLandHealth >= 70)
            {
                pasticideMsgTxt.text = "your land is good enough to Harvest";
            }
            else
            {
                if (GameManager.Instance.CurrentPasticideCount > 0)
                {
                    contentOfPasticedMsgPanel.SetActive(false);
                    contentOfPasticidePanel.SetActive(true);
                }
                else
                {
                    contentOfPasticedMsgPanel.SetActive(true);
                    contentOfPasticidePanel.SetActive(false);
                    pasticideMsgTxt.text = "you have to buy pasticide";
                    pasticideMsgTxt.color = Color.red;
                }
            }
        }
    }

   /* internal void ToInstantiateLandHealthbar(int fieldID)
    {
        if (fieldID == 0)
        {
            if (!isCarrotHealthBarspawn)
            {
                isCarrotHealthBarspawn = true;
                var go = Instantiate(LandHealthBarImg, lhHolderTransform);
                go.GetComponent<LandHealth>().CurrentLandName = "CarrotField";
            }
        }
        else if (fieldID == 1)
        {
            if (!isWheatHealthBarspawn)
            {
                isWheatHealthBarspawn = true;
                var go = Instantiate(LandHealthBarImg, lhHolderTransform);
                go.GetComponent<LandHealth>().CurrentLandName = "WheatField";
            }
        }
        else
        {
            if (!isTomatoHealthBarspawn)
            {
                isTomatoHealthBarspawn = true;
                var go = Instantiate(LandHealthBarImg, lhHolderTransform);
                go.GetComponent<LandHealth>().CurrentLandName = "TomatoField";
            }
        }
    }*/
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

            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = new Vector2(80, 0); 
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
            lhHolderTransform.GetChild(i).gameObject.SetActive(true);
        }
    }

    #endregion

}
