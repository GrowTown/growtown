using UnityEngine;
using UnityEngine.UI;

public class PlayerResourceInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;
    public ToolType equippedTool = ToolType.Hammer;
    public float interactHoldTime = 1.0f;
    [Min(1)] public int smashCyclesRequired = 2;

    [Header("UI Prompt")]
    public GameObject interactUI;

    [Header("Animation")]
    public Animator animator;
    public string animationTriggerName = "Hit";

    private GatherableResource currentResource;
    private bool isHoldingE;
    private float holdTimer;
    private int completedCycles;
    private bool canMove = true;  // optional, if you use movement lock

    void Update()
    {
        if (currentResource == null)
        {
            ResetHoldState();

            if (interactUI != null && interactUI.activeSelf)
                interactUI.SetActive(false);
            return;
        }

        if (interactUI != null && !interactUI.activeSelf)
            interactUI.SetActive(true);

        if (Input.GetKeyDown(interactKey))
        {
            holdTimer = 0f;
            completedCycles = 0;
            PlayGatherAnimation();
            isHoldingE = true;
        }

        if (Input.GetKey(interactKey))
        {
            holdTimer += Time.deltaTime;

            while (holdTimer >= interactHoldTime)
            {
                holdTimer -= interactHoldTime;
                RegisterSmashCycle();

                if (isHoldingE && currentResource != null)
                    PlayGatherAnimation();
            }
        }

        if (Input.GetKeyUp(interactKey))
        {
            ResetHoldState();
        }
    }

    public void PlayGatherAnimation()
    {
        if (animator != null)
            animator.SetTrigger(animationTriggerName);
    }

    public void TriggerHit()
    {
        if (currentResource == null)
            return;

        RegisterSmashCycle();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out GatherableResource resource))
        {
            currentResource = resource;
            holdTimer = 0f;
            completedCycles = 0;
            isHoldingE = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out GatherableResource resource) && resource == currentResource)
        {
            currentResource = null;
            ResetHoldState();

            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }

    private void RegisterSmashCycle()
    {
        if (currentResource == null)
            return;

        completedCycles++;
        if (completedCycles < smashCyclesRequired)
            return;

        completedCycles = 0;
        currentResource.ApplyHit(equippedTool);
    }

    private void ResetHoldState()
    {
        isHoldingE = false;
        holdTimer = 0f;
        completedCycles = 0;
    }
}
