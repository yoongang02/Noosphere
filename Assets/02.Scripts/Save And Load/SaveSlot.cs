using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveSlot : UIKeyboardNavigator, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private KeyboardNavigationController navigationController;
    private SaveUI saveUI;
    void Start()
    {
        saveUI = GetComponentInParent<SaveUI>();
        navigationController = GetComponentInParent<KeyboardNavigationController>();

        if (saveUI == null)
        {
            Debug.LogError("SaveUI가 부모 오브젝트에 부착되어 있지 않습니다.");
        }

        if(navigationController == null)
        {
            Debug.LogError("KeyboardNavigationController가 부모 오브젝트에 부착되어 있지 않습니다.");
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 1)
        {
            saveUI.ClickSaveSlot(slotIndex);
        }
        else if (eventData.clickCount == 2)
        {
            saveUI.DoubleClickSaveSlot(slotIndex);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        navigationController.curHoveredNavigator = this;
        saveUI.HoverEnterSaveSlot(slotIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        saveUI.UpdateAllSlotState();
    }
}
