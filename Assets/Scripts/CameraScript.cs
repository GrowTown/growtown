using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;

  /*  private void LateUpdate()
    {
        // Calculate the desired position with offset
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate between the current camera position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Set the camera's position to the smoothed position
        transform.position = smoothedPosition;

        // Optionally, make the camera look at the character
        transform.LookAt(target);
    }*/
    // Speed of the camera's smooth follow
    public LayerMask obstructionLayer;
    public float minDistance = 2f;

    private Vector3 defaultOffset;
    private void Start() 
    {
        // Save the default offset for restoration
        defaultOffset = offset;
    }

    private void LateUpdate()
    {
        AdjustForObstruction();
    }

    private void AdjustForObstruction()
    {
        // Calculate the desired camera position based on the offset
        Vector3 desiredPosition = target.position + offset;

        // Perform a raycast from the player toward the camera to detect obstacles
        Ray ray = new Ray(target.position, desiredPosition - target.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, offset.magnitude, obstructionLayer))
        {
            // If obstruction is detected, adjust camera distance to be closer to the player
            float distanceToObstacle = Vector3.Distance(target.position, hit.point);
            distanceToObstacle = Mathf.Clamp(distanceToObstacle, minDistance, offset.magnitude);

            // Calculate a new position based on the adjusted distance
            Vector3 adjustedPosition = target.position + (desiredPosition - target.position).normalized * distanceToObstacle;
            transform.position = Vector3.Lerp(transform.position, adjustedPosition, smoothSpeed);
        }
        else
        {
            // If no obstruction, move camera back to the original offset distance smoothly
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }

        // Make the camera look at the target
        transform.LookAt(target);
    }
}



