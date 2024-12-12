using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvidenceDetailUI : UIBase
{
    [Header("증거물 상세 UI GameObject")] [SerializeField]
    private GameObject _objectUI;
    private GameObject _onePageUI;
    private GameObject _twoPageUI;
    
    [Header("증거물 상세내용 공통 UI")]
    [SerializeField] private Image _bgImg;
    [SerializeField] private Sprite _inventoryBgImg;
    
    [Header("Object 증거물")]
    [SerializeField] private GameObject _objectParent;

    [Header("Page 증거물")] [SerializeField] private int _curPage;
    [SerializeField] private int _totalPage;
    [SerializeField] private List<Sprite> _pages;
    private GameObject _prevPageBtn; //이전 페이지 버튼
    private GameObject _nextPageBtn; //다음 페이지 버튼
    
    public override void OnOpen()
    {
        base.OnOpen();
    }

    public override void OnOpen(EvidenceStructure evidence)
    {
        //인벤토리에서 증거물 상세사항을 오픈할 경우에는 UI 순서를 위해 아래의 설정이 필요함.
        if (!UIManager.Instance.isInMap)
        {
            PlayerController.Instance._uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            PlayerController.Instance._uiCanvas.worldCamera = PlayerController.Instance._mainCamera;
        }
        else
        {
            //맵에서 증거물 상세사항이 오픈된 경우에는 해당 증거물을 습득함.
            evidence.AcquireEvidence();
        }
        SetDetailEvidence(evidence);
        base.OnOpen(evidence);
    }

    public override void OnClose()
    {
        base.OnClose();
        
        if (PlayerController.Instance._uiCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            PlayerController.Instance._uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();
    }

    public override void HandleMouseInput()
    {
        base.HandleMouseInput();
    }
    
    void SetDetailEvidence(EvidenceStructure evidence)
    {
        ArtResourceStructure artResource = DataManager.Instance._artResources[evidence.artresourceId];

        if (artResource != null)
        {
            if (!UIManager.Instance.isInMap)
            {
                _bgImg.sprite = _inventoryBgImg;
            }
            else
            {
                _bgImg.sprite = artResource.GetSpriteFromFilePath(artResource.filePathMapBackground);
            }
            
            //evidence 성질에 따라 프리팹인지 UI인지 결정
            if (evidence.shapeType == "Object")
            {
                SetObjectDetail(artResource);
            }
            else if (evidence.shapeType == "OnePage")
            {
                InitPageVariables(artResource);
            }
            else if (evidence.shapeType == "TwoPage")
            {
                InitPageVariables(artResource);
            }
        }
        else
        {
            Debug.Log($"#{evidence.artresourceId} 아트 리소스가 존재하지 않습니다.");
        }
    }
    
    // shapeType이 object인 증거물인 경우
    void SetObjectDetail(ArtResourceStructure artResource)
    {
        //prefab parent 아래에 자식 오브젝트가 있다면 제거 후, 올바른 오브젝트 생성
        if (_objectParent.transform.childCount > 0)
        {
            foreach (Transform child in _objectParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        Instantiate(artResource.GetPrefabFromFilePath(), _objectParent.transform);
    }

    void InitPageVariables(ArtResourceStructure artResource)
    {
        //페이지 초기화
        _curPage = 0;
        //페이지 리스트 초기화
        _pages.Clear();
        //상세 이미지 불러와서 페이지 리스트에 저장
        _totalPage = artResource.pageCnt;
        //시작 위치에서 totalPage 수 만큼, 변수 증가해서 읽어들이기
        string imgPath = artResource.filePathStartPage;
        for (int page = 0; page < _totalPage; page++)
        {
            int lastUnderscoreIndex = imgPath.LastIndexOf('_'); 
            string prefix = imgPath.Substring(0, lastUnderscoreIndex + 1);
            string modifiedString = $"{prefix}{page:D2}";
            
            Sprite pageImg = artResource.GetSpriteFromFilePath(modifiedString);
            _pages.Add(pageImg);
        }
    }
    
    // shapeType이 onePage인 증거물인 경우
    void SetOnePageDetail(ArtResourceStructure artResource)
    {
        //버튼 참조하기
        _prevPageBtn = _onePageUI.transform.Find("PageBtns").GetChild(0).gameObject;
        _nextPageBtn = _onePageUI.transform.Find("PageBtns").GetChild(1).gameObject;
        
        //페이지가 1개인 경우 vs 1개 이상인 경우
        //1개인 경우 : 좌우이동 버튼 비활성화
        //1개 이상인 경우 : 좌우이동 버튼 활성화
        if (_totalPage == 1)
        {
            _prevPageBtn.SetActive(false);
            _nextPageBtn.SetActive(false);
        }
        else if (_totalPage > 1)
        {
            _prevPageBtn.SetActive(true);
            _nextPageBtn.SetActive(true);
        }
        else
        {
            Debug.Log($"{artResource.artresourceId}의 pageCnt가 올바르지 않습니다.");
        }
        
        //이미지 설정하기
        
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
    /*
    public IEnumerator ForceQuitInteraction(EvidenceStructure evidenceStructure)
    {
        yield return new WaitForSeconds(_forceQuitSeconds);
        Debug.Log("강제 종료!!!!");

        CloseDetailEvidence();


        if (EventManagerYKM.Instance.diaryAccessCnt == 1)
        {
            DialogueTextON.Instance.ShowSimpleText("그 부분은 필요 없는 내용입니다. 훈련에 집중하세요.");
        }
        else if(EventManagerYKM.Instance.diaryAccessCnt > 1)
        {
            DialogueTextON.Instance.ShowSimpleText("나중에 열어보자...");
        }
        */
        /*
        if (_isDetailOpen)
        {
            CloseDetailEvidence();

            if (!String.IsNullOrEmpty(evidenceStructure.acquisition_Page_Result_id))
            {
                string resultID = evidenceStructure.acquisition_Page_Result_id;
                if (evidenceStructure.accessCnt == 1)
                {
                    DialogueTextON.Instance.ShowSimpleText("그 부분은 필요 없는 내용입니다. 훈련에 집중하세요.");
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
                        DialogueTextON.Instance.ShowSimpleText("나중에 열어보자...");
                        string nextEventID = DataManager.Instance._events[resultID].next_Event_id;
                        
                        if (DataManager.Instance._events.ContainsKey(nextEventID))
                        {
                            CoroutineManager.Instance.StartManagedCoroutine(EventManagerYKM.Instance.ExecuteEvent(resultID));
                        }
                    }
                }
            }  
    }
      */
}
