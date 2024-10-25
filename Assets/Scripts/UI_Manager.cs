using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : MonoBehaviour
{
  
    [Header("Panels")]
    public GameObject marketPopUp;
    public GameObject inventoryPanel;

    [Header("Buttons")]
    public Button wheatSeedBT;
    public Button carrotsSeedBT;
    public Button strawberriesSeedBT;

    [Header("Text")]
    public TextMeshProUGUI score;

    [Header("References")]

    public TriggerZoneCallBacks triggerCallBacks;
    [SerializeField]
    public ShopManager shopManager;

    #region Fields
    internal int scoreIn = 100;
    #endregion

    #region Properties
    ShopManager ShopManager
    {
        get => shopManager;
        set => shopManager = value;
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
       
        TestM();
    }

    #region Functions

    SelectionFunctionality currentSelected;
    void TestM()
    {
        for (int i = 0; i < inventoryPanel.transform.childCount; i++)
        {
              var item=inventoryPanel.transform.GetChild(i).gameObject.GetComponent<SelectionFunctionality>();
            item.OnClick += (i) =>
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
        triggerCallBacks.onPlayerEnter+=(a)=>marketPopUp.SetActive(true);
        triggerCallBacks.onPlayerExit+=(e)=>marketPopUp.SetActive(false);

        carrotsSeedBT.onClick.AddListener(() => { shopManager.ToBuyCarrots(); });
        wheatSeedBT.onClick.AddListener(() => { shopManager.ToBuyWheat(); });
        strawberriesSeedBT.onClick.AddListener(() => { shopManager.ToBuyStrawberries(); });

    }
    #endregion

}
