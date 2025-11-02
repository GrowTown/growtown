using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishDatabase", menuName = "Fishing/Fish Database")]
public class FishDatabase : ScriptableObject
{
    // The one true list of all fish
    public List<Fish> fishList = new List<Fish>();

    public List<Fish> GetAllFish()
    {
        return fishList;
    }

    /// <summary>
    /// Returns a random fish from the database.
    /// </summary>
    public Fish GetRandomFish()
    {
        if (fishList == null || fishList.Count == 0)
        {
            Debug.LogWarning("FishDatabase is empty!");
            return null;
        }
        int index = Random.Range(0, fishList.Count);
        return fishList[index];
    }

    /// <summary>
    /// Returns true if a fish with this name exists in the database.
    /// Useful when other scripts check if caught fish is valid.
    /// </summary>
    public bool ContainsFish(string fishName)
    {
        return fishList.Exists(f => f.fishName == fishName);
    }
}
