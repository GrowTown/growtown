using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class VirtualCameraControlls : MonoBehaviour
{

 [Header("Cinemachine")]
    public CinemachineVirtualCamera virtualCamera;
    public float rotationSpeed = 5f;
    public float pitchSpeed = 2f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    private CinemachineOrbitalTransposer orbitalTransposer;
    private float mouseX;
    private float mouseY;

    void Start()
    {
        // Get the Orbital Transposer component
        orbitalTransposer = virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
    }

    void Update()
    {
        if (UI_Manager.Instance.WeaponAttackEvent.isGunActive)
        {
            CamRotation();
        }
      
    }

    void CamRotation()
    {
        // Get mouse input
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * pitchSpeed;

        // Clamp the rotation angles
        mouseX = Mathf.Clamp(mouseX, -180f, 180f);
        mouseY = Mathf.Clamp(mouseY, minPitch, maxPitch);

        // Update the Orbital Transposer's heading and pitch
        if (orbitalTransposer != null)
        {
            orbitalTransposer.m_Heading.m_Bias = mouseX;
            orbitalTransposer.m_FollowOffset.y = mouseY;
        }
    }

}
