using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingZone : MonoBehaviour
{
    [Header("Water Surface")]
    public Transform waterSurfaceReference;
    public float waterSurfaceHeight = 0.2f;
    public float bobberHeightOffset = 0f;

    public float GetWaterSurfaceHeight()
    {
        if (waterSurfaceReference != null)
            return waterSurfaceReference.position.y;

        return waterSurfaceHeight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, transform.localScale);

        float surfaceY = GetWaterSurfaceHeight();
        Vector3 surfaceCenter = transform.position;
        surfaceCenter.y = surfaceY;
        Vector3 radius = transform.localScale * 0.5f;
        Gizmos.DrawLine(surfaceCenter + new Vector3(-radius.x, 0f, 0f), surfaceCenter + new Vector3(radius.x, 0f, 0f));
        Gizmos.DrawLine(surfaceCenter + new Vector3(0f, 0f, -radius.z), surfaceCenter + new Vector3(0f, 0f, radius.z));
    }
}
