using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public Transform target;  
    public Vector3 offset; 

    public float smoothSpeed = 0.125f;

    private void LateUpdate()
    {
        // Calculate the desired position with offset
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate between the current camera position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Set the camera's position to the smoothed position
        transform.position = smoothedPosition;

        // Optionally, make the camera look at the character
        transform.LookAt(target);
    }
}



