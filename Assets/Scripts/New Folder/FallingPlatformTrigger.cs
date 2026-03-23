using System.Collections;
using UnityEngine;

public class FallingPlatformTrigger : MonoBehaviour
{
    public FallingPlatformParent platform;
    public bool debugLogs = true;

    private bool fired;

    void Awake()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;

        if (platform == null)
            platform = GetComponentInParent<FallingPlatformParent>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (debugLogs) Debug.Log($"[TileZone] Enter: {other.name}");

        if (fired) return;
        if (platform == null) return;

        // detect CharacterController
        var cc = other.GetComponent<CharacterController>() ?? other.GetComponentInParent<CharacterController>();
        if (cc == null) return;

        fired = true;

        if (debugLogs) Debug.Log($"[TileZone] CC detected: {cc.name} -> trigger fall");
        platform.TriggerFall();
    }
}
