using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueStructure
{
    public class DialougeText
    {
        public string key;
        public string tutorialID;
        public string characterId;
    }
    
    public string dialogueId;
    public string characterId;
    public string triggerType;
    public string interactionType;
    public string dialogueKey;
    public string nextDialougeId;
    public string tutorialId;
    public List<DialougeText> Dialogue_Text_List;
}