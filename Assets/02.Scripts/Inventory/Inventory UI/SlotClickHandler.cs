using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = NooSphere.Debug;
public class SlotClickHandler : InventoryNavigator, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject _lastEnteredObject;
    public void OnPointerClick(PointerEventData eventData)
    {
        //젤 위에 있는 UI 아니면 작동 X
        if (!UIManager.Instance.IsUIOpen(UIManager.Instance.inventoryUI))
        {
            return;
        }
        
        //클릭한 오브젝트가 슬롯인지 파악
        GameObject clickedObject = eventData.pointerClick;
        Debug.Log($"{clickedObject.gameObject.name} 클릭");
        if (inventorySlots.Contains(clickedObject))
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                //인덱스 값 가져오기
                currentIndex = inventorySlots.IndexOf(clickedObject);
                if (_curSelectedSlot != clickedObject)
                {
                    //현재 선택된 오브젝트와 클릭한 오브젝트가 다를 경우 -> 신규 선택
                    Debug.Log("슬롯 클릭");
                    UpdateSelection();
                }
                else
                {
                    //현재 선택된 오브젝트와 클릭한 오브젝트가 같을 경우 -> 상세 보기 기능
                    Debug.Log("슬롯 선택");
                    OpenEvidenceDetailUI();
                }
            }

            // 우클릭 감지
            if (canEvidenceUse && eventData.button == PointerEventData.InputButton.Right)
            {
                //현재 선택된 오브젝트가 우클릭한 오브젝트와 같아야 함. -> 사용하기 기능
                if (_curSelectedSlot == clickedObject)
                {
                    Debug.Log("아이템 사용");
                    UseEvidence();
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //젤 위에 있는 UI 아니면 작동 X
        if (!UIManager.Instance.IsUIOpen(UIManager.Instance.inventoryUI))
        {
            return;
        }
        
        GameObject enteredObject = eventData.pointerEnter;
        Debug.Log($"{enteredObject.gameObject.name} 호버");
        if (inventorySlots.Contains(enteredObject))
        {
            Debug.Log("슬롯 호버 진입");
            _lastEnteredObject = enteredObject;
            SetSlotSelected(enteredObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //젤 위에 있는 UI 아니면 작동 X
        if (!UIManager.Instance.IsUIOpen(UIManager.Instance.inventoryUI))
        {
            return;
        }
        
        if (_lastEnteredObject != null)
        {
            Debug.Log("슬롯 호버 끝");
            UpdateSelection();
            _lastEnteredObject = null;
        }
    }
}
