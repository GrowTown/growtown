using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] internal Button useBT;
    int _count = 0;
   public int Itemcount
    {
        get => _count;
        set
        {
            _count = value;
            countText.text =value.ToString();
        }
    }

    public void Initialize(ShopItemHolder item,int count)
    {

        Itemcount += count;
        iconImage.sprite = item.iconImage.sprite;

    }


    
}
