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
        // 슬롯 번호 가져와서 반영하기
        int slotIndex= NooSphere.SaveManager.Instance.selectSlotIndex - 1;
        _titleText.text = $"슬롯 {slotIndex}에 데이터를 저장하시겠습니까?";
        _slotTitle.text = $"슬롯 {slotIndex}";

        // 슬롯 정보 가져오기
        _slotLocationInfo.text = "저장 위치 : " + NooSphere.SaveManager.Instance.GetCurrentLocation();
        _slotPlaytimeInfo.text = "플레이 타임 : " + NooSphere.SaveManager.Instance.GetCurrentPlayTime();
        _slotDateInfo.text = "저장 일시 : " + NooSphere.SaveManager.Instance.GetCurrentDateTime();
    }

    public void ClickYesBtn()
    {
        // UI 닫기
        ClickNoBtn();
        DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.slotCompleteUI);
    }

    public void ClickNoBtn()
    {
        DefaultUIController.Instance.CloseTopUI();
    }
}
