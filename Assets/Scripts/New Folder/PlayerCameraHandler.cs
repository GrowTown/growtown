using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraHandler : MonoBehaviour
{
    public Transform CinemachineCameraTarget;
    public float Sensitivity = 1f;
    public float CameraAngleOverride = 0f;
    public float BottomClamp = -20f;
    public float TopClamp = 80f;

    public float gunYawLimit = 60f;
    public float gunPitchLimit = 60f;

    private float currentYawLimit;
    private float currentPitchLimit;

    public float CinemachineTargetYaw { get; private set; }
    public float CinemachineTargetPitch { get; private set; }

    private bool isCameraReset = false;
    private Coroutine resetCameraCoroutine;

    private void Start()
    {
        CinemachineTargetYaw = CinemachineCameraTarget.rotation.eulerAngles.y;
        CinemachineTargetPitch = CinemachineCameraTarget.rotation.eulerAngles.x;
    }

    private void Update()
    {
        if (UI_Manager.Instance.WeaponAttackEvent.isGunActive)
        {
            UpdateConstraints();
            CameraRotation();
        }
    }

    private void UpdateConstraints()
    {
        float targetYawLimit = UI_Manager.Instance.WeaponAttackEvent.isGunActive ? gunYawLimit : float.MaxValue;
        float targetPitchLimit = UI_Manager.Instance.WeaponAttackEvent.isGunActive ? gunPitchLimit : float.MaxValue;

        currentYawLimit = Mathf.Lerp(currentYawLimit, targetYawLimit, Time.deltaTime * 5f);
        currentPitchLimit = Mathf.Lerp(currentPitchLimit, targetPitchLimit, Time.deltaTime * 5f);
    }

    private void CameraRotation()
    {
        if (UI_Manager.Instance.WeaponAttackEvent.isGunActive && !isCameraReset)
        {
            StartResetCamera();
            isCameraReset = true;
        }

        float lookX = Input.GetAxisRaw("Mouse X") * Sensitivity;
        float lookY = Input.GetAxisRaw("Mouse Y") * Sensitivity;

        if (Mathf.Abs(lookX) >= 0.01f || Mathf.Abs(lookY) >= 0.01f)
        {
            CinemachineTargetYaw += lookX;
            CinemachineTargetPitch -= lookY;
        }

        CinemachineTargetYaw = Mathf.Clamp(CinemachineTargetYaw, -currentYawLimit, currentYawLimit);
        CinemachineTargetPitch = Mathf.Clamp(CinemachineTargetPitch, BottomClamp, TopClamp);

        CinemachineCameraTarget.rotation = Quaternion.Euler(CinemachineTargetPitch + CameraAngleOverride, CinemachineTargetYaw, 0f);
    }

    private void StartResetCamera()
    {
        if (resetCameraCoroutine != null)
            StopCoroutine(resetCameraCoroutine);

        resetCameraCoroutine = StartCoroutine(ResetCameraCoroutine());
    }

    private System.Collections.IEnumerator ResetCameraCoroutine()
    {
        float startYaw = CinemachineTargetYaw;
        float desiredYaw = transform.eulerAngles.y;
        float startPitch = CinemachineTargetPitch;
        float desiredPitch = 0f;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            CinemachineTargetYaw = Mathf.LerpAngle(startYaw, desiredYaw, elapsed / duration);
            CinemachineTargetPitch = Mathf.Lerp(startPitch, desiredPitch, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        CinemachineTargetYaw = desiredYaw;
        CinemachineTargetPitch = desiredPitch;
    }
}
