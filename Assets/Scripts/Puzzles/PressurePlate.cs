using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PressurePlate : MonoBehaviour
{
    [Header("Weight Rules")]
    [SerializeField, Min(0f)] private float requiredWeight = 1f;
    [SerializeField, Min(0f)] private float activationDelay = 0f;
    [SerializeField] private bool toggleMode = false;
    [SerializeField] private bool permanentActivation = false;

    [Header("Visual")]
    [SerializeField] private Transform plateVisual;
    [SerializeField, Min(0f)] private float depressedDistance = 0.08f;
    [SerializeField, Min(0.01f)] private float depressSpeed = 3f;

    [Header("Targets (Must Implement IActivatable)")]
    [SerializeField] private List<MonoBehaviour> targetBehaviours = new List<MonoBehaviour>();

    [Header("Diagnostics")]
    [SerializeField] private bool autoAddKinematicRigidbody = true;
    [SerializeField] private bool verboseLogging = false;

    public float CurrentWeight => _currentWeight;
    public bool IsActivated => _isActivated;

    private readonly Dictionary<WeightObject, int> _overlapCounts = new Dictionary<WeightObject, int>();
    private readonly List<IActivatable> _targets = new List<IActivatable>();

    // Tracks how much total WeightObject mass is currently on this trigger.
    private float _currentWeight;
    private bool _isActivated;
    private bool _isPermanentlyLatched;
    private bool _togglePressConsumed;
    private DelayedActionType _pendingDelayedAction = DelayedActionType.None;

    private Vector3 _plateDefaultLocalPosition;
    private Coroutine _activationDelayRoutine;
    private Coroutine _visualMoveRoutine;
    private Collider _triggerCollider;

    private enum DelayedActionType
    {
        None,
        HoldActivate,
        TogglePress
    }

    private void Awake()
    {
        _triggerCollider = GetComponent<Collider>();
        EnsureTriggerSetup();

        if (plateVisual == null)
            plateVisual = transform;

        _plateDefaultLocalPosition = plateVisual.localPosition;
        CacheTargets();
    }

    private void OnTriggerEnter(Collider other)
    {
        WeightObject weightObject = other.GetComponentInParent<WeightObject>();
        if (weightObject == null)
        {
            if (verboseLogging)
                Debug.Log($"{name}: Ignored '{other.name}' because it has no WeightObject in parent hierarchy.", this);
            return;
        }

        AddWeightObject(weightObject);
        EvaluatePlateState();
    }

    private void OnTriggerExit(Collider other)
    {
        WeightObject weightObject = other.GetComponentInParent<WeightObject>();
        if (weightObject == null)
            return;

        RemoveWeightObject(weightObject);
        EvaluatePlateState();
    }

    private void AddWeightObject(WeightObject weightObject)
    {
        // Multiple colliders from the same object should only count once.
        if (_overlapCounts.TryGetValue(weightObject, out int currentCount))
        {
            _overlapCounts[weightObject] = currentCount + 1;
            return;
        }

        _overlapCounts.Add(weightObject, 1);
        _currentWeight += weightObject.Weight;
    }

    private void RemoveWeightObject(WeightObject weightObject)
    {
        if (!_overlapCounts.TryGetValue(weightObject, out int currentCount))
            return;

        currentCount--;
        if (currentCount > 0)
        {
            _overlapCounts[weightObject] = currentCount;
            return;
        }

        _overlapCounts.Remove(weightObject);
        _currentWeight = Mathf.Max(0f, _currentWeight - weightObject.Weight);
    }

    private void EvaluatePlateState()
    {
        if (_isPermanentlyLatched)
            return;

        bool meetsThreshold = _currentWeight >= requiredWeight;

        if (toggleMode)
            EvaluateToggleMode(meetsThreshold);
        else
            EvaluateHoldMode(meetsThreshold);
    }

    private void EvaluateHoldMode(bool meetsThreshold)
    {
        if (meetsThreshold)
        {
            if (_isActivated)
                return;

            RequestDelayedAction(HandleHoldActivation, DelayedActionType.HoldActivate);
            return;
        }

        CancelPendingDelay();
        SetActivated(false);
    }

    private void EvaluateToggleMode(bool meetsThreshold)
    {
        if (meetsThreshold)
        {
            if (_togglePressConsumed)
                return;

            _togglePressConsumed = true;
            RequestDelayedAction(HandleTogglePress, DelayedActionType.TogglePress);
            return;
        }

        _togglePressConsumed = false;
        CancelPendingDelay();
    }

    private void HandleHoldActivation()
    {
        if (_currentWeight < requiredWeight)
            return;

        SetActivated(true);
    }

    private void HandleTogglePress()
    {
        if (_currentWeight < requiredWeight)
            return;

        if (permanentActivation)
        {
            SetActivated(true);
            return;
        }

        SetActivated(!_isActivated);
    }

    private void SetActivated(bool active)
    {
        if (_isActivated == active)
            return;

        _isActivated = active;
        if (verboseLogging)
            Debug.Log($"{name}: Activation state changed => {_isActivated}", this);
        UpdateVisualDepression(_isActivated);

        if (_isActivated)
            ActivateTargets();
        else
            DeactivateTargets();

        if (_isActivated && permanentActivation)
            _isPermanentlyLatched = true;
    }

    private void RequestDelayedAction(System.Action action, DelayedActionType actionType)
    {
        // Prevent duplicate enter events from restarting the same delay.
        if (_activationDelayRoutine != null && _pendingDelayedAction == actionType)
            return;

        CancelPendingDelay();

        if (activationDelay <= 0f)
        {
            action.Invoke();
            return;
        }

        _pendingDelayedAction = actionType;
        _activationDelayRoutine = StartCoroutine(DelayedActionRoutine(action));
    }

    private IEnumerator DelayedActionRoutine(System.Action action)
    {
        yield return new WaitForSeconds(activationDelay);
        _activationDelayRoutine = null;
        _pendingDelayedAction = DelayedActionType.None;
        action.Invoke();
    }

    private void CancelPendingDelay()
    {
        if (_activationDelayRoutine == null)
            return;

        StopCoroutine(_activationDelayRoutine);
        _activationDelayRoutine = null;
        _pendingDelayedAction = DelayedActionType.None;
    }

    private void UpdateVisualDepression(bool depressed)
    {
        if (plateVisual == null)
            return;

        Vector3 target = depressed
            ? _plateDefaultLocalPosition - Vector3.up * depressedDistance
            : _plateDefaultLocalPosition;

        if (_visualMoveRoutine != null)
            StopCoroutine(_visualMoveRoutine);

        _visualMoveRoutine = StartCoroutine(MovePlateVisualRoutine(target));
    }

    private IEnumerator MovePlateVisualRoutine(Vector3 targetLocalPosition)
    {
        while (Vector3.Distance(plateVisual.localPosition, targetLocalPosition) > 0.0005f)
        {
            plateVisual.localPosition = Vector3.MoveTowards(
                plateVisual.localPosition,
                targetLocalPosition,
                depressSpeed * Time.deltaTime);

            yield return null;
        }

        plateVisual.localPosition = targetLocalPosition;
        _visualMoveRoutine = null;
    }

    private void ActivateTargets()
    {
        for (int i = 0; i < _targets.Count; i++)
            _targets[i].Activate();
    }

    private void DeactivateTargets()
    {
        for (int i = 0; i < _targets.Count; i++)
            _targets[i].Deactivate();
    }

    private void CacheTargets()
    {
        _targets.Clear();

        for (int i = 0; i < targetBehaviours.Count; i++)
        {
            MonoBehaviour behaviour = targetBehaviours[i];
            if (behaviour == null)
                continue;

            IActivatable activatable = behaviour as IActivatable;
            if (activatable == null)
            {
                Debug.LogWarning($"{name}: Target '{behaviour.GetType().Name}' does not implement IActivatable.", this);
                continue;
            }

            _targets.Add(activatable);
        }

        if (verboseLogging)
            Debug.Log($"{name}: Cached {_targets.Count} IActivatable target(s).", this);
    }

    private void EnsureTriggerSetup()
    {
        if (_triggerCollider == null)
        {
            Debug.LogWarning($"{name}: PressurePlate requires a Collider configured as trigger.", this);
            return;
        }

        if (!_triggerCollider.isTrigger)
        {
            Debug.LogWarning($"{name}: Collider is not trigger. Enabling Is Trigger automatically.", this);
            _triggerCollider.isTrigger = true;
        }

        if (!autoAddKinematicRigidbody)
            return;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            return;

        rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        if (verboseLogging)
            Debug.Log($"{name}: Added kinematic Rigidbody to guarantee trigger callbacks.", this);
    }

    private void OnDisable()
    {
        CancelPendingDelay();

        if (_visualMoveRoutine != null)
        {
            StopCoroutine(_visualMoveRoutine);
            _visualMoveRoutine = null;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        requiredWeight = Mathf.Max(0f, requiredWeight);
        activationDelay = Mathf.Max(0f, activationDelay);
        depressedDistance = Mathf.Max(0f, depressedDistance);
        depressSpeed = Mathf.Max(0.01f, depressSpeed);
    }
#endif
}
