using System.Collections.Generic;

using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class FishJournalUI : MonoBehaviour
{
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private Transform contentParent;

    private Dictionary<string, FishJournalEntry> entries = new Dictionary<string, FishJournalEntry>();
    private bool isOpen = false;
    private float uncaughtAlpha = 0.45f;

    // ==============================
    // Create UI entries once (like Pokedex)
    // ==============================
    public void PopulateJournal(List<Fish> fishes)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        entries.Clear();

        foreach (var fish in fishes)
        {
            var go = Instantiate(entryPrefab, contentParent);
            var entry = go.GetComponent<FishJournalEntry>();
            entry.Setup(fish, uncaughtAlpha);
            entries.Add(fish.fishName, entry);
        }
    }

    // ==============================
    // Called when player catches new fish
    // ==============================
    public void AddFish(Fish fish)
    {
        if (entries.ContainsKey(fish.fishName))
        {
            fish.caught = true;
            entries[fish.fishName].Refresh();
        }
    }

    // ==============================
    // Toggle panel (no param needed)
    // ==============================
    public void ToggleJournal()
    {
        isOpen = !isOpen;
        gameObject.SetActive(isOpen);
    }
}
