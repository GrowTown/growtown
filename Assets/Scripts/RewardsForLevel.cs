using System.Collections.Generic;
using UnityEngine;

public class RewardsForLevel : MonoBehaviour
{
    public class LevelReward
    {
        public List<Reward> Rewards { get; set; } = new List<Reward>();
    }

    private Dictionary<string, LevelReward> _levelRewards;

    private void Start()
    {
        InitializeLevelRewards();
    }

    private void InitializeLevelRewards()
    {
        _levelRewards = new Dictionary<string, LevelReward>
        {
            {
                "level1", new LevelReward
                {
                    Rewards = new List<Reward>
                    {
                        new Reward { RewardType = "Item", Name = "Pesticide", Value = 1 },
                        new Reward { RewardType = "Energy", Value = 50 },
                        new Reward { RewardType = "Water", Value = 100 },
                        new Reward { RewardType = "Seed", Name = "Wheat", Value = 50 },

                    }
                }
            },
            {
                "level2", new LevelReward
                {
                    Rewards = new List<Reward>
                    {
                       new Reward { RewardType = "Unlock", Name = "WheatLand" },
                       new Reward {RewardType="Unlock",Name="SuperXp"}
                    }
                }
            },
            {
                "level3", new LevelReward
                {
                    Rewards = new List<Reward>
                    {
                        new Reward { RewardType = "Debug", Name = "Level3 placeholder rewards" }
                    }
                }
            },
            {
                "level4", new LevelReward
                {
                    Rewards = new List<Reward>
                    {
                        new Reward { RewardType = "Debug", Name = "Level4 placeholder rewards" }
                    }
                }
            },
            {
                "level5", new LevelReward
                {
                    Rewards = new List<Reward>
                    {
                        new Reward { RewardType = "Unlock", Name = "CarrotLand" }
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
        if (GameManager.Instance.CurrentEnergyCount < 500)
            GameManager.Instance.CurrentEnergyCount += energyAmount;
    }

    private void GainWater(int waterAmount)
    {
        if (GameManager.Instance.CurrentWaterCount < 500)
            GameManager.Instance.CurrentWaterCount += waterAmount;
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
                break;
            case "CarrotLand":
                UI_Manager.Instance.lockImageForCarrotLand.SetActive(false);
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
    public string RewardType { get; set; } // e.g., "Item", "Ability", "Healing"
    public string Name { get; set; }       // Name of the reward, e.g., "Hammer", "Special Ability"
    public int Value { get; set; }         // Reward value, e.g., healing amount, or leave 0 for items
}
