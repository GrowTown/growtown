using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTool", menuName = "Shop/Tool")]
public class Tools : ShopItem
{
    [SerializeField] GameObject tool;
}
