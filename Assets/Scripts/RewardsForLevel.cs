using System.Collections.Generic;
using UnityEngine;

public class RewardsForLevel : MonoBehaviour
{
    public class LevelReward
    {
        public List<Reward> Rewards { get; set; } = new List<Reward>();
    }

    internal  Dictionary<string, LevelReward> _levelRewards;

    private void Start()
    {
        InitializeLevelRewards();
    }

    private void InitializeLevelRewards()
    {
        _levelRewards = new Dictionary<string, LevelReward>
        {
            {
                "level2", new LevelReward
                {
                    Rewards = new List<Reward>
                    {
                        new Reward { RewardType = "Item", Name = "Pesticide", Value = 1 },
                        new Reward { RewardType = "Energy",Name="EnergyPoints", Value = 50 },
                        new Reward { RewardType = "Water",Name="WaterPoints",Value = 100 },
                    }
                }
            },
            {
                "level3", new LevelReward
                {
                    Rewards = new List<Reward>
                    {
                       new Reward { RewardType = "Unlock", Name = "WheatLand",Value=1 },
                       new Reward { RewardType = "Seed", Name = "WheatSeed", Value = 50 },
                       new Reward {RewardType="Unlock",Name="SuperXp",Value=1}
                    }
                }
            },
            {
                "level4", new LevelReward
                {
                    Rewards = new List<Reward>
                    {
                       new Reward { RewardType = "Item", Name = "Pesticide", Value = 1 },
                        new Reward { RewardType = "Energy",Name="EnergyPoints", Value = 50 },
                        new Reward { RewardType = "Water", Name = "WaterPoints", Value = 100 },
                    }
                }
            },
            {
                "level5", new LevelReward
                {
                    Rewards = new List<Reward>
                    {
                        new Reward { RewardType = "Unlock", Name = "BeansLand",Value=1 }
                    }
                }
            },
            {
                "level6", new LevelReward
                {
                    Rewards = new List<Reward>
                    {
                        new Reward { RewardType = "Debug", Name = "Level6 placeholder rewards" }
                    }
                }
            },
            {
                "level7", new LevelReward
                {
                    Rewards = new List<Reward>
                    {
                        new Reward { RewardType = "Debug", Name = "Level7 placeholder rewards" }
                    }
                }
            },
            {
                "level8", new LevelReward
                {
                    Rewards = new List<Reward>
                    {
                        new Reward { RewardType = "Debug", Name = "Level8 placeholder rewards" }
                    }
                }
            },
            {
                "level9", new LevelReward
                {
                    Rewards = new List<Reward>
                    {
                        new Reward { RewardType = "Debug", Name = "Level9 placeholder rewards" }
                    }
                }
            },
            {
                "level10", new LevelReward
                {
                    Rewards = new List<Reward>
                    {
                        new Reward { RewardType = "Debug", Name = "Level10 placeholder rewards" }
                    }
                }
            },
        };
    }

    public List<Reward> GetRewardsForLevel(string levelKey)
    {
        if (_levelRewards.TryGetValue(levelKey, out LevelReward levelReward))
        {
            return levelReward.Rewards;
        }
        return null;
    }
    internal void LevelRewards(string level)
    {
        if (_levelRewards.TryGetValue(level, out var levelReward))
        {
            foreach (var reward in levelReward.Rewards)
            {
                switch (reward.RewardType)
                {
                    case "Item":
                        Debug.Log($"Player received {reward.Name} x{reward.Value}");
                        AddItemToInventory(reward.Name, reward.Value);
                         break;

                    case "Energy":
                        Debug.Log($"Player gained {reward.Value} energy");
                        GainEnergy(reward.Value);
                        break;

                    case "Water":
                        Debug.Log($"Player gained {reward.Value} water");
                        GainWater(reward.Value);
                        break;

                    case "Seed":
                        Debug.Log($"Player received {reward.Name} seeds x{reward.Value}");
                        AddSeedToInventory(reward.Name, reward.Value);
                        break;

                    case "Unlock":
                        Debug.Log($"Player unlocked: {reward.Name}");
                        UnlockFeature(reward.Name);
                        break;

                    case "Debug":
                        Debug.Log($"Debug placeholder: {reward.Name}");
                        break;

                    default:
                        Debug.LogWarning($"Unknown reward type: {reward.RewardType}");
                        break;
                }
            }
        }
        else
        {
            Debug.LogWarning($"No rewards configured for level {level}");
        }
    }
    private void AddItemToInventory(string itemName, int quantity)
    {
        switch (itemName)
        {
            case "Pesticide":
                GameManager.Instance.CurrentPasticideCount += quantity;
                break;

            default:
                Debug.Log($"For this logic not implemented for: {itemName}");
                break;
        }
    }
    /*   internal void LevelRewards(string level)
       {
           if ("level1" == level)
           {
               CurrentPasticideCount += 1;
               if (CurrentEnergyCount < 500)
                   CurrentEnergyCount += 50;
               if (CurrentWaterCount < 500)
                   CurrentWaterCount += 100;
               CurrentWheatSeedCount += 50;
               UI_Manager.Instance.lockImageForWheatLand.SetActive(false);
           }
       }*/
    private void GainEnergy(int energyAmount)
    {
        GameManager.Instance.CurrentEnergyCount = Mathf.Min(GameManager.Instance.CurrentEnergyCount + energyAmount,500);
    }

    private void GainWater(int waterAmount)
    {
       GameManager.Instance.CurrentWaterCount = Mathf.Min(GameManager.Instance.CurrentWaterCount + waterAmount, 500);
    }

    private void AddSeedToInventory(string seedName, int quantity)
    {
        switch (seedName)
        {
            case "Wheat":
                GameManager.Instance.CurrentWheatSeedCount += quantity;
                break;
            case "Tomato":
                GameManager.Instance.CurrentTomatoSeedCount += quantity;
                break;
            case "Carrot":
                GameManager.Instance.CurrentStrawberriesSeedCount += quantity;
                break;

            default:
                Debug.Log($"For this logic not implemented for: {seedName}");
                break;
        }
    }

    private void UnlockFeature(string featureName)
    {
        switch (featureName)
        {
            case "WheatLand":
                UI_Manager.Instance.lockImageForWheatLand.SetActive(false);
                UI_Manager.Instance.lockImageForWheatSeed.SetActive(false);
                break;
            case "CarrotLand":
                UI_Manager.Instance.lockImageForCarrotLand.SetActive(false);
                UI_Manager.Instance.lockImageForCarrotSeed.SetActive(false);
                break;
            case "SuperXp":
                UI_Manager.Instance.lockImageForSuperXp.SetActive(false);
                break;
            default:
                Debug.Log($"Unlock logic not implemented for: {featureName}");
                break;
        }
    }
}

public class Reward
{
    public string RewardType { get; set; }
    public string Name { get; set; }
    public int Value { get; set; } 
}
