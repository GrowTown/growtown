using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDialogueInteraction : MonoBehaviour
{
    public float interactDistance = 2f;
    public KeyCode interactKey = KeyCode.E;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
            TryInteract();
    }

    void TryInteract()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactDistance);
        foreach (var hit in hits)
        {
            NPCDialogue npc = hit.GetComponent<NPCDialogue>();
            if (npc != null && DialogueManager.Instance != null && !DialogueManager.Instance.IsActive())
            {
                // ✅ Pass the entire NPCDialogue object now (not just strings)
                DialogueManager.Instance.StartDialogue(npc);
                break;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
