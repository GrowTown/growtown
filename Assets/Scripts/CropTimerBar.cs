using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CropTimerBar : MonoBehaviour
{
    [SerializeField] private string _name = "";
    [SerializeField] private TextMeshProUGUI _cropname;
    [SerializeField] private Slider _bar;
    [SerializeField] private Image _barfillArea;
    float _currentcropTimer = 0;
    PlantGrowth plantGrowth;

    void Start()
    {
        _cropname.text = _name;
        CurrentCropTimer = _currentcropTimer;
    }
    private void Update()
    {
        if (PlantGrowth != null)
        {
            UpdatingTheTimer();
        }
    }
    string isPlantGrowthAdded = "";

    void UpdatingTheTimer()
    {

        if (isPlantGrowthAdded=="initial")
        {
            CurrentCropTimer = PlantGrowth.CurrentGrowth;
        }
        else if(isPlantGrowthAdded == "second")
        {
            CurrentCropTimer = PlantGrowth.CurrentGrowthAfterWater;
        }
        else if( isPlantGrowthAdded =="third")
        {
            CurrentCropTimer = PlantGrowth.CurrentGrowthAfterHarvest;
        }

    }
    public float CurrentCropTimer
    {
        get => _currentcropTimer;
        set
        {
            _currentcropTimer = value;
            UpdateHealthBar(_currentcropTimer);
        }
    }

    public PlantGrowth PlantGrowth
    {
        get => plantGrowth;
        set
        {
            plantGrowth = value;
            // CurrentCropTimer=plantGrowth.CurrentGrowth;
        }
    }



    public string CurrentCropName
    {
        get => _name;
        set
        {
            _name = value;
            _cropname.text = _name;
        }
    }

    internal void GetPlant(GameObject go,string state)
    {

        PlantGrowth = go.GetComponent<PlantGrowth>();
        isPlantGrowthAdded = state;
    }
    internal void UpdateHealthBar(float value)
    {
        float maxValue = 2f;
        _bar.value = Mathf.Clamp01(value / maxValue);

        if (_currentcropTimer >= 126)
        {
            _barfillArea.color = Color.green;
        }
        else if (_currentcropTimer >= 81)
        {
            _barfillArea.color = new Color(1f, 0.64f, 0f);
        }
        else
        {
            _barfillArea.color = Color.red;
        }
    }


}
