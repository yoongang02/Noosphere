using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvidenceDetailUI : UIBase
{
    [Header("증거물 상세 내용 UI")]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Sprite _evidenceDetailInventoryBackground;
    [SerializeField] private List<Sprite> _evidenceTextDetailImgs;
    [SerializeField] private GameObject _evidenceDetailPrefabParent;
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
                _backgroundImage.sprite = _evidenceDetailInventoryBackground;
            }
            else
            {
                _backgroundImage.sprite = artResource.GetSpriteFromFilePath(artResource.mapBackgroundImg);
            }
            
            //evidence 성질에 따라 프리팹인지 UI인지 결정
            if (evidence.shapeType == "Object") SetPrefabDetail(artResource);
            //SetActiveExtra(evidence.shapeType);
        }
        else
        {
            Debug.Log($"#{evidence.artresourceId} 아트 리소스가 존재하지 않습니다.");
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
