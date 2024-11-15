using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIManager : Singleton<UIManager>
{
    const string GreenColor = "#01EE00";
    const string WhiteColor = "#FFFFFF";
    /*
    public GameObject _showPressBtnUI;
    public GameObject _bookInfo;
    public GameObject _bookPopUp;
    public bool isPopUpOpen = false;
    public Animator transitionAnimator;
    public GameObject _endingMessage;
    */
    
    public TextMeshProUGUI dialogueUI;
    public TextMeshProUGUI popUI;

    //증거물 조사 UI
    private bool _isInvestigateUIOpened = false;
    [SerializeField] private GameObject _investigateUI;
    [SerializeField] private GameObject _investigateUIYesBtn;
    [SerializeField] private GameObject _investigateUINoBtn;
    [SerializeField] private GameObject _curSelectedBtn;
    private EvidenceStructure _curInvestigateEvidence;
    public bool isSelecting = false;
    public bool isEvidenceAcquired = false;

    void Update()
    {
        if (_isInvestigateUIOpened)
        {
            //왼쪽 화살표 - YES 버튼
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SelectYesBtn();
            }

            //오른쪽 화살표 - NO 버튼
            if (Input.GetKeyDown(KeyCode.RightArrow))
            { 
                SelectNoBtn();
            }

            //스페이스 - 버튼 선택
            if (_curSelectedBtn != null && Input.GetKeyDown(KeyCode.Space))
            {
                //해당 버튼의 이벤트 트리거 호출
                ExecuteEvents.Execute<ISelectHandler>(
                    _curSelectedBtn, 
                    new BaseEventData(EventSystem.current), (x, y) => x.OnSelect(y)
                );
                isSelecting = false;
            }
            
            //ESC - NO 선택
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //NO 버튼 선택
                SelectNoBtn();
                
                //해당 버튼의 이벤트 트리거 호출
                ExecuteEvents.Execute<ISelectHandler>(
                    _curSelectedBtn, 
                    new BaseEventData(EventSystem.current), (x, y) => x.OnSelect(y)
                );
                isSelecting = false;
            }
        }

    }

    //증거물 조사 UI의 정보 세팅하기
    public void SetInvestigateUI(EvidenceStructure evidence)
    {
        TextMeshProUGUI[] texts = _investigateUI.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var text in texts)
        {
            if (text.gameObject.name == "Evidence Name")
            {
                text.text = evidence.evidence_name;
            }
            else if (text.gameObject.name == "Evidence Text")
            {
                text.text = evidence.evidence_Text_Display;
            }
        }

        Image[] images = _investigateUI.GetComponentsInChildren<Image>(true);
        foreach (var image in images)
        {
            if (image.gameObject.name == "Evidence Img")
            {
                //아트 리소스 불러오기
                if (EventManagerYKM.Instance._artResources.ContainsKey(evidence.artresource_id))
                {
                    image.sprite = EventManagerYKM.Instance._artResources[evidence.artresource_id]
                        .GetSpriteFromFilePath();
                }
                else
                {
                    Debug.Log(evidence.artresource_id + "가 리소스 내에 존재하지 않습니다.");
                }

                break;
            }
        }

        //디폴트로 YES 선택되어 있음.
        SelectYesBtn();
    }


    //증거물 조사 UI 띄우기
    public void OpenInvestigateUI(EvidenceStructure evidence)
    {
        SetInvestigateUI(evidence);
        _curInvestigateEvidence = evidence;
        _isInvestigateUIOpened = true;
        isEvidenceAcquired = false;
        _investigateUI.SetActive(true);
        isSelecting = true;
    }

    public void CloseInvestigateUI()
    {
        _isInvestigateUIOpened = false;
        _curInvestigateEvidence = null;
        _investigateUI.SetActive(false);
    }
    //증거물 조사 UI - 버튼 선택
    void SetButtonSelected(GameObject btn, Color color)
    {
        TextMeshProUGUI tmp = btn.GetComponent<TextMeshProUGUI>();
        tmp.color = color;
    }

    void SelectYesBtn()
    {
        SetButtonSelected(_investigateUIYesBtn, UnityExtension.HexColor(GreenColor));
        SetButtonSelected(_investigateUINoBtn, UnityExtension.HexColor(WhiteColor));
        _curSelectedBtn = _investigateUIYesBtn;
    }

    void SelectNoBtn()
    {
        SetButtonSelected(_investigateUINoBtn,UnityExtension.HexColor(GreenColor));
        SetButtonSelected(_investigateUIYesBtn,UnityExtension.HexColor(WhiteColor));
        _curSelectedBtn = _investigateUINoBtn;
    }

    public void ShowDetailEvidence()
    {
        Debug.Log("자세히 보기 실행");
        isEvidenceAcquired = true;
        _curInvestigateEvidence.AcquireEvidence();
        CloseInvestigateUI();
    }

    public void PopUp(bool isActive,string text="")
    {
        popUI.gameObject.SetActive(isActive);
        popUI.text = text;
    }
    
}
