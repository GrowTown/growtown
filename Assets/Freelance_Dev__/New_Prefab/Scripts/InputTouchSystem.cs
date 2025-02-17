using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTouchSystem : MonoBehaviour
{
    public float rotationSpeed = 0.2f; // Adjust rotation sensitivity
    public float smoothTime = 0.1f; // Smoothing effect for rotation
    public float momentumDamping = 0.95f; // Reduce momentum gradually

    private Vector2 lastTouchPosition;
    private Vector2 rotationVelocity;
    private bool isTouching = false;

    private float momentumX = 0f;
    private float momentumY = 0f;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                isTouching = true;
                momentumX = 0f;
                momentumY = 0f;
            }
            else if (touch.phase == TouchPhase.Moved && isTouching)
            {
                Vector2 delta = touch.position - lastTouchPosition;

                float targetRotateX = delta.x * rotationSpeed;
                float targetRotateY = -delta.y * rotationSpeed;

                rotationVelocity.x = Mathf.Lerp(rotationVelocity.x, targetRotateX, smoothTime);
                rotationVelocity.y = Mathf.Lerp(rotationVelocity.y, targetRotateY, smoothTime);

                transform.Rotate(Vector3.up, rotationVelocity.x, Space.World);
                transform.Rotate(Vector3.right, rotationVelocity.y, Space.Self);

                lastTouchPosition = touch.position;

                // Store momentum for smooth stopping effect
                momentumX = rotationVelocity.x;
                momentumY = rotationVelocity.y;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouching = false;
            }
        }
        else if (!isTouching && (Mathf.Abs(momentumX) > 0.01f || Mathf.Abs(momentumY) > 0.01f))
        {
            // Apply momentum when no touch is active
            transform.Rotate(Vector3.up, momentumX, Space.World);
            transform.Rotate(Vector3.right, momentumY, Space.Self);

            // Gradually slow down the momentum
            momentumX *= momentumDamping;
            momentumY *= momentumDamping;
        }
    }
}
