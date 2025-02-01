using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerForStoppingTheRun : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UI_Manager.Instance.IsPlayerInSecondZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UI_Manager.Instance.IsPlayerInSecondZone = false;
        }
    }
}
