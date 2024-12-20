using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

  /*  public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;

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
        // Calculate the desired position with offset
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate between the current camera position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Set the camera's position to the smoothed position
        transform.position = smoothedPosition;

        // Optionally, make the camera look at the character
        transform.LookAt(target);
    }*/

    private Transform originalTarget; 

    [Header("Camera Follow")]
    public Transform player; // The player to follow
    public Vector3 offset = new Vector3(0, 3, -5); // Default camera offset
    public float smoothSpeed = 0.125f; // Camera follow smoothing speed

    [Header("Camera Rotation")]
    public float mouseSensitivity = 100f; // Sensitivity for mouse movement
 
    private float xRotation = 0f; // Vertical rotation (pitch)
    private float yRotation = 0f; // Horizontal rotation (yaw)

    private void Start()
    {
        // Lock the cursor to the game window
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        // Handle camera rotation
        HandleCameraRotation();
       // CameraFollowPlayer();

    }

    void CameraFollowPlayer()
    {
        // Calculate the desired position with offset
        Vector3 desiredPosition = player.position+ offset;

        // Smoothly interpolate the camera's position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Set the camera's position
        transform.position = smoothedPosition;

        // Make the camera look at the player
        transform.LookAt(player);
    }

    private void HandleCameraRotation()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Update yaw and pitch
        yRotation += mouseX;
        xRotation -= mouseY;

        // Clamp vertical rotation (pitch)
        xRotation = Mathf.Clamp(xRotation, -40f, 80f); // Limit the camera's upward and downward movement

        // Create rotation based on mouse movement
        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Calculate the desired camera position based on the player's position and offset
        Vector3 desiredPosition = player.position + rotation * offset;

        // Smoothly interpolate the camera's position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Set the camera's position
        transform.position = smoothedPosition;

        // Make the camera look at the player
        transform.LookAt(player.position + Vector3.up  ); // Adjust the look-at point for better framing
    }

}







