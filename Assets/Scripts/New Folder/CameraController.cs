using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public CinemachineVirtualCamera vCam;

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minZoom = 6f;
    public float maxZoom = 16f;

    [Header("Pan Settings")]
    public float panSpeed = 20f;
    public float dragSpeed = 0.5f;
    public bool allowEdgePan = true;

    private float targetZoom;
    private Vector3 dragOrigin;

    void Start()
    {
        if (vCam == null) vCam = GetComponent<CinemachineVirtualCamera>();
        targetZoom = vCam.m_Lens.OrthographicSize; // if orthographic
        if (!vCam.m_Lens.Orthographic)
            targetZoom = vCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance;
    }

    void Update()
    {
        HandleZoom();
        HandlePan();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetZoom -= scroll * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

            if (vCam.m_Lens.Orthographic)
            {
                vCam.m_Lens.OrthographicSize = Mathf.Lerp(vCam.m_Lens.OrthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
            }
            else
            {
                var transposer = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
                transposer.m_CameraDistance = Mathf.Lerp(transposer.m_CameraDistance, targetZoom, Time.deltaTime * zoomSpeed);
            }
        }
    }

    void HandlePan()
    {
        // --- Drag with right mouse ---
        if (Input.GetMouseButtonDown(1))
            dragOrigin = Input.mousePosition;

        if (Input.GetMouseButton(1))
        {
            Vector3 diff = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(-diff.x * dragSpeed, 0, -diff.y * dragSpeed);
            transform.Translate(move, Space.World);
        }

        // --- Edge Panning ---
        if (allowEdgePan)
        {
            Vector3 move = Vector3.zero;
            if (Input.mousePosition.x <= 5) move.x -= 1;
            if (Input.mousePosition.x >= Screen.width - 5) move.x += 1;
            if (Input.mousePosition.y <= 5) move.z -= 1;
            if (Input.mousePosition.y >= Screen.height - 5) move.z += 1;
            transform.Translate(move * panSpeed * Time.deltaTime, Space.World);
        }
    }
}
