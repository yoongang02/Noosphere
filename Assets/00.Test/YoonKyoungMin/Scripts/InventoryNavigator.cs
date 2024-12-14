using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryNavigator : MonoBehaviour, IPointerClickHandler
{
    [Header("인벤토리 네비게이션 정보")]
    [SerializeField] private GameObject _curSelectedSlot;
    public int currentIndex = 0;
    public bool isBothInventory = false; //정신세계 증거물과 현실세계 증거물이 모두 있을 때
    public bool canEvidenceUse = false;
    
    [Space(5)][Header("인벤토리 네비게이션 UI")]
    public List<GameObject> inventorySlots = new List<GameObject>();
    private List<GameObject> realWorldSlots = new List<GameObject>();
    private List<GameObject> mentalWorldSlots = new List<GameObject>();
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private Sprite _deselectedSprite;
    [SerializeField] private TextMeshProUGUI _slotUseBtn;
    
    public void InitNavigator(Transform realWorld, Transform mentalWorld)
    {
        //초기화
        realWorldSlots.Clear();
        mentalWorldSlots.Clear();
        inventorySlots.Clear();
        
        //현재 인벤토리 기준으로 재설정
        realWorldSlots = GetChildSlots(realWorld);
        mentalWorldSlots = GetChildSlots(mentalWorld);
        
        //실제 이동에 사용할 슬롯 리스트
        if (realWorldSlots.Count > 0 && mentalWorldSlots.Count > 0)
        {
            //현실세계 증거물과 정신세계 증거물을 교차로 넣기
            AddSlotsPerRow();
            isBothInventory = true;
        }
        else if(realWorldSlots.Count > 0)
        {
            inventorySlots = realWorldSlots;
        }
        else if (mentalWorldSlots.Count > 0)
        {
            inventorySlots = mentalWorldSlots;
        }

        if (inventorySlots.Count == 0)
        {
            _curSelectedSlot = null;
            return;
        }

        //챕터 및 선택 초기화
        int chapterIndex = InventoryManager.Instance.currentViewChapter;
        SetChapterSelected(chapterIndex);
        currentIndex = 0;
        UpdateSelection();
    }

    //증거물 행 별로 슬롯에 추가 함수
    void AddSlotsPerRow()
    {
        int maxCount = (int)Mathf.Max(realWorldSlots.Count,mentalWorldSlots.Count);
        int maxRow = Mathf.CeilToInt((float)maxCount / 3);

        for (int row = 0; row < maxRow; row++)
        {
            int startIndex = row * 3;
            int endIndexR = Mathf.Min(startIndex + 3, realWorldSlots.Count);
            int endIndexM = Mathf.Min(startIndex + 3, mentalWorldSlots.Count);
            for (int index = startIndex; index < endIndexR; index++)
            {
                inventorySlots.Add(realWorldSlots[index]);
            }
            for (int index = startIndex; index < endIndexM; index++)
            {
                inventorySlots.Add(mentalWorldSlots[index]);
            }
        }
    }

    //슬롯들 가져오기
    List<GameObject> GetChildSlots(Transform parent)
    {
        List<GameObject> childSlots = new List<GameObject>();
        foreach (Transform child in parent)
        {
            childSlots.Add(child.gameObject);
        }
        return childSlots;
    }
    
    //현재 슬롯에서 위로 이동 - 정신(현실) 세계 증거물만 있는 경우
    public void MoveUp()
    {
        if (isBothInventory)
        {
            //현실 세계에 있다면
            if (IsInRealWorldSlot())
            {
                MoveUpInBothInventory(realWorldSlots);
            }
            else
            {
                MoveUpInBothInventory(mentalWorldSlots);
            }
            
        }
        else
        {
            currentIndex -= 3;
            if(currentIndex < 0) currentIndex = Mathf.FloorToInt((inventorySlots.Count - 1) / 3) * 3;
        }
        UpdateSelection();
    }

    //현재 슬롯에서 위로 이동 - 정신과 현실 세계 증거물이 모두 있는 경우
    private void MoveUpInBothInventory(List<GameObject> slots)
    {
        int index = slots.IndexOf(_curSelectedSlot);
        int targetIndex = index - 3;

        if (targetIndex < 0)
        {
            targetIndex = Mathf.FloorToInt((slots.Count - 1) / 3) * 3;
        }
        
        GameObject target = slots[targetIndex];
        currentIndex = inventorySlots.IndexOf(target);
    }

    //현재 슬롯에서 아래로 이동 - 정신과 현실 세계 증거물이 모두 있는 경우
    private void MoveDownInBothInventory(List<GameObject> slots)
    {
        int index = slots.IndexOf(_curSelectedSlot);
        int targetIndex = index + 3;
        
        if (targetIndex >= slots.Count)
        {
            int currentRow = targetIndex / 3;
            int endRow = Mathf.FloorToInt((slots.Count - 1) / 3);
            if (currentRow == endRow) targetIndex = endRow * 3;
            else if (currentRow > endRow) targetIndex = 0;
        }
        GameObject target = slots[targetIndex];
        currentIndex = inventorySlots.IndexOf(target);
    }
    
    //현재 슬롯에서 아래로 이동 - 정신(현실) 세계 증거물만 있는 경우
    public void MoveDown()
    {
        if (isBothInventory)
        {
            if (IsInRealWorldSlot())
            {
                MoveDownInBothInventory(realWorldSlots);
            }
            else
            {
                MoveDownInBothInventory(mentalWorldSlots);
            }
        }
        else
        {
            currentIndex += 3;
            if (currentIndex >= inventorySlots.Count)
            {
                int currentRow = currentIndex / 3;
                int endRow = Mathf.FloorToInt((inventorySlots.Count - 1) / 3);
                if (currentRow == endRow) currentIndex = endRow * 3;
                else if (currentRow > endRow) currentIndex = 0;
            }
        }
        UpdateSelection();
    }

    //현재 슬롯에서 왼쪽으로 이동
    public void MoveLeft()
    {
        currentIndex -= 1;
        if (currentIndex < 0) currentIndex = inventorySlots.Count - 1;
        UpdateSelection();
    }

    //현재 슬롯에서 오른쪽으로 이동
    public void MoveRight()
    {
        currentIndex += 1;
        if (currentIndex >= inventorySlots.Count) currentIndex = 0;
        UpdateSelection();
    }

    //슬롯 선택 시, 슬롯 선택에 따른 업데이트
    private void UpdateSelection()
    {
        _curSelectedSlot = inventorySlots[currentIndex];
        SetSlotSelected(_curSelectedSlot);
        SetSlotUseBtn(_curSelectedSlot);
    }

    //슬롯 배경 업데이트
    private void SetSlotSelected(GameObject slot)
    {
        Image slotImg = slot.GetComponent<Image>();
        slotImg.sprite = _selectedSprite;
        foreach (var _slot in inventorySlots)
        {
            if (_slot != slot) _slot.GetComponent<Image>().sprite = _deselectedSprite;
        }
    }

    //챕터 선택
    private void SetChapterSelected(int index)
    {
        GameObject selectedChapter = InventoryManager.Instance.selectedChapterUIList[index];
        GameObject deselectedChapter = InventoryManager.Instance.deselectedChapterUIList[index];
        selectedChapter.SetActive(true);
        deselectedChapter.SetActive(false);

        foreach (var _chapter in InventoryManager.Instance.selectedChapterUIList)
        {
            if(_chapter != selectedChapter) _chapter.SetActive(false);
        }
        foreach (var _chapter in InventoryManager.Instance.deselectedChapterUIList)
        {
            if(_chapter != deselectedChapter) _chapter.SetActive(true);
        }
        
        //인벤토리 업데이트 하기
        InventoryManager.Instance.UpdateInventoryUI();
    }

    //현실 세계 항목에 있는 슷롯인지 확인
    private bool IsInRealWorldSlot()
    {
        if (realWorldSlots.Contains(_curSelectedSlot))
        {
            return true;
        }

        return false;
    }

    //슬롯 버튼 업데이트
    void SetSlotUseBtn(GameObject slot)
    {
        string evidenceId = slot.GetComponent<InventorySlotInfo>().evidenceId;
        char canUse = DataManager.Instance._evidences[evidenceId].canUse;
        
        if (canUse == 'Y')
        {
            //플레이어가 현재 상황에서 사용할 수 있는 증거물 아이디를 가져오기
            PlayerInteract playerInteract = PlayerController.Instance.GetComponent<PlayerInteract>();
            //현재 선택한 슬롯의 증거물 아이디 가져오기
            string slotId = _curSelectedSlot.GetComponent<InventorySlotInfo>().evidenceId;
            
            foreach (var key in playerInteract.GetPlayeCanUseEvidenceID().Keys)
            {
                /*
                //두 증거물 아이디 값 비교하기, 같으면 사용할 수 있음.
                if (slotId == playerInteract.GetPlayeCanUseEvidenceID()[key])
                {
                    _slotUseBtn.color = UnityExtension.HexColor(UIManager.BlackColor);
                    canEvidenceUse = true;
                    return;
                }
                */
            }
        }
        _slotUseBtn.color = UnityExtension.HexColor("#B3B3B3");
        canEvidenceUse = false;
    }

    //인벤토리 네비게이션 - 키보드 입력
    public void HandleKeyboardInput()
    {
        if (_curSelectedSlot != null)
        {
            //슬롯 상하좌우 이동 - 키보드 WASD
            if (Input.GetKeyDown(KeyCode.W))
            {
                MoveUp();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                MoveLeft();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                MoveDown();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                MoveRight();
            }
                
            //증거물 상세 정보 열기 - 키보드 E
            if (Input.GetKeyDown(KeyCode.E))
            { 
                OpenEvidenceDetailUI();
            }
            
            //Space 버튼을 누르면 증거물 사용하기
            if (canEvidenceUse && Input.GetKeyDown(KeyCode.Space))
            {
                UseEvidence();
            }
        }
    }
    
    //마우스 클릭 이벤트 관리
    public void OnPointerClick(PointerEventData eventData)
    {
        //젤 위에 있는 UI 아니면 작동 X
        if (!InventoryManager.Instance.IsTopUI())
        {
            return;
        }
        
        //클릭한 오브젝트가 슬롯인지 파악
        GameObject clickedObject = eventData.pointerClick;
        if (inventorySlots.Contains(clickedObject))
        {
            //인덱스 값 가져오기
            currentIndex = inventorySlots.IndexOf(clickedObject);
            
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (_curSelectedSlot != clickedObject)
                {
                    //현재 선택된 오브젝트와 클릭한 오브젝트가 다를 경우 -> 신규 선택
                    UpdateSelection();
                }
                else
                {
                    //현재 선택된 오브젝트와 클릭한 오브젝트가 같을 경우 -> 상세 보기 기능
                    OpenEvidenceDetailUI();
                }
            }

            // 우클릭 감지
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                //현재 선택된 오브젝트가 우클릭한 오브젝트와 같아야 함. -> 사용하기 기능
                if (_curSelectedSlot == clickedObject)
                {
                    UseEvidence();
                }
            }
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($"{clickedObject} 가 클릭 됨");
        }
    }

    //증거물 상세 내용 UI 열기
    void OpenEvidenceDetailUI()
    {
        string id = _curSelectedSlot.GetComponent<InventorySlotInfo>().evidenceId;
        EvidenceStructure evidence = DataManager.Instance._evidences[id];
        UIManager.Instance.OpenUI(UIManager.Instance.evidenceDetailUI,evidence);
    }

    //증거물 사용하기
    void UseEvidence()
    {
        
    }
}
