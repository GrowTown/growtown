using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    [TextArea(2, 4)] public string responseText;
}

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)]
    public string text;
    public DialogueChoice[] choices;
}

public class NPCDialogue : MonoBehaviour
{
    public string npcName = "Villager";
    public DialogueLine[] dialogueLines;
}
