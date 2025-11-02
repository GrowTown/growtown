using System.Collections;
using UnityEngine;

public class FishFightActor : MonoBehaviour
{
    Transform bobber;
    Transform player;
    Vector3 originPosition;
    float surfaceOffset;
    float swimSpeed = 2f;
    float fightDepth = -0.2f;

    public void Configure(Transform bobberTransform, Transform playerTransform, Vector3 spawnPosition, float bobberSurfaceHeight, float swimSpeed, float depthOffset)
    {
        bobber = bobberTransform;
        player = playerTransform;
        originPosition = spawnPosition;
        surfaceOffset = bobberSurfaceHeight;
        this.swimSpeed = Mathf.Max(0.1f, swimSpeed);
        fightDepth = depthOffset;

        transform.position = spawnPosition;
        FaceTowards(bobber != null ? bobber.position : spawnPosition + transform.forward);
    }

    public void UpdateFightProgress(float normalized)
    {
        if (bobber == null)
            return;

        normalized = Mathf.Clamp01(normalized);
        Vector3 bobberPos = bobber.position;
        Vector3 target = Vector3.Lerp(originPosition, bobberPos, normalized);
        target.y = Mathf.Lerp(originPosition.y, surfaceOffset + fightDepth * 0.2f, normalized);

        MoveTowards(target);
    }

    public IEnumerator PlayCatchAnimation(Transform landingTarget, float jumpHeight, float duration)
    {
        Vector3 start = transform.position;
        Transform destinationTransform = landingTarget != null ? landingTarget : player;
        Vector3 destination = destinationTransform != null ? destinationTransform.position : start;

        float elapsed = 0f;
        duration = Mathf.Max(0.05f, duration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 pos = Vector3.Lerp(start, destination, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * jumpHeight;
            transform.position = pos;
            FaceTowards(destination);
            yield return null;
        }
    }

    public IEnumerator PlayEscapeAnimation(float duration, float diveDepth)
    {
        Vector3 start = transform.position;
        Vector3 destination = originPosition;
        duration = Mathf.Max(0.1f, duration);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 pos = Vector3.Lerp(start, destination, t);
            float wave = Mathf.Sin(t * Mathf.PI);
            pos.y = Mathf.Lerp(start.y, destination.y, t) + wave * diveDepth;
            transform.position = pos;
            FaceTowards(destination);
            yield return null;
        }
    }

    public Vector3 GetOriginPosition() => originPosition;

    void MoveTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        float step = swimSpeed * Time.deltaTime;

        if (direction.magnitude <= step)
        {
            transform.position = target;
        }
        else
        {
            transform.position += direction.normalized * step;
        }

        FaceTowards(target);
    }

    void FaceTowards(Vector3 target)
    {
        Vector3 flatDir = target - transform.position;
        flatDir.y = 0f;
        if (flatDir.sqrMagnitude > 0.0001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(flatDir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
        }
    }
}
