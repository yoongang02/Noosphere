using System;
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
public class InventoryManager : UIBase
{
    //싱글톤
    private static InventoryManager _instance;
    
    public static InventoryManager Instance 
    { 
        get 
        { 
            if (_instance == null) 
            {
                _instance = FindObjectOfType<InventoryManager>();
                if (_instance == null) 
                {
                    GameObject singletonObject = new GameObject(nameof(InventoryManager));
                    _instance = singletonObject.AddComponent<InventoryManager>();
                }
            }
            return _instance;
        } 
    }
    
    //key : 챕터 숫자
    Dictionary<int, ChapterInventory> chapterInventories = new Dictionary<int, ChapterInventory>();
    
    //인벤토리 UI
    [Header("인벤토리 UI 오브젝트")]
    [SerializeField] private GameObject _inventoryWindow;
    [SerializeField] private GameObject _realWorldInventory;
    [SerializeField] private GameObject _mentalWorldInventory;
    [SerializeField] private GameObject _inventorySlotPrefab;
    [SerializeField] private GameObject _inventoryIcon;
    
    [Space(5)][Header("인벤토리 정보")]
    //챕터 관련
    public List<GameObject> deselectedChapterUIList;
    public List<GameObject> selectedChapterUIList;
    public int currentViewChapter = 0;
    
    void Awake(){
        
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); 
    }
    void Start()
    {
        //인벤토리 초기화
        InitInventory();
    }

    void Update()
    {
        //인벤토리 열기
        if (!UIManager.Instance.IsUIOpen(this))
        {
            //키보드 입력 - Tab 버튼
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                UIManager.Instance.OpenUI(this);
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
                if (result.gameObject == _inventoryIcon && Input.GetMouseButtonDown(0))
                {
                    UIManager.Instance.OpenUI(this);
                }
            }
        }
    }

    public override void OnOpen()
    {
        base.OnOpen();
        _inventoryWindow.SetActive(true);
        //인벤토리 열었을 때, 현재 상태를 바탕으로 인벤토리 업데이트 진행
        GetComponent<InventoryNavigator>().InitNavigator(_realWorldInventory.transform,_mentalWorldInventory.transform);
    }

    public override void OnClose()
    {
        base.OnClose();
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();
        GetComponent<InventoryNavigator>().HandleKeyboardInput();
    }
    
    public override void HandleMouseInput()
    {
        base.HandleMouseInput();
    }
    void InitInventory()
    {
        //챕터 정보 저장하기
        int chapterCount = Enum.GetValues(typeof(EventManagerYKM.ChapterInfo)).Length;
        InitChapter();
        chapterInventories.Clear();
        
        for (int i = 0; i < chapterCount; i++)
        {
            chapterInventories.Add(i,new ChapterInventory());
        }
    }

    void InitChapter()
    {
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
        
        UpdateInventoryUI();
        
        //맵에서 증거 오브젝트 파괴하기 - 현재 오류 존재. 다시 씬으로 이동해오면 원상복구 됨. 아예 삭제 해 버려야 함.
        if (PlayerInteract.Instance._evidenceGameObject != null)
        {
            Debug.Log("맵에서 습득한 오브젝트 파괴");
            PlayerInteract.Instance._interactionMark.SetActive(false);
            Destroy(PlayerInteract.Instance._evidenceGameObject);
        }
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
            slot.GetComponent<InventorySlotInfo>().evidenceId = evidence.evidenceId;
            UpdateSlotUI(slot, evidence);
        }

        // 정신 세계 증거물 추가
        foreach (var evidence in currentInventory.mentalWorldEvidences)
        {
            GameObject slot = Instantiate(_inventorySlotPrefab, _mentalWorldInventory.transform);
            slot.GetComponent<InventorySlotInfo>().evidenceId = evidence.evidenceId;
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
        Debug.Log(evidence_id + "획득검사");
        foreach (var chapter in chapterInventories)
        {
            foreach (var slot in chapter.Value.realWorldEvidences)
            {
                if (slot.evidenceId == evidence_id)
                {
                    return true;
                }
            }
            foreach (var slot in chapter.Value.mentalWorldEvidences)
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
