using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotOverwriteUI : DefaultUIBase
{
    [Header("슬롯 관련")]
    [Space(5)]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _slotTitle;
    [SerializeField] private TextMeshProUGUI _slotLocationInfo;
    [SerializeField] private TextMeshProUGUI _slotPlaytimeInfo;
    [SerializeField] private TextMeshProUGUI _slotDateInfo;
    [SerializeField] private Image _slotImage;
    [SerializeField] private List<Sprite> slotActiveSprites = new List<Sprite>();
    public override void OnOpen()
    {
        base.OnOpen();
        InitSlotInfo();
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

        if (InputRouter.Instance.ConsumeEscape())
        {
            DefaultUIController.Instance.CloseTopUI();
        }
    }

    void InitSlotInfo()
    {
        // 슬롯 번호 가져와서 반영하기
        int slotIndex = NooSphere.SaveManager.Instance.selectSlotIndex - 1;
        _titleText.text = $"슬롯 {slotIndex}에 데이터를 덮어쓰시겠습니까?";
        _slotTitle.text = $"슬롯 {slotIndex}";

        // 슬롯 정보 가져오기
        _slotLocationInfo.text = "저장 위치 : " + NooSphere.SaveManager.Instance.GetCurrentLocation();
        _slotPlaytimeInfo.text = "플레이 타임 : " + NooSphere.SaveManager.Instance.GetCurrentPlayTime();
        _slotDateInfo.text = "저장 일시 : " + NooSphere.SaveManager.Instance.GetCurrentDateTime();

        // 슬롯 이미지 설정
        SetSlotImage();
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

    private void SetSlotImage()
    {
        _slotImage.sprite = GetSlotSprite();
    }

    private Sprite GetSlotSprite()
    {
        if (FindObjectOfType<RoomInfoManager>() is RoomInfoManager roomInfoManager)
        {
            var location = roomInfoManager.roomName;

            switch (location)
            {
                case "Lounge":
                    return slotActiveSprites[0];
                case "R-101":
                    return slotActiveSprites[1];
                case "R-102":
                    return slotActiveSprites[2];
                case "R-103":
                    return slotActiveSprites[4];
                case "R-104":
                    return slotActiveSprites[3];
            }
        }

        return slotActiveSprites[0];
    }
}
