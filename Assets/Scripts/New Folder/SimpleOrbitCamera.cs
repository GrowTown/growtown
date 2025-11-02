using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("Camera/Simple Orbit Camera (RMB / Right-side touch)")]
public class SimpleOrbitCamera : MonoBehaviour
{
    [Header("Target (assign player)")]
    public Transform target;

    [Header("Orbit settings")]
    public float distance = 6f;
    public float height = 2.0f;
    public float sensitivity = 1.0f;
    public bool invertY = false;
    public float pitchMin = -10f;
    public float pitchMax = 60f;

    // runtime
    private float yaw;
    private float pitch = 15f;
    private bool rotating = false;
    private int activeTouchId = -1;

    void Start()
    {
        if (target != null)
        {
            yaw = target.eulerAngles.y;
        }
        else
        {
            Debug.LogWarning("SimpleOrbitCamera: no target assigned.");
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        float dx = 0f, dy = 0f;
        bool inputThisFrame = GetRotateInput(out dx, out dy);

        // start/stop logging for easier debugging
        if (inputThisFrame && !rotating)
        {
            rotating = true;
            Debug.Log("Camera rotate START");
        }
        else if (!inputThisFrame && rotating)
        {
            rotating = false;
            activeTouchId = -1;
            Debug.Log("Camera rotate STOP");
        }

        if (rotating)
        {
            // apply deltas (scale so mouse and touch feel similar)
            yaw += dx * sensitivity * 100f * Time.deltaTime;
            float sign = invertY ? 1f : -1f;
            pitch += sign * dy * sensitivity * 100f * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        }

        // place camera
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPos = target.position + rot * new Vector3(0f, height, -distance);
        transform.position = desiredPos;
        transform.LookAt(target.position + Vector3.up * (height * 0.5f));
    }

    // Returns true when rotation input is active and fills dx, dy
    private bool GetRotateInput(out float dx, out float dy)
    {
        dx = 0f; dy = 0f;

        // If pointer over UI, ignore
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return false;

        // -------- Desktop / Web: Right mouse hold + drag --------
        if (Input.GetMouseButton(1))
        {
            dx = Input.GetAxis("Mouse X");
            dy = Input.GetAxis("Mouse Y");
            return Mathf.Abs(dx) > Mathf.Epsilon || Mathf.Abs(dy) > Mathf.Epsilon;
        }

        // -------- Mobile touch: only when touching on RIGHT half --------
        if (Input.touchCount > 0)
        {
            // If we already claimed a touch, use that finger
            if (activeTouchId != -1)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch t = Input.GetTouch(i);
                    if (t.fingerId == activeTouchId)
                    {
                        // ignore if touch is over UI
                        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(t.fingerId))
                            return false;

                        if (t.phase == TouchPhase.Moved)
                        {
                            // use deltaPosition scaled to screen to feel like mouse
                            dx = (t.deltaPosition.x / Screen.width) * 10f;
                            dy = (t.deltaPosition.y / Screen.height) * 10f;
                            return true;
                        }
                        else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                        {
                            activeTouchId = -1;
                            return false;
                        }

                        return false;
                    }
                }
            }
            else
            {
                // No active touch claimed yet: claim the first touch that began on right half
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch t = Input.GetTouch(i);

                    // ignore UI touch
                    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(t.fingerId))
                        continue;

                    if (t.phase == TouchPhase.Began && t.position.x > Screen.width * 0.5f)
                    {
                        activeTouchId = t.fingerId;
                        // we don't rotate on the very same Begin frame until there's movement
                        return false;
                    }
                }
            }
        }

        return false;
    }
}
