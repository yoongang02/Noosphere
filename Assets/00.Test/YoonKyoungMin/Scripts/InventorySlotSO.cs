using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Evidence", menuName = "New Evidence")]
public class InventorySlot : ScriptableObject
{
    public string evidence_id;
    public string evidence_name;
    public string evidence_Text_Display;
    public string artresource_id;
}
