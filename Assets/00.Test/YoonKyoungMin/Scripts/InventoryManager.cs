using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot
{
    public string evidenceId;
    public string evidenceName;
    public char canUse;
    public string artresourceId;

    public InventorySlot(EvidenceStructure evidence)
    {
        evidenceId = evidence.evidenceId;
        evidenceName = evidence.evidenceName;
        canUse = evidence.canUse;
        artresourceId = evidence.artresourceId;
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
    
    //인벤토리 UI
    [Header("인벤토리 UI 오브젝트")]
    [SerializeField] private GameObject _realWorldInventory;
    [SerializeField] private GameObject _mentalWorldInventory;
    [SerializeField] private GameObject _inventorySlotPrefab;
    
    [Space(5)][Header("인벤토리 정보")]
    //챕터 관련
    public List<GameObject> deselectedChapterUIList;
    public List<GameObject> selectedChapterUIList;
    public int currentViewChapter = 0;

    private InventoryNavigator _navigator;

    [Space(5)] [Header("증거물 사용 정보")] [SerializeField]
    private string usingEvidenceId;
    void Start()
    {
        //인벤토리 초기화
        InitInventory();
    }

    void Update()
    {
        //인벤토리 열기
        if (!UIManager.Instance.IsAnyUIOpen())
        {
            //키보드 입력 - Tab 버튼
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                UIManager.Instance.OpenUI(UIManager.Instance.inventoryUI);
            }
        
            //마우스 입력 - 인벤토리 아이콘 클릭
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                if (result.gameObject == UIManager.Instance.inventoryIcon && Input.GetMouseButtonDown(0))
                {
                    UIManager.Instance.OpenUI(UIManager.Instance.inventoryUI);
                }
            }
        }
    }
    
    //인벤토리 초기화(맨 처음 실행 후, 다시 실행되지 않음)
    void InitInventory()
    {
        //챕터 정보 저장하기
        int chapterCount = Enum.GetValues(typeof(EventManagerYKM.ChapterInfo)).Length;
        currentViewChapter = 0;
        chapterInventories.Clear();
        
        for (int i = 0; i < chapterCount; i++)
        {
            chapterInventories.Add(i,new ChapterInventory());
        }
    }

    //증거물 가져오기
    public void AddEvidence(EvidenceStructure evidence)
    {
        //현재 어디 챕터인지 정보 가져오기
        int chapterNum = (int)EventManagerYKM.Instance.curStageInfo;
        ChapterInventory _chapterInventory = chapterInventories[chapterNum];
        
        //인벤토리 슬롯 생성
        InventorySlot newSlot = new InventorySlot(evidence);
        
        //현실 증거인지 정신세계 증거인지 csv에서 구분 필요
        if (evidence.evidenceType == 'M')
        {
            //정신세계 증거
            _chapterInventory.mentalWorldEvidences.Add(newSlot);
        }
        else if (evidence.evidenceType == 'R')
        {
            //현실세계 증거
            _chapterInventory.realWorldEvidences.Add(newSlot);
        }
        
        //맵에서 증거 오브젝트 파괴하기 - 현재 오류 존재. 다시 씬으로 이동해오면 원상복구 됨. 아예 삭제 해 버려야 함.
        /*
        if (PlayerInteract.Instance._evidenceObjectInScene != null)
        {
            Debug.Log("맵에서 습득한 오브젝트 파괴");
            Destroy(PlayerInteract.Instance._evidenceObjectInScene);
        }
        */
    }

    //현재 보이는 인벤토리 UI 업데이트
    public void UpdateInventoryUI()
    {
        //현실 세계, 정신 세계 인벤토리 초기화
        StartCoroutine(ClearInventoryUI());
    }
    
    //인벤토리 슬롯 초기화
    IEnumerator ClearInventoryUI()
    {
        int childCount = _realWorldInventory.transform.childCount;
        Debug.Log($"{_realWorldInventory.name}의 자식은 {childCount}개 입니다.");

        if (childCount > 0)
        {
            for (int i = childCount - 1; i >= 0; i--)
            {
                Transform child = _realWorldInventory.transform.GetChild(i);
                if (child != null)
                {
                    Debug.Log($"{_realWorldInventory.name}의 자식 {child.name}을 제거");
                    Destroy(child.gameObject);
                }
            }
        }
        
        childCount = _mentalWorldInventory.transform.childCount;
        Debug.Log($"{_mentalWorldInventory.name}의 자식은 {childCount}개 입니다.");

        if (childCount > 0)
        {
            for (int i = childCount - 1; i >= 0; i--)
            {
                Transform child = _mentalWorldInventory.transform.GetChild(i);
                if (child != null)
                {
                    Debug.Log($"{_mentalWorldInventory.name}의 자식 {child.name}을 제거");
                    Destroy(child.gameObject);
                }
            }
        }
        yield return null;
        
        //현재 챕터에 따른 인벤토리 가져오기
        ChapterInventory currentInventory = chapterInventories[currentViewChapter];

        // 현실 세계 증거물 추가
        if (currentInventory.realWorldEvidences.Count != 0)
        {
            foreach (var evidence in currentInventory.realWorldEvidences)
            {
                GameObject slot = Instantiate(_inventorySlotPrefab, _realWorldInventory.transform);
                slot.GetComponent<InventorySlotInfo>().evidenceId = evidence.evidenceId;
                UpdateSlotUI(slot, evidence);
            }
        }

        if (currentInventory.mentalWorldEvidences.Count != 0)
        {
            // 정신 세계 증거물 추가
            foreach (var evidence in currentInventory.mentalWorldEvidences)
            {
                GameObject slot = Instantiate(_inventorySlotPrefab, _mentalWorldInventory.transform);
                slot.GetComponent<InventorySlotInfo>().evidenceId = evidence.evidenceId;
                UpdateSlotUI(slot, evidence);
            }
        }
        yield return null;
        
        //인벤토리 네비게이션 업데이트
        GetComponent<InventoryNavigator>().InitNavigator(_realWorldInventory,_mentalWorldInventory);

        yield return null;
        _navigator = GetComponent<InventoryNavigator>();
    }
    
    //slot UI 업데이트 함수
    void UpdateSlotUI(GameObject slot, InventorySlot evidence)
    {
        TextMeshProUGUI nameText = slot.GetComponentInChildren<TextMeshProUGUI>(true);
        nameText.text = evidence.evidenceName;
        
        Image[] images = slot.GetComponentsInChildren<Image>(true);
        foreach (Image img in images)
        {
            if (img.gameObject.name == "EvidenceImg")
            {
                //아트 리소스 불러오기
                if (DataManager.Instance._artResources.ContainsKey(evidence.artresourceId))
                {
                    ArtResourceStructure artResource = DataManager.Instance._artResources[evidence.artresourceId];
                    img.sprite = artResource.GetSpriteFromFilePath(artResource.filePathInventoryThumbnail);
                }
                else
                {
                    Debug.Log(evidence.artresourceId +"가 리소스 내에 존재하지 않습니다.");
                }
                break;
            }
        }
    }
    
    //인벤토리 내에 해당 증거물이 존재하는지 확인
    public bool IsAcquiredEvidence(string evidence_id)
    {
        int chapterIndex = (int)EventManagerYKM.Instance.curStageInfo;
        
        if (chapterInventories[chapterIndex].realWorldEvidences.Count > 0)
        {
            foreach (var slot in chapterInventories[chapterIndex].realWorldEvidences)
            {
                if (slot.evidenceId == evidence_id)
                {
                    return true;
                }
            }
        }

        if (chapterInventories[chapterIndex].mentalWorldEvidences.Count > 0)
        {
            foreach (var slot in chapterInventories[chapterIndex].mentalWorldEvidences)
            {
                if (slot.evidenceId == evidence_id)
                {
                    return true;
                }
            }
        }
        
        return false;
    }
}
