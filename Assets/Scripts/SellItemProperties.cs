using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SellItemProperties: MonoBehaviour
{
    internal string itemName;
    [SerializeField] internal TextMeshProUGUI countTx;
    [SerializeField] internal Image seedIcon;
    [SerializeField] internal TMP_InputField inputFieldCountTx ;
    [SerializeField] internal Button sellBT;
  
}
