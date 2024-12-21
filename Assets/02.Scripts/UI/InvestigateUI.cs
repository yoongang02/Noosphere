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
    
    public bool isAquired = false;

    public override void OnOpen(EvidenceStructure evidence)
    {
        base.OnOpen(evidence);
        //조사하는 증거물에 대한 정보 반영
        if (!UIManager.Instance.isInMap)
        {
            //증거물 상세보기가 열려있는 경우, 조사 UI는 열려도, 위에 보이지 않기 때문에
            UIManager.Instance.CloseAllUI();
        }
        SetInvestigateUI(evidence);
        _curEvidence = evidence;
        isAquired = false;
        HoverYesBtn();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        UIManager.Instance.OnSelectEnd?.Invoke();
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();
        //키보드 A - YES 버튼
        if (Input.GetKeyDown(KeyCode.A))
        {
            HoverYesBtn();
        }

        //키보드 D - NO 버튼
        if (Input.GetKeyDown(KeyCode.D))
        { 
            HoverNoBtn();
        }

        //스페이스 - 버튼 선택
        if (_curSelectedBtn != null && Input.GetKeyDown(KeyCode.Space))
        {
            if (_curSelectedBtn == _yesBtn)
            {
                ClickYesBtn();
            }
            else if (_curSelectedBtn == _noBtn)
            {
                ClickNoBtn();
            }
        }
            
        //ESC - NO 선택
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //NO 버튼 선택
            ClickNoBtn();
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
                HoverYesBtn();
                if (Input.GetMouseButtonDown(0)) 
                {
                    ClickYesBtn();
                }
            }
            else if (result.gameObject == _noBtn)
            {
                HoverNoBtn();
                if (Input.GetMouseButtonDown(0)) 
                {
                    ClickNoBtn();
                }
            }
        }
    }
    
    
    void HoverYesBtn()
    {
        SetButtonSelected(_yesBtn, UnityExtension.HexColor(GreenColor));
        SetButtonSelected(_noBtn, UnityExtension.HexColor(WhiteColor));
        _curSelectedBtn = _yesBtn;
    }

    void HoverNoBtn()
    {
        SetButtonSelected(_noBtn,UnityExtension.HexColor(GreenColor));
        SetButtonSelected(_yesBtn,UnityExtension.HexColor(WhiteColor));
        _curSelectedBtn = _noBtn;
    }

    void ClickYesBtn()
    {
        HoverYesBtn();
        UIManager.Instance.CloseTopUI();
        
        isAquired = true;
        UIManager.Instance.OnSelectEnd?.Invoke();
        UIManager.Instance.OpenUI(UIManager.Instance.evidenceDetailUI,_curEvidence);
        
        _curEvidence = null;
    }

    void ClickNoBtn()
    {
        HoverNoBtn();
        UIManager.Instance.CloseTopUI();
        
        isAquired = false;
        UIManager.Instance.OnSelectEnd?.Invoke();
        
        _curEvidence = null;
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
