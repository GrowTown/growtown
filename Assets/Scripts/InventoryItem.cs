using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI rarityText;

    [SerializeField] internal Button useBT;
    [SerializeField] internal Button sellBT;

    int _count = 0;
    public int Itemcount
    {
        get => _count;
        set
        {
            _count = value;
            if (countText != null)
                countText.text = value.ToString();
        }
    }

    public void Initialize(ShopItemHolder item, int count)
    {
        Itemcount += count;

        if (item.iconImage != null && iconImage != null)
        {
            iconImage.sprite = item.iconImage.sprite;
            iconImage.color = Color.white;
        }

        if (nameText != null)  nameText.text  = item.Item.itemName;
        if (priceText != null) priceText.text = $"${item.Item.price}";
        if (rarityText != null) rarityText.text = "";
    }

    public void SetupFish(Fish fish)
    {
        if (fish == null) return;

        if (fish.caught)
        {
            if (iconImage != null)
            {
                iconImage.sprite = fish.icon;
                iconImage.color = Color.white;
                iconImage.preserveAspect = true;
            }

            if (nameText != null)  nameText.text  = fish.fishName;
            if (priceText != null) priceText.text = $"${fish.value}";
            if (rarityText != null) rarityText.text = fish.rarity;
        }
        else
        {
            if (iconImage != null)
            {
                iconImage.sprite = fish.icon;
                iconImage.color = new Color(1, 1, 1, 0.2f);
                iconImage.preserveAspect = true;
            }

            if (nameText != null)  nameText.text  = "???";
            if (priceText != null) priceText.text = "?";
            if (rarityText != null) rarityText.text = "Unknown";
        }
    }
}
