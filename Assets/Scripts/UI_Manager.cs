using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject marketPopUp;

    [Header("Buttons")]
    public Button wheatSeedBT;
    public Button CarrotsSeedBT;
    public Button strawberriesSeedBT;

    [Header("Text")]
    public TextMeshProUGUI text;

    [Header("References")]
    public TriggerZoneCallBacks TriggerCallBacks;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        ActivePanel();
    }

    void ActivePanel()
    {
        TriggerCallBacks.onPlayerEnter+=(a)=>marketPopUp.SetActive(true);
        TriggerCallBacks.onPlayerExit+=(e)=>marketPopUp.SetActive(false);

    }

}
