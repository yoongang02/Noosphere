using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotSaveUI : DefaultUIBase
{
    [SerializeField] private GameObject firstSelectable;

    [Header("슬롯 관련")]
    [Space(5)]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _slotTitle;
    [SerializeField] private TextMeshProUGUI _slotLocationInfo;
    [SerializeField] private TextMeshProUGUI _slotPlaytimeInfo;
    [SerializeField] private TextMeshProUGUI _slotDateInfo;
    public override void OnOpen()
    {
        base.OnOpen();
        InitSlotInfo();
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

    void InitSlotInfo()
    {

    }

    public void ClickYesBtn()
    {
        int index = NooSphere.SaveManager.Instance.selectSlotIndex;
        // 슬롯 데이터 삭제
        StartCoroutine(NooSphere.SaveManager.Instance.DoSave());
        // UI 닫기
        ClickNoBtn();
        // 슬롯 UI 업데이트하기
        FindAnyObjectByType<SaveUI>().UpdateSaveUI();
    }

    public void ClickNoBtn()
    {
        DefaultUIController.Instance.CloseTopUI();
    }
}
