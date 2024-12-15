using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EvidenceDetailUI : UIBase
{
    [Header("증거물 상세 UI GameObject")] [SerializeField]
    private GameObject _objectUI;
    [SerializeField] private GameObject _onePageUI;
    [SerializeField] private GameObject _twoPageUI;
    
    [Header("증거물 상세내용 공통 UI")]
    [SerializeField] private Image _bgImg;
    [SerializeField] private Sprite _inventoryBgImg;

    [Header("Page 증거물")] [SerializeField] private int _curPage;
    [SerializeField] private int _totalPage;
    [SerializeField] private List<Sprite> _pages;
    [SerializeField] private Image _firstPage;
    [SerializeField] private Image _secondPage;
    [SerializeField] private GameObject _prevPageBtn; //이전 페이지 버튼
    [SerializeField] private GameObject _nextPageBtn; //다음 페이지 버튼

    public override void OnOpen(EvidenceStructure evidence)
    {
        base.OnOpen(evidence);
        Debug.Log($"#{evidence}에 대한 상세 설명 오픈");
        if (evidence == null)
        {
            Debug.LogError("🔥 evidence가 null이므로 OpenUI()를 호출할 수 없습니다.");
            return;
        }
        
        //인벤토리에서 증거물 상세사항을 오픈할 경우에는 UI 순서를 위해 아래의 설정이 필요함.
        if (!UIManager.Instance.isInMap)
        {
            PlayerController.Instance._uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            PlayerController.Instance._uiCanvas.worldCamera = PlayerController.Instance._mainCamera;
        }
        else
        {
            //맵에서 증거물 상세사항이 오픈된 경우에는 해당 증거물을 습득함.
            Debug.Log($"Evidence Detail UI {evidence} 확인");
            evidence.AcquireEvidence();
        }
        SetDetailEvidence(evidence);
        
        if (transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        
        if (PlayerController.Instance._uiCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            PlayerController.Instance._uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        
        _objectUI.SetActive(false);
        _onePageUI.SetActive(false);
        _twoPageUI.SetActive(false);
        
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();
        
        //키보드 A - 이전 페이지 버튼 
        if (_prevPageBtn != null && _nextPageBtn != null)
        {
            if (_prevPageBtn.activeSelf && Input.GetKeyDown(KeyCode.A))
            {
                RemoveAllListeners();
                AddOnClickListener(ClickPrevPageEvent);
                OnClickEvent?.Invoke();
            }

            //키보드 D - 다음 페이지 버튼
            if (_nextPageBtn.activeSelf && Input.GetKeyDown(KeyCode.D))
            { 
                RemoveAllListeners();
                AddOnClickListener(ClickNextPageEvent);
                OnClickEvent?.Invoke();
            }
        }
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
                _objectUI.SetActive(true);
            }
            else if (evidence.shapeType == "OnePage")
            {
                InitPageVariables(artResource);
                SetOnePageDetail(artResource);
                _onePageUI.SetActive(true);
            }
            else if (evidence.shapeType == "TwoPage")
            {
                InitPageVariables(artResource);
                SetTwoPageDetail(artResource);
                _twoPageUI.SetActive(true);
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
        if (_objectUI.transform.childCount > 0)
        {
            foreach (Transform child in _objectUI.transform)
            {
                Destroy(child.gameObject);
            }
        }
        
        GameObject prefab = artResource.GetPrefabFromFilePath();
        if (prefab != null)
        {
            Instantiate(prefab, _objectUI.transform);
        }
        else
        {
            Debug.LogError($"🔥 {artResource.filePath}에 해당하는 프리팹이 존재하지 않습니다.");
        }
    }

    void InitPageVariables(ArtResourceStructure artResource)
    {
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
        //버튼 참조하기
        _prevPageBtn = _onePageUI.transform.Find("PageBtns").GetChild(0).gameObject;
        _nextPageBtn = _onePageUI.transform.Find("PageBtns").GetChild(1).gameObject;
        //이미지 null로 초기화
        _firstPage = null;
        _secondPage = null;
    }
    
    // shapeType이 onePage인 증거물인 경우
    void SetOnePageDetail(ArtResourceStructure artResource)
    {
        //페이지 초기화
        _curPage = 1;
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
            _prevPageBtn.SetActive(false);
            _nextPageBtn.SetActive(true);
        }
        else
        {
            Debug.Log($"{artResource.artresourceId}의 pageCnt가 올바르지 않습니다.");
        }
        
        //이미지 설정하기
        _firstPage = _onePageUI.transform.Find("Pages").GetChild(0).gameObject.GetComponent<Image>();
        _firstPage.sprite = _pages[0];
    }
    
    // shapeType이 twoPage인 증거물인 경우
    void SetTwoPageDetail(ArtResourceStructure artResource)
    {
        //페이지 초기화
        _curPage = 2;
        //페이지가 2개인 경우, 2개 이상인 경우
        if (_totalPage == 2)
        {
            _prevPageBtn.SetActive(false);
            _nextPageBtn.SetActive(false);
        }
        else if (_totalPage > 2)
        {
            _prevPageBtn.SetActive(false);
            _nextPageBtn.SetActive(true);
        }
        else
        {
            Debug.Log($"{artResource.artresourceId}의 pageCnt가 올바르지 않습니다.");
        }
        
        //이미지 설정하기
        _firstPage = _onePageUI.transform.Find("Pages").GetChild(0).gameObject.GetComponent<Image>();
        _secondPage = _onePageUI.transform.Find("Pages").GetChild(1).gameObject.GetComponent<Image>();
        _firstPage.sprite = _pages[0];
        _secondPage.sprite = _pages[1];
    }

    void UpdateOnePage()
    {
        //이미지 업데이트
        _firstPage.sprite = _pages[_curPage - 1];
        //버튼 업데이트
        bool prev = _curPage != 1;
        bool next = _curPage != _totalPage;
        
        _prevPageBtn.SetActive(prev);
        _nextPageBtn.SetActive(next);
    }
    
    void UpdateTwoPage()
    {
        //이미지 업데이트
        _firstPage.sprite = _pages[_curPage - 1];
        _secondPage.sprite = _pages[_curPage];
        //버튼 업데이트
        bool prev = _curPage != 2;
        bool next = _curPage != _totalPage;
        
        _prevPageBtn.SetActive(prev);
        _nextPageBtn.SetActive(next);
    }

    void ClickNextPageEvent()
    {
        if (_secondPage == null)
        {
            _curPage++;
            UpdateOnePage();
        }
        else
        {
            _curPage += 2;
            UpdateTwoPage();
        }
    }

    void ClickPrevPageEvent()
    {
        if (_secondPage == null)
        {
            _curPage--;
            UpdateOnePage();
        }
        else
        {
            _curPage -= 2;
            UpdateTwoPage();
        }
    }
}
