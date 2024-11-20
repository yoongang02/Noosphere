using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using VFolders.Libs;

public class InventorySlot
{
    public string evidence_id;
    public string evidence_name;
    public string can_Use;
    //public string evidence_Text_Display;
    public string artresource_id;

    public InventorySlot(EvidenceStructure evidence)
    {
        evidence_id = evidence.evidence_id;
        evidence_name = evidence.evidence_name;
        can_Use = evidence.can_Use;
        artresource_id = evidence.artresource_id;
    }
}
public class ChapterInventory
{
    public List<InventorySlot> realWorldEvidences { get; set; } = new List<InventorySlot>();
    public List<InventorySlot> mentalWorldEvidences { get; set; } = new List<InventorySlot>();
}
public class InventoryManager : Singleton<InventoryManager>
{
    //key : 챕터 숫자
    Dictionary<int, ChapterInventory> chapterInventories = new Dictionary<int, ChapterInventory>();
    
    //인벤토리 창 오픈 여부
    public bool isInventoryOpen = false;
    
    //인벤토리 UI
    [SerializeField] private GameObject _inventoryWindow;
    [SerializeField] private GameObject _realWorldInventory;
    [SerializeField] private GameObject _mentalWorldInventory;
    [SerializeField] private GameObject _inventorySlotPrefab;
    
    //챕터 관련
    [SerializeField] private GameObject _chapterParent;
    [SerializeField] private GameObject _chapterPrefab;
    public List<GameObject> chapterUIList;
    public int currentViewChapter = 0;

    void Start()
    {
        //인벤토리 초기화
        InitInventory();
    }

    void Update()
    {
        //인벤토리 열기 or 닫기
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isInventoryOpen = !isInventoryOpen;
            ControlWindow();
        }
        
        //인벤토리 닫기
        if (isInventoryOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            isInventoryOpen = !isInventoryOpen;
            ControlWindow();
        }
    }

    void InitInventory()
    {
        //챕터 정보 저장하기
        int chapterCount = Enum.GetValues(typeof(EventManagerYKM.ChapterInfo)).Length;
        InitChapter(chapterCount);
        isInventoryOpen = false;
        currentViewChapter = 0;
        chapterInventories.Clear();
        
        for (int i = 0; i < chapterCount; i++)
        {
            chapterInventories.Add(i,new ChapterInventory());
        }
    }

    void InitChapter(int chapterCount)
    {
        for (int chapter = 0; chapter < chapterCount; chapter++)
        {
            GameObject chapterUI = Instantiate(_chapterPrefab, _chapterParent.transform);
            TextMeshProUGUI chapterName = chapterUI.GetComponentInChildren<TextMeshProUGUI>();
            chapterName.text = Enum.GetName(typeof(EventManagerYKM.ChapterInfo), chapter);
            chapterUIList.Add(chapterUI);
        }

        currentViewChapter = 0;
    }

    public void AddEvidence(EvidenceStructure evidence)
    {
        //현재 어디 챕터인지 정보 가져오기
        int chapterNum = (int)EventManagerYKM.Instance.curStageInfo;
        ChapterInventory _chapterInventory = chapterInventories[chapterNum];
        
        //인벤토리 슬롯 생성
        InventorySlot newSlot = new InventorySlot(evidence);
        
        //현실 증거인지 정신세계 증거인지 csv에서 구분 필요
        if (evidence.evidence_Type == 'M')
        {
            //정신세계 증거
            _chapterInventory.mentalWorldEvidences.Add(newSlot);
            
        }
        else if (evidence.evidence_Type == 'R')
        {
            //현실세계 증거
            _chapterInventory.realWorldEvidences.Add(newSlot);
        }
        UpdateInventoryUI();
    }

    //현재 보이는 인벤토리 UI 업데이트
    public void UpdateInventoryUI()
    {
        ClearInventoryUI(_realWorldInventory);
        ClearInventoryUI(_mentalWorldInventory);
        
        ChapterInventory currentInventory = chapterInventories[currentViewChapter];

        // 현실 세계 증거물 추가
        foreach (var evidence in currentInventory.realWorldEvidences)
        {
            GameObject slot = Instantiate(_inventorySlotPrefab, _realWorldInventory.transform);
            UpdateSlotUI(slot, evidence);
        }

        // 정신 세계 증거물 추가
        foreach (var evidence in currentInventory.mentalWorldEvidences)
        {
            GameObject slot = Instantiate(_inventorySlotPrefab, _mentalWorldInventory.transform);
            UpdateSlotUI(slot, evidence);
        }
    }
    
    void ClearInventoryUI(GameObject inventoryParent)
    {
        int childCount = inventoryParent.transform.childCount;
        
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = inventoryParent.transform.GetChild(i);
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }
    }
    
    //slot UI 업데이트 함수
    void UpdateSlotUI(GameObject slot, InventorySlot evidence)
    {
        TextMeshProUGUI nameText = slot.GetComponentInChildren<TextMeshProUGUI>(true);
        nameText.text = evidence.evidence_name;
        
        Image[] images = slot.GetComponentsInChildren<Image>(true);
        foreach (Image img in images)
        {
            if (img.gameObject.name == "EvidenceImg")
            {
                //아트 리소스 불러오기
                if (EventManagerYKM.Instance._artResources.ContainsKey(evidence.artresource_id))
                {
                    img.sprite = EventManagerYKM.Instance._artResources[evidence.artresource_id].GetSpriteFromFilePath();
                }
                else
                {
                    Debug.Log(evidence.artresource_id +"가 리소스 내에 존재하지 않습니다.");
                }
                break;
            }
        }
    }
    
    //인벤토리 내에 해당 증거물이 존재하는지 확인
    public bool IsAcquiredEvidence(string evidence_id)
    {
        Debug.Log(evidence_id + "획득검사");
        foreach (var chapter in chapterInventories)
        {
            foreach (var slot in chapter.Value.realWorldEvidences)
            {
                if (slot.evidence_id == evidence_id)
                {
                    return true;
                }
            }
            foreach (var slot in chapter.Value.mentalWorldEvidences)
            {
                if (slot.evidence_id == evidence_id)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void ControlWindow()
    {
        _inventoryWindow.SetActive(isInventoryOpen);
        if(isInventoryOpen) GetComponent<InventoryNavigator>().InitNavigator(_realWorldInventory.transform,_mentalWorldInventory.transform);
    }
}
