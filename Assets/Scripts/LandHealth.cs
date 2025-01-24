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
            _landHealthBarFill.color = new Color(1f, 0.64f, 0f); // Orange
        }
        else
        {
            _landHealthBarFill.color = Color.red; 
        }
    }

    internal void ShowPasticidePop()
    {
        if (!UI_Manager.Instance.ShopManager.isPasticidsBought)
        {
            UI_Manager.Instance.contentOfPasticidePanel.SetActive(false);
            UI_Manager.Instance.contentOfPasticedMsgPanel.SetActive(true);
            if (CurrentLandHealth <= 70)
            {
                UI_Manager.Instance.pasticideMsgTxt.text = "your land is not good enough to Harvest and you didn't bought pasticide";
                UI_Manager.Instance.pasticideMsgTxt.color = Color.red;
            }
            else
            {
                UI_Manager.Instance.pasticideMsgTxt.text = "your land is good enough to Harvest";
            }
        }
        else
        {
            if (CurrentLandHealth >= 70)
            {
                UI_Manager.Instance.pasticideMsgTxt.text = "your land is good enough to Harvest";
            }
            else
            {
                if (GameManager.Instance.CurrentPasticideCount > 0)
                {
                    UI_Manager.Instance.contentOfPasticedMsgPanel.SetActive(false);
                    UI_Manager.Instance.contentOfPasticidePanel.SetActive(true);
                }
                else
                {
                    UI_Manager.Instance.contentOfPasticedMsgPanel.SetActive(true);
                    UI_Manager.Instance.contentOfPasticidePanel.SetActive(false);
                    UI_Manager.Instance.pasticideMsgTxt.text = "you have to buy pasticide";
                    UI_Manager.Instance.pasticideMsgTxt.color = Color.red;
                }
            }
        }
    }  
}

 

