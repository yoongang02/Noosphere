using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterInventory
{
    public List<InventorySlot> realWorldEvidences { get; set; } = new List<InventorySlot>();
    public List<InventorySlot> mentalWorldEvidences { get; set; } = new List<InventorySlot>();
}
public class InventoryManager : MonoBehaviour
{
    //key : 챕터 숫자
    Dictionary<int, ChapterInventory> chapterInventories = new Dictionary<int, ChapterInventory>();

    void Start()
    {
        //인벤토리 초기화
        InitInventory();
    }

    void InitInventory()
    {
        
    }

    void AddEvidence(EvidenceStructure evidence)
    {
        //현실 증거인지 정신세계 증거인지 csv에서 구분 필요
    }

    void UpdateInventoryUI()
    {
        
    }
}
