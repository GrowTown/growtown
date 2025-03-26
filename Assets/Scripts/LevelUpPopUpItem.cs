using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPopUpItem : MonoBehaviour
{

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI nameText;
    //[SerializeField] internal Button useBT;
    int _count = 0;
    public int Itemcount
    {
        get => _count;
        set
        {
            _count = value;
            countText.text = value.ToString();
        }
    }

    public void Initialize(ShopItemHolder item, int count)
    {

        Itemcount += count;
        iconImage.sprite = item.iconImage.sprite;
        nameText.text = item.Item.itemName;

    }


}
