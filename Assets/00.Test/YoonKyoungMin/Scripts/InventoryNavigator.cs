using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VHierarchy.Libs;
using Cysharp.Threading.Tasks;

public class InventoryNavigator : MonoBehaviour
{
    [SerializeField] private GameObject _curSelectedSlot;
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private Sprite _deselectedSprite;
    [SerializeField] private TextMeshProUGUI _slotUseBtn;
    
    public List<GameObject> inventorySlots = new List<GameObject>();
    private List<GameObject> realWorldSlots = new List<GameObject>();
    private List<GameObject> mentalWorldSlots = new List<GameObject>();
    private int currentIndex = 0;
    private bool isBothInventory = false;
    public bool canEvidenceUse = false;
    [SerializeField] private string _curUseEventID;


    void Update()
    {
        if (InventoryManager.Instance.isInventoryOpen && !UIManager.Instance._isDetailOpen)
        {
            if (_curSelectedSlot != null)
            {
                //wasd 키를 통해 슬롯 상하좌우 이동
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
                //E 버튼을 누르면 상세 정보 열기
                if (Input.GetKeyDown(KeyCode.E))
                {
                    string id = _curSelectedSlot.GetComponent<InventorySlotInfo>().evidence_id;
                    EvidenceStructure evidence = DataManager.Instance._evidences[id];
                    UIManager.Instance.ShowDetailEvidenceInInventory(evidence);
                }
                //Space 버튼을 누르면 증거물 사용하기
                if (canEvidenceUse && Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("아이템 사용하기");
                    if (!EventManagerYKM.Instance.isGetDiary)
                    {
                        DialogueTextON.Instance.ShowSimpleText("(철컥) 열렸다!");
                        EventManagerYKM.Instance.isGetDiary = true;
                        canEvidenceUse = false;
                        UIManager.Instance.OpenInvestigateUI(DataManager.Instance._evidences["evidence_007"]);
                        //인벤토리 창 자동으로 닫기
                        InventoryManager.Instance.isInventoryOpen = !InventoryManager.Instance.isInventoryOpen;
                        InventoryManager.Instance.ControlWindow();
                    }
                }
            }
            
            //좌우 화살표 클릭 시, 페이지 넘김
            
            /*
            //숫자 키 클릭 시, 챕터 넘김
            for (int i = 0; i <= 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    int inputIndex = i - 1;
                    if (inputIndex >= 0 && inputIndex < InventoryManager.Instance.selectedChapterUIList.Count)
                    {
                        InventoryManager.Instance.currentViewChapter = inputIndex;
                        SetChapterSelected(inputIndex);
                        InventoryManager.Instance.UpdateInventoryUI();
                        UpdateSelection();
                    }
                }
            }
            */
        }

        if (UIManager.Instance._isDetailOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.CloseDetailEvidence();
        }
    }

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

        int chapterIndex = InventoryManager.Instance.currentViewChapter;
        SetChapterSelected(chapterIndex);
        
        _curSelectedSlot = inventorySlots[0];
        SetSlotSelected(_curSelectedSlot);
        SetSlotUseBtn(_curSelectedSlot);
    }

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

    List<GameObject> GetChildSlots(Transform parent)
    {
        List<GameObject> childSlots = new List<GameObject>();
        foreach (Transform child in parent)
        {
            childSlots.Add(child.gameObject);
        }
        return childSlots;
    }
    
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

    public void MoveLeft()
    {
        currentIndex -= 1;
        if (currentIndex < 0) currentIndex = inventorySlots.Count - 1;
        UpdateSelection();
    }

    public void MoveRight()
    {
        currentIndex += 1;
        if (currentIndex >= inventorySlots.Count) currentIndex = 0;
        UpdateSelection();
    }

    private void UpdateSelection()
    {
        _curSelectedSlot = inventorySlots[currentIndex];
        SetSlotSelected(_curSelectedSlot);
        SetSlotUseBtn(_curSelectedSlot);
    }

    private void SetSlotSelected(GameObject slot)
    {
        Image slotImg = slot.GetComponent<Image>();
        slotImg.sprite = _selectedSprite;
        foreach (var _slot in inventorySlots)
        {
            if (_slot != slot) _slot.GetComponent<Image>().sprite = _deselectedSprite;
        }
    }

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
    }

    private bool IsInRealWorldSlot()
    {
        if (realWorldSlots.Contains(_curSelectedSlot))
        {
            return true;
        }

        return false;
    }

    void SetSlotUseBtn(GameObject slot)
    {
        string evidenceId = slot.GetComponent<InventorySlotInfo>().evidence_id;
        string canUse = DataManager.Instance._evidences[evidenceId].can_Use;
        
        if (canUse == "Y")
        {
            //플레이어가 현재 상황에서 사용할 수 있는 증거물 아이디를 가져오기
            PlayerInteract playerInteract = PlayerController.Instance.GetComponent<PlayerInteract>();
            //현재 선택한 슬롯의 증거물 아이디 가져오기
            string slotId = _curSelectedSlot.GetComponent<InventorySlotInfo>().evidence_id;
            
            foreach (var key in playerInteract.GetPlayeCanUseEvidenceID().Keys)
            {
                //두 증거물 아이디 값 비교하기, 같으면 사용할 수 있음.
                if (slotId == playerInteract.GetPlayeCanUseEvidenceID()[key])
                {
                    _slotUseBtn.color = UnityExtension.HexColor(UIManager.BlackColor);
                    canEvidenceUse = true;
                    return;
                }
            }
        }
        _slotUseBtn.color = UnityExtension.HexColor("#B3B3B3");
        canEvidenceUse = false;
    }
}
