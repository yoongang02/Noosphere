using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResumeUI : MonoBehaviour
{
    [SerializeField] private List<Image> slotContents = new List<Image>();
    [SerializeField] private List<Image> slotBackgrounds = new List<Image>();
    [SerializeField] private List<TextMeshProUGUI> slotEmptyTexts = new List<TextMeshProUGUI>();

    [System.Serializable]
    public struct SlotInfoTexts
    {
        public TextMeshProUGUI locationText;
        public TextMeshProUGUI playTimeText;
        public TextMeshProUGUI dateTimeText;
    }
    [SerializeField] private List<SlotInfoTexts> slotInfoTexts = new List<SlotInfoTexts>();
    [SerializeField] private List<Sprite> slotContentSprites = new List<Sprite>(); // 0 디폴트, 1 호버
    

    private void Start()
    {
        // 슬롯 상태 초기화
        InitSlotState();
    }

    // 슬롯 상태에 따라 문구를 설정하는 함수
    void SetSlotText(int slotIndex)
    {
        int listIndex = slotIndex - 1;
        bool hasData = NooSphere.SaveManager.Instance.HasSaveData(slotIndex);

        if (hasData)
        {
            slotInfoTexts[listIndex].locationText.text = "저장 위치 : " + NooSphere.SaveManager.Instance.GetLocationData(slotIndex);
            slotInfoTexts[listIndex].playTimeText.text = "플레이 타임 : " + NooSphere.SaveManager.Instance.GetPlayTimeData(slotIndex);
            slotInfoTexts[listIndex].dateTimeText.text = "저장 일시 : " + NooSphere.SaveManager.Instance.GetDateTimeData(slotIndex);
        }

        slotInfoTexts[listIndex].locationText.gameObject.SetActive(hasData);
        slotInfoTexts[listIndex].playTimeText.gameObject.SetActive(hasData);
        slotInfoTexts[listIndex].dateTimeText.gameObject.SetActive(hasData);
        slotEmptyTexts[listIndex].gameObject.SetActive(!hasData);

    }

    // 슬롯 배경 설정 함수
    void SetSlotBackgroundOpacity(int slotIndex, float opacity)
    {
        Color _color = slotBackgrounds[slotIndex-1].color;
        
        _color.a = opacity;
        slotBackgrounds[slotIndex-1].color = _color;
    }

    // 슬롯을 한번 클릭하였을 때 함수
    // 이 함수가 호출되었다는 것은 이미 클릭을 할 수 있는 슬롯임이 검증된 것.
    // TODO : 버튼 업데이트 함수와 연동해야 함.
    public void ClickLoadSlot(int slotIndex)
    {
        for (int i = 1; i <= 3; i++)
        {
            if (slotIndex == i)
            {
                NooSphere.SaveManager.Instance.SetSlotIndex(slotIndex);
                SetSlotBackgroundOpacity(slotIndex, 1);
            }
            else
            {
                SetSlotBackgroundOpacity(i, 0);
            }
            slotContents[i - 1].sprite = slotContentSprites[0];
        }
    }

    // 슬롯에 호버 진입했을 때
    public void HoverEnterLoadSlot(int slotIndex)
    {
        for(int i=1; i<=3; i++)
        {
            if (slotIndex == i)
            {
                if (NooSphere.SaveManager.Instance.selectSlotIndex == i) continue;
                slotContents[slotIndex - 1].sprite = slotContentSprites[1];
            }
            else
            {
                if (NooSphere.SaveManager.Instance.selectSlotIndex == i) continue;
                slotContents[i-1].sprite = slotContentSprites[0];
            }
        }
    }

    // 슬롯을 더블 클릭하였을 때 = 게임 시작
    // TODO : 해당 슬롯 데이터를 바탕으로 로딩 씬에서 로드하는 것과 연결
    public void DoubleClickLoadSlot(int slotIndex)
    {

    }

    // 슬롯 상태 초기화 함수
    // auto 슬롯이 선택되어 있는 기본 상태
    void InitSlotState()
    {
        bool hasData = NooSphere.SaveManager.Instance.HasSaveData(1);
        if (hasData) NooSphere.SaveManager.Instance.SetSlotIndex(1);

        UpdateAllSlotState();
    }

    // 이어하기 UI 업데이트 할 때 사용하는 함수
    // TODO : 버튼 업데이트 연동해야 함.
    public void UpdateAllSlotState()
    {
        int curSelectedSlotIndex = NooSphere.SaveManager.Instance.selectSlotIndex;

        // 슬롯 문구 업데이트
        SetSlotText(1);
        SetSlotText(2);
        SetSlotText(3);

        // 슬롯 배경 업데이트
        ClickLoadSlot(curSelectedSlotIndex);
    }

    // 슬롯 삭제할 경우

    // 돌아가기 버튼 클릭할 경우 혹은 ESC 클릭할 경우
}
