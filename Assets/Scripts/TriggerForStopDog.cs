using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerForStopDog : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
          
        }
    
    }

    private void OnTriggerExit(Collider other)
    {
      
    }
}
