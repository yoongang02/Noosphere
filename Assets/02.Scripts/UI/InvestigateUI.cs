using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InvestigateUI : UIBase
{
    [Header("증거물 조사 UI")] [SerializeField] private EvidenceStructure _curEvidence;
    [SerializeField] private GameObject _curSelectedBtn;

    [Space(5)] [SerializeField] private TextMeshProUGUI _evidenceName;
    [SerializeField] private TextMeshProUGUI _evidenceDescription;
    [SerializeField] private Image _evidenceImage;
    [Space(5)]
    [SerializeField] private GameObject _yesBtn;
    [SerializeField] private GameObject _noBtn;

    private void OnEnable()
    {
        //Yes 버튼을 디폴트로 설정
        RemoveAllListeners();
        AddYesBtnEvent();
    }

    public override void OnOpen(EvidenceStructure evidence)
    {
        //조사하는 증거물에 대한 정보 반영
        SetInvestigateUI(evidence);
        _curEvidence = evidence;
        base.OnOpen();
    }

    public override void OnClose()
    {
        _curEvidence = null;
        base.OnClose();
        UIManager.Instance.OnSelectEnd?.Invoke();
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();
        //키보드 A - YES 버튼
        if (Input.GetKeyDown(KeyCode.A))
        {
            RemoveAllListeners();
            AddYesBtnEvent();
        }

        //키보드 D - NO 버튼
        if (Input.GetKeyDown(KeyCode.D))
        { 
            RemoveAllListeners();
            AddNoBtnEvent();
        }

        //스페이스 - 버튼 선택
        if (_curSelectedBtn != null && Input.GetKeyDown(KeyCode.Space))
        {
            //현재 등록되어 있는 OnClick 이벤트 실행
            OnClickEvent?.Invoke();
        }
            
        //ESC - NO 선택
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //NO 버튼 선택
            SelectNoBtn();
        }
    }

    public override void HandleMouseInput()
    {
        base.HandleMouseInput();
        
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject == _yesBtn)
            {
                AddYesBtnEvent();
                if (Input.GetMouseButtonDown(0)) 
                {
                    OnClickEvent?.Invoke();
                }
            }
            else if (result.gameObject == _noBtn)
            {
                AddNoBtnEvent();
                if (Input.GetMouseButtonDown(0)) 
                {
                    OnClickEvent?.Invoke();
                }
            }
        }
    }

    void AddYesBtnEvent()
    {
        AddOnHoverListener(SelectYesBtn);
        AddOnClickListener(UIManager.Instance.evidenceDetailUI.OnOpen);
        OnHoverEvent?.Invoke();
    }

    void AddNoBtnEvent()
    {
        AddOnHoverListener(SelectNoBtn);
        AddOnClickListener(OnClose);
        OnHoverEvent?.Invoke();
    }

    void SelectYesBtn()
    {
        SetButtonSelected(_yesBtn, UnityExtension.HexColor(GreenColor));
        SetButtonSelected(_noBtn, UnityExtension.HexColor(WhiteColor));
        _curSelectedBtn = _yesBtn;
    }

    void SelectNoBtn()
    {
        SetButtonSelected(_noBtn,UnityExtension.HexColor(GreenColor));
        SetButtonSelected(_yesBtn,UnityExtension.HexColor(WhiteColor));
        _curSelectedBtn = _noBtn;
    }
    
    //증거물 조사 UI의 정보 세팅하기
    public void SetInvestigateUI(EvidenceStructure evidence)
    {
        _evidenceName.text = evidence.evidenceName;
        _evidenceDescription.text = evidence.evidenceTextDisplay;
        
        //아트 리소스 불러오기
        if (DataManager.Instance._artResources.ContainsKey(evidence.artresourceId))
        {
            //아트 리소스 내 증거물 인벤토리 이미지 가져오기
            ArtResourceStructure artResource = DataManager.Instance._artResources[evidence.artresourceId];
            _evidenceImage.sprite = artResource.GetSpriteFromFilePath(artResource.filePathInventoryThumbnail);
        }
        else
        {
            Debug.Log(evidence.artresourceId + "가 리소스 내에 존재하지 않습니다.");
        }
    }
}
