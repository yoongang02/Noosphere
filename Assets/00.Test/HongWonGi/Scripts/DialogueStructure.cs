using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueStructure
{
    public string dialogueId;
    public string characterID;
    public string triggerType;
    public string interactionType;
    public string dialogueText;
    public string nextDialougeId;
    public List<string> Dialogue_Text_List;
}