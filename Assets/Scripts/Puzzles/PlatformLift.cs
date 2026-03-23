using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class PlatformLift : MonoBehaviour, IActivatable
{
    [Header("Lift Motion")]
    [SerializeField] private Transform platformTransform;
    [SerializeField, Min(0f)] private float liftHeight = 2f;
    [SerializeField, Min(0.01f)] private float moveSpeed = 2f;
    [SerializeField] private Vector3 liftDirection = Vector3.up;
    [SerializeField] private bool useLocalDirection = true;

    private Vector3 _baseWorldPosition;
    private Coroutine _moveRoutine;

    private void Awake()
    {
        if (platformTransform == null)
            platformTransform = transform;

        _baseWorldPosition = platformTransform.position;
    }

    public void Activate()
    {
        MoveTo(GetRaisedPosition());
    }

    public void Deactivate()
    {
        MoveTo(_baseWorldPosition);
    }

    private Vector3 GetRaisedPosition()
    {
        // Supports lifting along local axis (rotated platforms) or world axis.
        Vector3 direction = useLocalDirection
            ? transform.TransformDirection(liftDirection.normalized)
            : liftDirection.normalized;

        return _baseWorldPosition + direction * liftHeight;
    }

    private void MoveTo(Vector3 targetWorldPosition)
    {
        if (_moveRoutine != null)
            StopCoroutine(_moveRoutine);

        _moveRoutine = StartCoroutine(MoveRoutine(targetWorldPosition));
    }

    private IEnumerator MoveRoutine(Vector3 targetWorldPosition)
    {
        while (Vector3.Distance(platformTransform.position, targetWorldPosition) > 0.001f)
        {
            platformTransform.position = Vector3.MoveTowards(
                platformTransform.position,
                targetWorldPosition,
                moveSpeed * Time.deltaTime);

            yield return null;
        }

        platformTransform.position = targetWorldPosition;
        _moveRoutine = null;
    }

    private void OnDisable()
    {
        if (_moveRoutine != null)
        {
            StopCoroutine(_moveRoutine);
            _moveRoutine = null;
        }
    }
}
