using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    internal bool isThroughingseeds;
    internal bool isCutting;
    internal bool isHarvestCompleted;
    internal bool isOneWorkingActionCompleted = false;

    int _currentFieldID;

    int _currentWheatSeedCount = 0;
    int _currentTomatoSeedCount = 0;
    int _currentStrawberriesSeedCount = 0;
    int _currentEnergyCount = 0;
    int _currentWaterCount = 0;
    Timer _timer;
    bool _timerStartAfterPlants;
    #region Properties
    public bool TimerStartAfterPlants
    {
        get => _timerStartAfterPlants;
        set
        {
            if (value== true)
            {
                StartCoroutine(ToIncreaseWaterPointsAccordingtoTime());
                _timerStartAfterPlants = value;
            }
        }
    }
    public int CurrentFieldID
    {
        get => _currentFieldID;
        set => _currentFieldID = value;
    }

    public int CurrentStrawberriesSeedCount
    {
        get => _currentStrawberriesSeedCount;
        set => _currentStrawberriesSeedCount = value;
    }
    public int CurrentWheatSeedCount
    {
        get => _currentWheatSeedCount;
        set => _currentWheatSeedCount = value;
    }
    public int CurrentTomatoSeedCount
    {
        get => _currentTomatoSeedCount;
        set => _currentTomatoSeedCount = value;
    }

    public int CurrentEnergyCount
    {
        get => _currentEnergyCount;
        set => _currentEnergyCount = value;
    }

    public int CurrentWaterCount
    {
        get => _currentWaterCount;
        set => _currentWaterCount = value;
    }

    #endregion

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

    #region Methods
   /* public void StartActionAnimation(PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.Clean:
                UI_Manager.Instance.cleaningTool.SetActive(true);
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(2, 1);
                //Debug.Log("Cleanig");
                break;
            case PlayerAction.Seed:
                isThroughingseeds = true;

                UI_Manager.Instance.seedsBag.gameObject.SetActive(true);
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(3, 1);
                CurrentEnergyCount -= 5;
                UI_Manager.Instance.energyText.text = CurrentEnergyCount.ToString();
                // Debug.Log("Seeding");
                break;
            case PlayerAction.Water:
                CurrentEnergyCount -= 2;
                UI_Manager.Instance.energyText.text = CurrentEnergyCount.ToString();
                ToDecreaseTheWaterPoints();
                isThroughingseeds = false;
                OnWaterTile();
                TimerStartAfterPlants = true;
                UI_Manager.Instance.wateringTool.SetActive(true);
                UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().isTileHasSeed = false;
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(4, 1);
                break;
            case PlayerAction.Harvest:
                isCutting = true;
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(5, 1);
                UI_Manager.Instance.sickleWeapon.SetActive(true);
                CurrentEnergyCount -= 3;
                UI_Manager.Instance.energyText.text = CurrentEnergyCount.ToString();

                break;
        }
    }*/
    private bool isEnergyDeducted = false; 
    public void StartActionAnimation(PlayerAction action)
    {
       
        isEnergyDeducted = false;

        switch (action)
        {
            case PlayerAction.Clean:
                UI_Manager.Instance.cleaningTool.SetActive(true);
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(2, 1);
                // Debug.Log("Cleaning");
                break;
            case PlayerAction.Seed:
                isThroughingseeds = true;
                UI_Manager.Instance.seedsBag.gameObject.SetActive(true);
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(3, 1);
                if (!isEnergyDeducted)
                {
                    DeductEnergyPoints(5); 
                }
                // Debug.Log("Seeding");
                break;
            case PlayerAction.Water:
                if (!isEnergyDeducted)
                {
                    DeductEnergyPoints(2); 
                }
                isThroughingseeds = false;
                OnWaterTile();
                TimerStartAfterPlants = true;
                UI_Manager.Instance.wateringTool.SetActive(true);
                UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().isTileHasSeed = false;
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(4, 1);
                break;
            case PlayerAction.Harvest:
                if (!isEnergyDeducted)
                {
                    DeductEnergyPoints(3); 
                }
                isCutting = true;
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(5, 1);
                UI_Manager.Instance.sickleWeapon.SetActive(true);
                break;
        }
    }

    public void StopCurrentAnimations()
    {
        UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(2, 0);
        UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(3, 0);
        UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(4, 0);
        UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(5, 0);
        UI_Manager.Instance.cleaningTool.SetActive(false);
    }
    public void ShowFieldPopup(PlayerAction currentAction)
    {
        UI_Manager.Instance.ShowPopup(currentAction);
    }

    public void HideFieldPopup()
    {
        UI_Manager.Instance.HideFieldPopup();
    }

    internal void StartPlayerAction(PlayerAction action)
    {
        UI_Manager.Instance.TriggerZoneCallBacks.playerInZone = true;
        UI_Manager.Instance.FieldGrid.StartCoverageTracking(action);
    }

    public void StopCurrentAction()
    {
        UI_Manager.Instance.FieldGrid.StopCoverageTracking();
    }

    public void CompleteAction()
    {
        HideFieldPopup();
        if (UI_Manager.Instance.TriggerZoneCallBacks.currentStep < UI_Manager.Instance.TriggerZoneCallBacks.actionSequence.Length - 1)
        {
            isOneWorkingActionCompleted = true;
            UI_Manager.Instance.TriggerZoneCallBacks.currentStep++;
            UI_Manager.Instance.oldcurrentStep = UI_Manager.Instance.TriggerZoneCallBacks.currentStep;
            ShowFieldPopup(UI_Manager.Instance.TriggerZoneCallBacks.actionSequence[UI_Manager.Instance.TriggerZoneCallBacks.currentStep]);
        }
    }


    public void OnWaterTile()
    {
        foreach (var item in UI_Manager.Instance.spawnPlantsForGrowth)
        {
            var instance = item.GetComponent<PlantGrowth>();
            StartCoroutine(instance.GrowPlant());
        }

    }

    public void StartPackToBuy()
    {
        UI_Manager.Instance.ShopManager.ToBuyTomato();
        UI_Manager.Instance.ShopManager.ToBuyCleaningTool();
        UI_Manager.Instance.ShopManager.ToBuyWateringTool();
        UI_Manager.Instance.ShopManager.ToBuyCuttingTool();
        UI_Manager.Instance.starterPackInfoPopUpPanel.SetActive(false);
        UI_Manager.Instance.sickleWeaponBT.interactable = false;
        UI_Manager.Instance.wateringWeaponBT.interactable = false;
        UI_Manager.Instance.cleaningWeaponBT.interactable = false;
    }
    bool isWaterPointDecreased;

    private void DeductEnergyPoints(int amount)
    {
        CurrentEnergyCount -= amount;
        CurrentEnergyCount = Mathf.Clamp(CurrentEnergyCount, 0, int.MaxValue); // Ensure energy doesn't go below 0
        UI_Manager.Instance.energyText.text = CurrentEnergyCount.ToString(); // Update UI
        isEnergyDeducted = true; // Set the flag to prevent further deductions
    }
    public void ToDecreaseTheWaterPoints()
    {
        if (!isWaterPointDecreased)
        {
            CurrentWaterCount -= 10;
            UI_Manager.Instance.waterText.text = CurrentWaterCount.ToString();
            isWaterPointDecreased = true;
        }
    }

    public void ToBuyEnergyPoints()
    {
        if (UI_Manager.Instance.scoreIn >= 15)
        {
            if (CurrentEnergyCount < 50)
            {
                CurrentEnergyCount += 50;
            }
            else if (CurrentEnergyCount > 50)
            {
                CurrentEnergyCount = 100;

            }
            UI_Manager.Instance.energyText.text = CurrentEnergyCount.ToString();
            UI_Manager.Instance.energyBuyBT.interactable = false;
        }
    }

    public void ToBuyWaterPoints()
    {
        if (UI_Manager.Instance.scoreIn >= 20)
        {
            if (CurrentWaterCount < 100)
            {
                CurrentWaterCount = 100;
                UI_Manager.Instance.waterText.text = CurrentWaterCount.ToString();
            }
        }
    }

   
    bool isTimerStarted;
    public IEnumerator ToIncreaseWaterPointsAccordingtoTime()
    {
        if (CurrentWaterCount < 100)
        {
            if (_timer == null)
            {
                _timer = this.gameObject.AddComponent<Timer>();
            }
            if (!isTimerStarted)
            {
                _timer.Initialize("", DateTime.Now, TimeSpan.FromMinutes(1));
                _timer.StartTimer();
                isTimerStarted = true;

            }

            // Loop while the timer is running
            while (_timer.secondsLeft > 0)
            {
                Debug.Log("Time Left :: " + _timer.secondsLeft);
                yield return null; 
            }

            
            if (CurrentWaterCount < 100)
            {
                CurrentWaterCount += 10;
                CurrentWaterCount = Mathf.Clamp(CurrentWaterCount, 0, 100);
                UI_Manager.Instance.waterText.text = CurrentWaterCount.ToString();
                isTimerStarted = false;
                StartCoroutine(ToIncreaseWaterPointsAccordingtoTime());
            }
            else
            {
                Destroy(_timer);
            }
        }
    }


    private void ResetValues()
    {
        UI_Manager.Instance.oldcurrentStep = -1;
        UI_Manager.Instance.FieldManager.fieldSteps.Remove(CurrentFieldID);
        isCutting = false;

    }
    #endregion
}
