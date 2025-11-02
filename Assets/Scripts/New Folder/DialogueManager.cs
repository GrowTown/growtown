using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Button nextButton;
    public Transform choicesParent;
    public GameObject choiceButtonPrefab;

    private NPCDialogue currentNPC;
    private int currentLine;
    private bool isActive;

    void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false);
        nextButton.onClick.AddListener(NextLine);
    }

    public void StartDialogue(NPCDialogue npc)
    {
        currentNPC = npc;
        currentLine = 0;
        isActive = true;
        dialoguePanel.SetActive(true);
        nameText.text = npc.npcName;
        ShowLine();
    }

    void ShowLine()
    {
        if (currentLine >= currentNPC.dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        var line = currentNPC.dialogueLines[currentLine];
        dialogueText.text = line.text;

        // Clear old choices
        foreach (Transform child in choicesParent)
            Destroy(child.gameObject);

        // Show choices if any
        if (line.choices != null && line.choices.Length > 0)
        {
            nextButton.gameObject.SetActive(false);
            foreach (var choice in line.choices)
            {
                GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesParent);
                TMP_Text btnText = buttonObj.GetComponentInChildren<TMP_Text>();
                btnText.text = choice.choiceText;

                buttonObj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnChoiceSelected(choice.responseText);
                });
            }
        }
        else
        {
            nextButton.gameObject.SetActive(true);
        }
    }

    void OnChoiceSelected(string response)
    {
        // Replace the dialogue text with NPC's response
        dialogueText.text = response;

        // Remove choice buttons
        foreach (Transform child in choicesParent)
            Destroy(child.gameObject);

        nextButton.gameObject.SetActive(true);
    }

    public void NextLine()
    {
        currentLine++;
        ShowLine();
    }

    void EndDialogue()
    {
        isActive = false;
        dialoguePanel.SetActive(false);
    }

    public bool IsActive() => isActive;
}
