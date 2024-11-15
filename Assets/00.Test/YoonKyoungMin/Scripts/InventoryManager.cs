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
    private bool isInventoryOpen = false;
    
    //인벤토리 UI
    [SerializeField] private GameObject _inventoryWindow;
    [SerializeField] private GameObject _realWorldInventory;
    [SerializeField] private GameObject _mentalWorldInventory;
    [SerializeField] private GameObject _inventorySlotPrefab;
    private int _currentViewChapter = 0;
    private int _inventoryMoveSpacingLR = 1;
    private int _inventoryMoveSpacingUD = 1;
    private int _inventoryMoveCurIndex = 0;
    [SerializeField] List<GameObject> _inventorySlots = new List<GameObject>(); //인벤토리 슬롯 커서를 위해, 한 페이지 내의 슬롯을 모두 관리하는 리스트
    [SerializeField] private GameObject _curSelectedSlot;
    [SerializeField] private Sprite _deselectedSlotBg;
    [SerializeField] private Sprite _selectedSlotBg;
    [SerializeField] private int _curSlotIndex = 0;

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
        
        //인벤토리 UI 조작
        if (isInventoryOpen)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                SetSlotSelected(CalculateSlotIndex(-_inventoryMoveSpacingUD));
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                SetSlotSelected(CalculateSlotIndex(-_inventoryMoveSpacingLR));
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                SetSlotSelected(CalculateSlotIndex(+_inventoryMoveSpacingUD));
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                SetSlotSelected(CalculateSlotIndex(+_inventoryMoveSpacingLR));
            }

            //숫자 키 누르면

            //왼쪽 화살표 (왼쪽 페이지)

            //오른쪽 화살표 (오른쪽 페이지)
        }
    }

    void InitInventory()
    {
        isInventoryOpen = false;
        _currentViewChapter = 0;
        chapterInventories.Clear();
        
        int chapterCnt = Enum.GetValues(typeof(EventManagerYKM.ChapterInfo)).Length;
        for (int i = 0; i < chapterCnt; i++)
        {
            chapterInventories.Add(i,new ChapterInventory());
        }
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
    void UpdateInventoryUI()
    {
        ClearInventoryUI(_realWorldInventory);
        ClearInventoryUI(_mentalWorldInventory);
        
        ChapterInventory currentInventory = chapterInventories[_currentViewChapter];

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
        if(isInventoryOpen) InitInventorySlotCursor();
    }
    
    //인벤토리 슬롯 UI
    void InitInventorySlotCursor()
    {
        //초기화
        _curSelectedSlot = null;
        _inventorySlots.Clear();
        _inventoryMoveSpacingLR = 1;
        _inventoryMoveSpacingUD = 3;
        //현실세계 슬롯
        List<GameObject> realWorldSlots = GetChildSlots(_realWorldInventory.transform);
        //가상세계 슬롯
        List<GameObject> mentalWorldSlots = GetChildSlots(_mentalWorldInventory.transform);
        
        //현실세계 증거물와 정신세계 증거물이 둘 다 있는 경우
        if (realWorldSlots.Count > 0 && mentalWorldSlots.Count > 0)
        {
            _inventoryMoveSpacingUD = 6;
            //현실 세계 증거물과 정신세계 증거물 중에 더 많은 행이 무엇인지 파악
            int maxRow = (int)Mathf.Max(realWorldSlots.Count,
                mentalWorldSlots.Count) / 3;
            
            for (int row = 0; row < maxRow; row++)
            {
                int index = row * 3;
                AddRowToList(realWorldSlots,index);
                AddRowToList(mentalWorldSlots,index);
            }
        }
        else if(realWorldSlots.Count > 0)
        {
            foreach (var slot in realWorldSlots)
            {
                _inventorySlots.Add(slot);
            }
        }
        else if (mentalWorldSlots.Count > 0)
        {
            foreach (var slot in mentalWorldSlots)
            {
                _inventorySlots.Add(slot);
            }
        }
        
        //슬롯이 비어있지 않으면 슬롯 리스트의 첫번째 인덱스가 기본 디폴트 선택
        if (_inventorySlots.Count != 0) SetSlotSelected(0);
    }
    
    List<GameObject> GetChildSlots(Transform parent)
    {
        List<GameObject> childSlots = new List<GameObject>();
        foreach (Transform child in parent)
        {
            childSlots.Add(child.gameObject);
            Debug.Log(child.name + "getChildSlot");
        }
        return childSlots;
    }
    
    void AddRowToList(List<GameObject> slotList, int row)
    {
        int startIndex = row * 3; // 해당 행의 시작 인덱스
        int endIndex = Mathf.Min(startIndex + 3, slotList.Count); // 해당 행의 끝 인덱스

        for (int i = startIndex; i < endIndex; i++)
        {
            _inventorySlots.Add(slotList[i]);
        }
    }

    void SetSlotSelected(int index)
    {
        GameObject slot = _inventorySlots[index];
        _curSelectedSlot = slot;
        Image slotBackground = slot.GetComponent<Image>();
        slotBackground.sprite = _selectedSlotBg;
        foreach (var _slot in _inventorySlots)
        {
            if (_slot != slot)
            {
                slot.GetComponent<Image>().sprite = _deselectedSlotBg;
            }
        }
    }

    int CalculateSlotIndex(int value)
    {
        _curSlotIndex += value;
        //1번 슬롯에서 왼쪽으로 이동하는 경우 가장 마지막 슬롯으로 이동
        if (_curSlotIndex == -1)
        {
            _curSlotIndex = _inventorySlots.Count - 1;
        }
        else if (_curSlotIndex == _inventorySlots.Count) //가장 마지막 슬롯에서 오른쪽으로 이동하는 경우, 가장 첫 슬롯으로 이동
        {
            _curSlotIndex = 0;
        }

        if (_curSlotIndex < -1) //위로 이동했는데 슬롯이 없다면, 가장 마지막 행의 젤 왼쪽 슬롯으로 이동
        {
            //마지막행의 가장 왼쪽 인덱스 값 구하기
            _curSlotIndex = ((int)_inventorySlots.Count / _inventoryMoveSpacingUD) * _inventoryMoveSpacingUD;
        }
        else if (_curSlotIndex > _inventorySlots.Count) //아래로 이동했는데, 슬롯이 없다면
        {
            //마지막 행에 위치한다면, 가장 첫번째 행의 젤 왼쪽 슬롯으로 이동
            //마지막 행이 아니라면, 가장 마지막 행의 젤 왼쪽 슬롯으로 이동
            int curRow = _curSlotIndex / _inventoryMoveSpacingUD;
            int endRow = _inventorySlots.Count / _inventoryMoveSpacingUD;

            if (curRow == endRow)
            {
                //마지막 행에 위치
                _curSlotIndex = 0;
            }
            else
            {
                //마지막 행이 위치
                _curSlotIndex *= _inventoryMoveSpacingUD;
            }
        }

        return _curSlotIndex;
    }
}
