using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class NPCApproachCutscene : MonoBehaviour
{
    public Transform player;
    public Transform exitPoint;
    public float moveSpeed = 2f;

    public NPCDialogue dialogue;     // FIXED HERE

    public float disappearDelay = 0.5f;
    [Tooltip("Delay before the NPC starts walking/dialogue once the scene begins.")]
    public float startDelay = 0.5f;
    [Header("Spawn")]
    [Tooltip("If true the NPC snaps next to the player on scene start before walking/playing dialogue.")]
    public bool spawnNextToPlayerOnStart = true;
    [Tooltip("Offset applied when spawning near the player.")]
    public Vector3 spawnOffset = new Vector3(1f, 0f, 0f);

    [Header("Post Dialogue")]
    [SerializeField] private GameObject dialogueCompleteImage;
    [SerializeField] private float imageDisplayDuration = 4f;

    private Animator anim;
    private bool done = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (spawnNextToPlayerOnStart && player != null)
        {
            transform.position = GetPlayerOffsetPosition(spawnOffset);
            Face(player.position);
        }
        StartCoroutine(RunCutscene());
    }

    IEnumerator RunCutscene()
    {
        if (done) yield break;
        done = true;

        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        if (pPlayerController.Instance != null)
            pPlayerController.Instance.SetCanMove(false);

        if (player != null)
            yield return StartCoroutine(WalkTo(player.position));

        if (player != null)
            Face(player.position);

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogue);

            while (DialogueManager.Instance.IsActive())
                yield return null;
        }
        else
        {
            Debug.LogWarning("NPCApproachCutscene: DialogueManager.Instance is missing.");
        }

        yield return ShowDialogueCompleteImage();

        if (exitPoint != null)
            yield return StartCoroutine(WalkTo(exitPoint.position));

        yield return new WaitForSeconds(disappearDelay);

        gameObject.SetActive(false);

        if (pPlayerController.Instance != null)
            pPlayerController.Instance.SetCanMove(true);
    }

    IEnumerator WalkTo(Vector3 target)
    {
        anim.SetBool("isWalking", true);

        while (Vector3.Distance(transform.position, target) > 0.3f)
        {
            Vector3 dir = (target - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(dir);
            yield return null;
        }

        anim.SetBool("isWalking", false);
    }

    void Face(Vector3 pos)
    {
        Vector3 dir = (pos - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    private Vector3 GetPlayerOffsetPosition(Vector3 offset)
    {
        if (player == null)
            return transform.position;

        return player.position + offset;
    }

    private IEnumerator ShowDialogueCompleteImage()
    {
        if (dialogueCompleteImage == null)
            yield break;

        dialogueCompleteImage.SetActive(true);
        if (imageDisplayDuration > 0f)
            yield return new WaitForSeconds(imageDisplayDuration);
        dialogueCompleteImage.SetActive(false);
    }
}
