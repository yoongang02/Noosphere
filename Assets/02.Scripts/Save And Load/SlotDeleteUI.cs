using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotDeleteUI : DefaultUIBase
{
    [SerializeField] private GameObject firstSelectable;
    public override void OnOpen()
    {
        base.OnOpen();
        InitDefaultBtnState();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();
    }

    void InitDefaultBtnState()
    {
        EventSystem.current.SetSelectedGameObject(null); // 먼저 비우고
        EventSystem.current.SetSelectedGameObject(firstSelectable); // 새로 지정
    }

    public void ClickYesBtn()
    {
        int index = NooSphere.SaveManager.Instance.selectSlotIndex;
        // 슬롯 데이터 삭제
        NooSphere.SaveManager.Instance.DeleteSlotData(index);
        // UI 닫기
        ClickNoBtn();
        // 슬롯 UI 업데이트하기
        FindAnyObjectByType<ResumeUI>().UpdateResumeUI();
    }

    public void ClickNoBtn()
    {
        DefaultUIController.Instance.CloseTopUI();
    }
}
