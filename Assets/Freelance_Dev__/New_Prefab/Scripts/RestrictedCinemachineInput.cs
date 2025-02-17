using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cinemachine;
using System.Collections.Generic;


public class RestrictedCinemachineInput : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    public RectTransform restrictedUIImage; // Assign the UI image in Inspector

    private void Start()
    {
        if (!freeLookCamera)
            freeLookCamera = GetComponent<CinemachineFreeLook>();

        freeLookCamera.m_XAxis.m_InputAxisName = "";
        freeLookCamera.m_YAxis.m_InputAxisName = "";
    }

    private void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        // Check if the mouse is over the UI element
        bool isMouseOverUI = RectTransformUtility.RectangleContainsScreenPoint(restrictedUIImage, mousePos);

        if (isMouseOverUI)
        {
            freeLookCamera.m_XAxis.m_InputAxisValue = 0;
            freeLookCamera.m_YAxis.m_InputAxisValue = 0;
        }
        else
        {
            freeLookCamera.m_XAxis.m_InputAxisValue = Input.GetAxis("Mouse X");
            freeLookCamera.m_YAxis.m_InputAxisValue = Input.GetAxis("Mouse Y");
        }
    }
}
