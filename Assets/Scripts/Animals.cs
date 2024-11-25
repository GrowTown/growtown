using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewAnimal", menuName = "Shop/Animal")]
public class Animals : ShopItem
{
    [SerializeField] GameObject animal;
}
