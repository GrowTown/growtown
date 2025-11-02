using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FreeLookRightClickHorizontal : MonoBehaviour
{
    public CinemachineFreeLook freeLookCam;
    public float sensitivity = 2f;
    private bool isDragging;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isDragging = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (isDragging)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            freeLookCam.m_XAxis.Value += mouseX; // horizontal only
        }
    }
}
