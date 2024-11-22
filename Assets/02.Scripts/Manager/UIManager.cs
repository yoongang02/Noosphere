using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using VFolders.Libs;

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
    public bool _isInvestigateUIOpened = false;
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
    [Space] [SerializeField] private GameObject _newsPaperParent;
    [Space] [SerializeField] private GameObject _letterParent;
    [Space] [SerializeField] private GameObject _participateFileParent;
    [Space] [SerializeField] private GameObject _researchFileMParent;
    [Space] [SerializeField] private GameObject _researchFileRParent;
    [Space] [SerializeField] private GameObject _diaryParent;
    
    public bool _isDetailOpen = false;
    private Dictionary<string, GameObject> _detailParents = new Dictionary<string,GameObject>();
    
    [Space(5)] [Header("강제 종료 관련 변수")] [SerializeField] 
    private EvidenceStructure curDetailEvidence;
    [SerializeField] private float _forceQuitSeconds = 0.5f;

    void Start()
    {
        _detailParents.Add("P",_evidenceDetailPrefabParent);
        _detailParents.Add("newsPaper",_newsPaperParent);
        _detailParents.Add("letter",_letterParent);
        _detailParents.Add("participateFile",_participateFileParent);
        _detailParents.Add("researchFileM",_researchFileMParent);
        _detailParents.Add("researchFileR",_researchFileRParent);
        _detailParents.Add("diary",_diaryParent);
    }
    
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
                Debug.Log("상세 내용 조사 YES BTN 클릭");
                isSelecting = false;
            }
            
            //ESC - NO 선택
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //NO 버튼 선택
                SelectNoBtn();
                Debug.Log("상세 내용 조사 NO BTN 클릭");
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

        PlayerController.Instance.isDialogueOn = true;
    }

    public void CloseInvestigateUI()
    {
        _isInvestigateUIOpened = false;
        _curInvestigateEvidence = null;
        _investigateUI.SetActive(false);
        PlayerController.Instance.isDialogueOn = false;
        EventManagerYKM.Instance.StopCoroutine();
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
        curDetailEvidence = _curInvestigateEvidence;
        SetDetailEvidence(_curInvestigateEvidence);
        Debug.Log("자세히 보기 실행");
        
        //인벤토리에 해당 증거물 획득
        isEvidenceAcquired = true;
        _curInvestigateEvidence.AcquireEvidence();
        CloseInvestigateUI();
        
        //상세보기 창 열기
        _isDetailOpen = true;
        _evidenceDetailUI.SetActive(true);
    }
    
    public void ShowDetailEvidenceInInventory(EvidenceStructure evidence)
    {
        curDetailEvidence = evidence;
        SetDetailEvidence(evidence);
        Debug.Log("자세히 보기 실행");
        //상세보기 창 열기
        _isDetailOpen = true;
        _evidenceDetailUI.SetActive(true);
    }

    public void CloseDetailEvidence()
    {
        _isDetailOpen = false;
        _evidenceDetailUI.SetActive(false);

        //해당 이벤트만 예외 조건으로 실행
        if (EventManagerYKM.Instance.nextEventID == "Event_A008")
        {
            CoroutineManager.Instance.StartManagedCoroutine(
                EventManagerYKM.Instance.ExecuteEvent(EventManagerYKM.Instance.nextEventID));
        }
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
        SetActiveExtra(evidence.shape_Type);
    }

    void SetActiveExtra(string key)
    {
        foreach (var detail in _detailParents)
        {
            if (detail.Key == key)
            {
                detail.Value.SetActive(true);
            }
            else
            {
                detail.Value.SetActive(false);
            }
            
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

    /*
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
    */

    /*
    void UpdateTextDetail(int index)
    {
        //index에 따라 이미지 변경
        int totalPageCnt = _evidenceTextDetailImgs.Count;
        _evidenceDetailTextImg.sprite = _evidenceTextDetailImgs[index];

        //페이지 버튼 변경
        UpdateTextDetailBtns(totalPageCnt);
    }
    /*

    /*
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
    /*

    /*
    public void PopUp(bool isActive,string text="")
    {
        popUI.gameObject.SetActive(isActive);
        popUI.text = text;
    }
    */
    public IEnumerator ForceQuitInteraction(EvidenceStructure evidenceStructure)
    {
        yield return new WaitForSeconds(_forceQuitSeconds);
        Debug.Log("강제 종료!!!!");
        if (_isDetailOpen)
        {
            CloseDetailEvidence();

            if (!evidenceStructure.acquisition_Page_Result_id.IsNullOrEmpty())
            {
                string resultID = evidenceStructure.acquisition_Page_Result_id;
                if (evidenceStructure.accessCnt == 1)
                {
                    if (DataManager.Instance._events.ContainsKey(resultID))
                    {
                        CoroutineManager.Instance.StartManagedCoroutine(EventManagerYKM.Instance.ExecuteEvent(resultID));
                    }
                }
                else if (evidenceStructure.accessCnt >= 2)
                {
                    //첫번째 열람했을 떄 호출하는 이벤트의 다음 이벤트를 확인하고 해당 이벤트를 호출
                    if (DataManager.Instance._events.ContainsKey(resultID))
                    {
                        string nextEventID = DataManager.Instance._events[resultID].next_Event_id;
                        
                        if (DataManager.Instance._events.ContainsKey(nextEventID))
                        {
                            CoroutineManager.Instance.StartManagedCoroutine(EventManagerYKM.Instance.ExecuteEvent(resultID));
                        }
                    }
                }
            }
        }
    }
}
