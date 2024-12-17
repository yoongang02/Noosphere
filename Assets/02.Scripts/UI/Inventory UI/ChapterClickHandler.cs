using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChapterClickHandler : InventoryNavigator, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject _lastEnteredObject;
    public void OnPointerClick(PointerEventData eventData)
    {
        //젤 위에 있는 UI 아니면 작동 X
        if (!InventoryManager.Instance.IsTopUI())
        {
            return;
        }
        
        //클릭한 오브젝트가 챕터인지 파악
        GameObject clickedObject = eventData.pointerClick;
        if (InventoryManager.Instance.deselectedChapterUIList.Contains(clickedObject))
        {
            int chapterIndex = InventoryManager.Instance.deselectedChapterUIList.IndexOf(clickedObject);
            SetChapterSelected(chapterIndex);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //젤 위에 있는 UI 아니면 작동 X
        if (!InventoryManager.Instance.IsTopUI())
        {
            return;
        }
        
        //클릭한 오브젝트가 챕터인지 파악
        GameObject enteredObject = eventData.pointerEnter;
        if (InventoryManager.Instance.deselectedChapterUIList.Contains(enteredObject))
        {
            _lastEnteredObject = enteredObject;
            
            int chapterIndex = InventoryManager.Instance.deselectedChapterUIList.IndexOf(enteredObject);
            HoverEnterOnChapter(chapterIndex);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //젤 위에 있는 UI 아니면 작동 X
        if (!InventoryManager.Instance.IsTopUI())
        {
            return;
        }
        
        //클릭한 오브젝트가 챕터인지 파악
        if (_lastEnteredObject != null)
        {
            HoverExitOnChapter();
            _lastEnteredObject = null;
        }
    }
}
