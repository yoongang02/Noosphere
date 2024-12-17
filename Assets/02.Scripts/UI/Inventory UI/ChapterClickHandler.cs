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
        
        Debug.Log($"챕터 클릭 {clickedObject.name}");
        
        if (InventoryManager.Instance.selectedChapterUIList.Contains(clickedObject))
        {
            int chapterIndex = InventoryManager.Instance.selectedChapterUIList.IndexOf(clickedObject);
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
        int curChapterIndex = InventoryManager.Instance.currentViewChapter;
        
        GameObject selectedChapter = InventoryManager.Instance.selectedChapterUIList[curChapterIndex];
        GameObject deselectedChapter = InventoryManager.Instance.deselectedChapterUIList[curChapterIndex];
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

        _lastEnteredObject = null;
    }
}
