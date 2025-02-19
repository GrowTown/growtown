using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class RestrictedCineMachineVirtual : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; // Assign in Inspector
    public RectTransform restrictedUIImage; // Assign UI element to block input
    public float rotationSpeed = 5f; // Adjustable rotation speed

    private Transform cameraTransform;
    private float yaw, pitch; // Track rotation

    private void Start()
    {
        if (!virtualCamera)
        {
            Debug.LogError("Cinemachine Virtual Camera is not assigned!");
            return;
        }

        // Get the actual transform of the virtual camera
        cameraTransform = virtualCamera.transform;

        // Initialize rotation
        yaw = cameraTransform.eulerAngles.y;
        pitch = cameraTransform.eulerAngles.x;
    }

    private void Update()
    {
        if (IsPointerOverUIElement())
            return; // Stop camera movement if over UI

        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -30f, 60f); // Prevent flipping

        // Apply rotation
        cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private bool IsPointerOverUIElement()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;

        Vector2 mousePos = Input.mousePosition;
        return RectTransformUtility.RectangleContainsScreenPoint(restrictedUIImage, mousePos);
    }
}
