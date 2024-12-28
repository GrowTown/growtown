using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamerasSwitch : MonoBehaviour
{
    public CinemachineFreeLook primaryCam;
    public List<CinemachineFreeLook>virtualCams=new List<CinemachineFreeLook>();
    // Start is called before the first frame update
    internal void SwitchToCam(int index)
    {
        foreach (var cam in virtualCams)
        {
            cam.enabled = false;
        }

        virtualCams[index].enabled = true;

        Debug.Log($"Switched to camera at index: {index}");
    }
}
