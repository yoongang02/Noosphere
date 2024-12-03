using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueStructure
{
    public string dialogue_id;
    public string character_id;
    public string trigger_type;
    public string interaction_type;
    public string dialogue_Text;
    public string next_dialouge_id;
    public List<string> Dialogue_Text_List;
}