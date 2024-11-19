using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIManager : Singleton<UIManager>
{
    public static string GreenColor = "#01EE00";
    public static string WhiteColor = "#FFFFFF";
    public static string BlackColor = "#000000";
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
    [Header("증거물 조사 UI")]
    [SerializeField] private GameObject _investigateUI;
    [SerializeField] private GameObject _investigateUIYesBtn;
    [SerializeField] private GameObject _investigateUINoBtn;
    [SerializeField] private GameObject _curSelectedBtn;
    private EvidenceStructure _curInvestigateEvidence;
    public bool isSelecting = false;
    public bool isEvidenceAcquired = false;

    [Space(5)] [Header("증거물 상세 내용 UI")] [SerializeField]
    private bool _isInMap = true;
    [SerializeField] private GameObject _evidenceDetailUI;
    [SerializeField] private GameObject _evidenceDetailBackground;
    [SerializeField] private Sprite _evidenceDetailInventoryBackground;
    [SerializeField] private List<Sprite> _evidenceTextDetailImgs;
    [SerializeField] private GameObject _evidenceDetailPrefabParent;
    [Space]
    [SerializeField] private GameObject _evidenceDetailTextParent;
    [SerializeField] private GameObject _evidenceDetailTextLeftBtn;
    [SerializeField] private GameObject _evidenceDetailTextRightBtn;
    [SerializeField] private Image _evidenceDetailTextImg;
    [SerializeField] private int _evidenceDetailTextCurPageIndex = 0;
    [SerializeField] private bool _isDetailOpen = false;
    [SerializeField] private bool _isDetailText = false;
    
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

        //증거물 상세내용 UI가 열려있고, 텍스트 상세내용이라면 페이지 버튼 활성화
        if (_isDetailOpen)
        {
            if(_isDetailText)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow) && _evidenceDetailTextCurPageIndex != 0)
                {
                    _evidenceDetailTextCurPageIndex -= 1;
                    UpdateTextDetail(_evidenceDetailTextCurPageIndex);
                }

                if (Input.GetKeyDown(KeyCode.RightArrow) && (_evidenceDetailTextCurPageIndex != (_evidenceTextDetailImgs.Count -1)))
                {
                    _evidenceDetailTextCurPageIndex += 1;
                    UpdateTextDetail(_evidenceDetailTextCurPageIndex);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseDetailEvidence();
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
                Debug.Log("art resource id : " + evidence.artresource_id);
                //아트 리소스 불러오기
                if (DataManager.Instance._artResources.ContainsKey(evidence.artresource_id))
                {
                    //아트 리소스 내 증거물 인벤토리 이미지 가져오기
                    ArtResourceStructure artResource = DataManager.Instance._artResources[evidence.artresource_id];
                    Debug.Log("artResource debug : " + artResource + ", filePath : " + artResource.inventoryFilePath);
                    image.sprite = artResource.GetSpriteFromFilePath(artResource.inventoryFilePath);
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
        _isInMap = true;
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
        SetDetailEvidence(_curInvestigateEvidence);
        Debug.Log("자세히 보기 실행");
        
        //인벤토리에 해당 증거물 획득
        isEvidenceAcquired = true;
        _curInvestigateEvidence.AcquireEvidence();
        CloseInvestigateUI();
        
        //상세보기 창 열기
        _isDetailOpen = true;
    }

    void CloseDetailEvidence()
    {
        _isDetailOpen = false;
        _evidenceDetailUI.SetActive(false);
    }

    void SetDetailEvidence(EvidenceStructure evidence)
    {
        ArtResourceStructure artResource = DataManager.Instance._artResources[evidence.artresource_id];
        //배경 이미지 변경하기
        Image backgroundImg = _evidenceDetailBackground.GetComponent<Image>();

        if (!_isInMap)
        {
            backgroundImg.sprite = _evidenceDetailInventoryBackground;
        }
        else
        {
            backgroundImg.sprite = artResource.GetSpriteFromFilePath(artResource.map_background_img);
        }
        
        //evidence 성질에 따라 프리팹인지 UI인지 결정
        if (evidence.shape_Type == 'P')
        {
            SetPrefabDetail(artResource);
            _evidenceDetailPrefabParent.SetActive(true);
            _evidenceDetailTextParent.SetActive(false);
        }
        else if (evidence.shape_Type == 'T')
        {
            _isDetailText = true;
            SetTextDetail(artResource);
            _evidenceDetailPrefabParent.SetActive(false);
            _evidenceDetailTextParent.SetActive(true);
        }
    }

    void SetPrefabDetail(ArtResourceStructure artResource)
    {
        //prefab parent 아래에 자식 오브젝트가 있다면 제거 후, 올바른 오브젝트 생성
        if (_evidenceDetailPrefabParent.transform.childCount > 0)
        {
            foreach (Transform child in _evidenceDetailPrefabParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        Instantiate(artResource.GetPrefabFromFilePath(), _evidenceDetailPrefabParent.transform);
    }

    void SetTextDetail(ArtResourceStructure artResource)
    {
        //초기화
        _evidenceDetailTextCurPageIndex = 0;
        _evidenceTextDetailImgs.Clear();
        //페이지가 여러개 일수도 있으니까 페이지 불러오기
        int totalPageCnt = artResource.text_detail_img_cnt;

        if (totalPageCnt > 0)
        {
            for (int page = 0; page < totalPageCnt; page++)
            {
                //파일 시작 경로에서 이미지 숫자만큼 불러오기
                string imgPath = artResource.text_detail_start_img;
                int lastUnderscoreIndex = imgPath.LastIndexOf('_'); 
                string prefix = imgPath.Substring(0, lastUnderscoreIndex + 1);
                string modifiedString = $"{prefix}{page:D2}";
            
                Sprite pageImg = artResource.GetSpriteFromFilePath(modifiedString);
                _evidenceTextDetailImgs.Add(pageImg);
            }

            _evidenceDetailTextImg.sprite = _evidenceTextDetailImgs[0];
            //페이지 개수에 따른 버튼 업데이트
            UpdateTextDetailBtns(totalPageCnt);
        }
        else
        {
            Debug.Log("text detail img가 존재하지 않습니다.");
        }
    }

    void UpdateTextDetail(int index)
    {
        //index에 따라 이미지 변경
        int totalPageCnt = _evidenceTextDetailImgs.Count;
        _evidenceDetailTextImg.sprite = _evidenceTextDetailImgs[index];

        //페이지 버튼 변경
        UpdateTextDetailBtns(totalPageCnt);
    }

    void UpdateTextDetailBtns(int totalPage)
    {
        if (totalPage > 1)
        {
            if (_evidenceDetailTextCurPageIndex == 0)
            {
                _evidenceDetailTextLeftBtn.SetActive(false);
                _evidenceDetailTextRightBtn.SetActive(true);
            }
            else if (_evidenceDetailTextCurPageIndex == totalPage - 1)
            {
                _evidenceDetailTextLeftBtn.SetActive(true);
                _evidenceDetailTextRightBtn.SetActive(false);
            }
            else
            {
                _evidenceDetailTextLeftBtn.SetActive(true);
                _evidenceDetailTextRightBtn.SetActive(true);
            }
        }
        else
        {
            _evidenceDetailTextLeftBtn.SetActive(false);
            _evidenceDetailTextRightBtn.SetActive(false);
        }
    }

    public void PopUp(bool isActive,string text="")
    {
        popUI.gameObject.SetActive(isActive);
        popUI.text = text;
    }
    
}
