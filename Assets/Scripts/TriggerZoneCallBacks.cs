using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZoneCallBacks : MonoBehaviour
{

    // UnityEvents for trigger enter and exit events
    public Action<TriggerZoneCallBacks> onPlayerEnter;
    public Action<TriggerZoneCallBacks> onPlayerExit;

    // Define the player tag to identify the player
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger has the correct tag
        if (other.CompareTag(playerTag))
        {
            // Invoke all events associated with the onPlayerEnter event
            onPlayerEnter?.Invoke(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object that exited the trigger has the correct tag
        if (other.CompareTag(playerTag))
        {
            // Invoke all events associated with the onPlayerExit event
            onPlayerExit?.Invoke(this);
        }
    }
}


