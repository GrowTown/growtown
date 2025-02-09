 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LandHealth : MonoBehaviour
{

    [SerializeField]
    private Slider _landHealthBar;
    [SerializeField]
    private int _landHealth = 100;
    [SerializeField]
    private TextMeshProUGUI _landHealthTxt;
    [SerializeField]
    private TextMeshProUGUI _landNameTxt;
    [SerializeField]
    private Image _landHealthBarFill;
    [SerializeField]
    string _landName = "";
    internal bool isPasticidsBought;
    private int _pasticideCount = 0;

    public int CurrentLandHealth
    {
        get => _landHealth;
        set
        {
            _landHealth = Mathf.Clamp(value, 0, 100); 
            _landHealthTxt.text = _landHealth.ToString() + "%";
            UpdateHealthBar();
        }
    }
    public int CurrentPasticideCount
    {
        get=> _pasticideCount;
        set=>_pasticideCount=value;
    }

    public string CurrentLandName
    {
        get => _landName;
        set
        {
            _landName=value;
            _landNameTxt.text = _landName;
        }
    }

    private void Start()
    {
        _landNameTxt.text = _landName;
        CurrentLandHealth = _landHealth;
    }

    internal void LandHealthIncrease(int lh)
    {
        CurrentLandHealth += lh;
    }

    internal void LandHealthDecrease(int lh)
    {
        CurrentLandHealth -= lh;
    }

    private void UpdateHealthBar()
    {
       
        _landHealthBar.value = _landHealth / 100f;

        if (_landHealth > 70)
        {
            _landHealthBarFill.color = Color.green;
        }
        else if (_landHealth > 45)
        {
            _landHealthBarFill.color = new Color(1f, 0.64f, 0f);
        }
        else
        {
            _landHealthBarFill.color = Color.red; 
        }
    }

    internal void ShowPasticidePop()
    {
        if (!isPasticidsBought)
        {
            if (CurrentLandHealth <= 70)
            {
                 UI_Manager.Instance.contentOfPasticidePanel.SetActive(false);
                UI_Manager.Instance.contentOfPasticedMsgPanel.SetActive(false);
                UI_Manager.Instance.contentOfNotBuyPasticedMsgPanel.SetActive(true);
                UI_Manager.Instance.contentOfNotBuyPasticedMsgTxt.text = "your land is not good enough to Harvest and you didn't bought pasticide,go market area buy them";
                UI_Manager.Instance.pasticideNotBoughtBT.gameObject.SetActive(false);
                UI_Manager.Instance.pasticideNotBoughNormalHealthtBT.gameObject.SetActive(true);

            }
            else if(CurrentLandHealth == 0)
            {
                UI_Manager.Instance.contentOfPasticidePanel.SetActive(false);
                UI_Manager.Instance.contentOfPasticedMsgPanel.SetActive(false);
                UI_Manager.Instance.contentOfNotBuyPasticedMsgPanel.SetActive(true);
                UI_Manager.Instance.contentOfNotBuyPasticedMsgTxt.text = "your land is not good enough to Harvest and you didn't bought pasticide,go market area buy them";
                UI_Manager.Instance.pasticideNotBoughtBT.gameObject.SetActive(true);
                UI_Manager.Instance.pasticideNotBoughNormalHealthtBT.gameObject.SetActive(false);
            }
            else
            {
                 UI_Manager.Instance.contentOfPasticedMsgPanel.SetActive(true);
            }
        }
        else
        {
            if (CurrentLandHealth >= 70)
            {
                UI_Manager.Instance.contentOfPasticidePanel.SetActive(false);
                UI_Manager.Instance.contentOfNotBuyPasticedMsgPanel.SetActive(false);
                UI_Manager.Instance.contentOfPasticideNormalHealthPanel.SetActive(false);
                UI_Manager.Instance.contentOfPasticedMsgPanel.SetActive(true);
            }
            else
            {

                if (CurrentPasticideCount > 0)
                {
                    if (CurrentLandHealth <= 45)
                    {
                        UI_Manager.Instance.contentOfPasticedMsgPanel.SetActive(false);
                        UI_Manager.Instance.contentOfNotBuyPasticedMsgPanel.SetActive(false);
                        UI_Manager.Instance.contentOfPasticidePanel.SetActive(true);
                        UI_Manager.Instance.contentOfPasticideNormalHealthPanel.SetActive(false);
                    }
                    else if(CurrentLandHealth <= 70)
                    {
                       UI_Manager.Instance.contentOfPasticedMsgPanel.SetActive(false);
                       UI_Manager.Instance.contentOfNotBuyPasticedMsgPanel.SetActive(false);
                       UI_Manager.Instance.contentOfPasticideNormalHealthPanel.SetActive(true);
                       UI_Manager.Instance.contentOfPasticidePanel.SetActive(false);
                    }
                }
                else
                {
                    UI_Manager.Instance.contentOfNotBuyPasticedMsgPanel.SetActive(true);
                    UI_Manager.Instance.contentOfPasticedMsgPanel.SetActive(false);
                    UI_Manager.Instance.contentOfPasticidePanel.SetActive(false);
                    UI_Manager.Instance.contentOfPasticideNormalHealthPanel.SetActive(false);
                    UI_Manager.Instance.contentOfNotBuyPasticedMsgTxt.text = "you have to buy pasticide,go market area buy them";
                }
            }
        }
    }  
}

 

