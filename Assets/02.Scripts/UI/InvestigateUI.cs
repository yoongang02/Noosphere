using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InvestigateUI : UIBase, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
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
        UIManager.Instance.OpenUI(UIManager.Instance.evidenceDetailUI,_curEvidence);
        isAquired = true;
        UIManager.Instance.OnSelectEnd?.Invoke();
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

    public void OnPointerClick(PointerEventData eventData)
    {
        //클릭한 오브젝트가 슬롯인지 파악
        GameObject clickedObject = eventData.pointerClick;
        
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (clickedObject == _yesBtn)
            {
                ClickYesBtn();
            }
            else if (clickedObject == _noBtn)
            {
                ClickNoBtn();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //클릭한 오브젝트가 슬롯인지 파악
        GameObject enteredObject = eventData.pointerEnter;
        
        if (enteredObject == _yesBtn)
        {
            HoverYesBtn();
        }
        else if (enteredObject == _noBtn)
        {
            HoverNoBtn();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //클릭한 오브젝트가 슬롯인지 파악
        GameObject exitedObject = eventData.pointerEnter;
        
        if (exitedObject == _yesBtn)
        {
            SetButtonSelected(_yesBtn,UnityExtension.HexColor(WhiteColor));
        }
        else if (exitedObject == _noBtn)
        {
            SetButtonSelected(_yesBtn,UnityExtension.HexColor(WhiteColor));
        }
    }
}
