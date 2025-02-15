using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CamerasSwitch : MonoBehaviour
{
    public CinemachineFreeLook primaryCam;
    public List<CinemachineFreeLook> virtualCams = new List<CinemachineFreeLook>();
    public CinemachineFreeLook activeCamera;
    public float minDis;
    public CinemachineVirtualCamera aimvirtualCamera;


    private Vector3 lastPlayerPosition;

    void Start()
    {
        lastPlayerPosition = transform.position;
        activeCamera = virtualCams[0];
    }

   
    // Start is called before the first frame update
    internal void SwitchToCam(int index)
    {
        foreach (var cam in virtualCams)
        {
            cam.enabled = false;
        }

        virtualCams[index].enabled = true;
        aimvirtualCamera.gameObject.SetActive(false);
        activeCamera = virtualCams[index];

        Debug.Log($"Switched to camera at index: {index}");
    }

    internal void EnableShootCameraOnly() 
    {
        foreach (var cam in virtualCams)
        {
            cam.enabled = false;
        }
        aimvirtualCamera.gameObject.SetActive( true );
    }
   

   


}
