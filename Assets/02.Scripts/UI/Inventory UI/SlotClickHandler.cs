using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotClickHandler : InventoryNavigator, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        //젤 위에 있는 UI 아니면 작동 X
        if (!InventoryManager.Instance.IsTopUI())
        {
            return;
        }
        
        GameObject enteredObject = eventData.pointerEnter;
        if (inventorySlots.Contains(enteredObject))
        {
            SetSlotSelected(enteredObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //젤 위에 있는 UI 아니면 작동 X
        if (!InventoryManager.Instance.IsTopUI())
        {
            return;
        }
        
        GameObject exitedObject = eventData.pointerEnter;
        
        if (exitedObject != null)
        {
            SetSlotDeselected(exitedObject);
        }
    }
}
