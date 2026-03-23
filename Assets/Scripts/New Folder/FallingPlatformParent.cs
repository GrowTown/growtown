using System.Collections;
using UnityEngine;
using System.Collections;

public class FallingPlatformParent : MonoBehaviour
{
    public float fallDelay = 0.15f;
    public float disappearDelay = 1.5f;
    public bool destroyInsteadOfDisable = false;
    public bool debugLogs = true;

    private Rigidbody rb;
    private bool triggered;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError($"[FallingPlatform] No Rigidbody found on {name}. Add one to the platform ROOT.");
            enabled = false;
            return;
        }

        rb.useGravity = false;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void TriggerFall()
    {
        if (triggered) return;
        triggered = true;

        if (debugLogs) Debug.Log($"[FallingPlatform] TriggerFall: {name}");
        StartCoroutine(FallRoutine());
    }

    private IEnumerator FallRoutine()
    {
        yield return new WaitForSeconds(fallDelay);

        rb.isKinematic = false;
        rb.useGravity = true;

        // Unfreeze Y if frozen
        rb.constraints &= ~RigidbodyConstraints.FreezePositionY;

        if (debugLogs) Debug.Log($"[FallingPlatform] Falling now: {name} | kin={rb.isKinematic} grav={rb.useGravity}");

        yield return new WaitForSeconds(disappearDelay);

        if (destroyInsteadOfDisable) Destroy(gameObject);
        else gameObject.SetActive(false);
    }
}
