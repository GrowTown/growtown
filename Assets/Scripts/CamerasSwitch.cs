using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerasSwitch : MonoBehaviour
{
    public CinemachineFreeLook primaryCam;
    public List<CinemachineFreeLook> virtualCams = new List<CinemachineFreeLook>();
    public CinemachineFreeLook activeCamera;
    public float minDis;
    public CinemachineVirtualCamera aimvirtualCamera;

    private Coroutine shakeCoroutine;
    private readonly List<CinemachineBasicMultiChannelPerlin> rigNoise = new List<CinemachineBasicMultiChannelPerlin>();
    private readonly List<float> rigAmplitude = new List<float>();
    private readonly List<float> rigFrequency = new List<float>();

    private Vector3 lastPlayerPosition;

    void Start()
    {
        lastPlayerPosition = transform.position;
        if (virtualCams != null && virtualCams.Count > 0)
            activeCamera = virtualCams[0];
        else
            activeCamera = primaryCam;
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
    internal void DisAbaleAllCamera()
    {
        foreach (var cam in virtualCams)
        {
            cam.enabled = false;
        }
    }

    public void ShakeCamera(float amplitude, float frequency, float duration)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine(amplitude, frequency, duration));
    }

    private IEnumerator ShakeRoutine(float amplitude, float frequency, float duration)
    {
        if (aimvirtualCamera != null && aimvirtualCamera.gameObject.activeSelf)
        {
            var noise = EnsureNoise(aimvirtualCamera);
            if (noise == null)
                yield break;

            float prevAmp = noise.m_AmplitudeGain;
            float prevFreq = noise.m_FrequencyGain;
            noise.m_AmplitudeGain = amplitude;
            noise.m_FrequencyGain = frequency;

            yield return new WaitForSeconds(Mathf.Max(0f, duration));

            noise.m_AmplitudeGain = prevAmp;
            noise.m_FrequencyGain = prevFreq;
            yield break;
        }

        CinemachineFreeLook cam = GetActiveFreeLook();
        if (cam == null)
            yield break;

        rigNoise.Clear();
        rigAmplitude.Clear();
        rigFrequency.Clear();

        for (int i = 0; i < 3; i++)
        {
            var rig = cam.GetRig(i);
            if (rig == null)
                continue;

            var noise = EnsureNoise(rig);
            if (noise == null)
                continue;

            rigNoise.Add(noise);
            rigAmplitude.Add(noise.m_AmplitudeGain);
            rigFrequency.Add(noise.m_FrequencyGain);
            noise.m_AmplitudeGain = amplitude;
            noise.m_FrequencyGain = frequency;
        }

        yield return new WaitForSeconds(Mathf.Max(0f, duration));

        for (int i = 0; i < rigNoise.Count; i++)
        {
            rigNoise[i].m_AmplitudeGain = rigAmplitude[i];
            rigNoise[i].m_FrequencyGain = rigFrequency[i];
        }
    }

    private CinemachineBasicMultiChannelPerlin EnsureNoise(CinemachineVirtualCamera vcam)
    {
        if (vcam == null)
            return null;

        var noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null)
            noise = vcam.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        return noise;
    }

    private CinemachineFreeLook GetActiveFreeLook()
    {
        if (activeCamera != null)
            return activeCamera;

        foreach (var cam in virtualCams)
        {
            if (cam != null && cam.enabled)
                return cam;
        }

        return primaryCam;
    }

    void AdjustingTHeCameras()
    {
        Vector3 playerDirection = (transform.position - lastPlayerPosition).normalized; // Player movement direction
        Vector3 toCameraDirection = (activeCamera.transform.position - transform.position).normalized; // Direction to camera

        lastPlayerPosition = transform.position; // Update last position

        // Check if the player is walking toward the camera
        minDis = Vector3.Dot(playerDirection, toCameraDirection);
    }

   


}
