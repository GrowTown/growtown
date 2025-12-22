using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class AreaUIScreenTrigger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject uiScreen;
    [SerializeField] private bool disableScreenOnStart = true;
    [SerializeField] private bool hideWhenPlayerLeaves = true;

    [Header("Events")]
    public UnityEvent onPlayerEnter;
    public UnityEvent onPlayerExit;

    private bool playerInside = false;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    private void Start()
    {
        if (disableScreenOnStart)
            SetScreenActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;
        SetScreenActive(true);
        onPlayerEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        if (hideWhenPlayerLeaves)
            SetScreenActive(false);

        onPlayerExit?.Invoke();
    }

    private void OnDisable()
    {
        if (hideWhenPlayerLeaves)
            SetScreenActive(false);

        playerInside = false;
    }

    private void SetScreenActive(bool active)
    {
        if (uiScreen == null)
        {
            Debug.LogWarning($"{nameof(AreaUIScreenTrigger)}: UI Screen reference not assigned.", this);
            return;
        }

        uiScreen.SetActive(active);
    }
}
