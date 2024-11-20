using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteract : Singleton<PlayerInteract>
{
    [SerializeField] private GameObject _curEnterNPC;
    public bool isInteracting = false;
    
    //정신세계 진입 관련 변수
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private GameObject _progressBar;
    [SerializeField] private EnterProgressBar _progressBarFill;
    [SerializeField] private float _enterTime = 5.0f;
    private bool _canEnter = false;
    private bool _startEnter = false;
    public bool _isComplete = false;
    private float _timer = 0f;

    public bool isEnd = false;

    //찐 사용 변수 우선 아래에 옮기기
    [SerializeField] private GameObject _curInteractableEventID;
    public bool canInteract = true; //상호작용을 할 수 있는지(lock 조건에 이용)

    [Header("상호작용 표식")] [SerializeField] private GameObject _interactionMark;
    
    void Update()
    {

        if (canInteract && _curInteractableEventID != null && Input.GetKeyUp(KeyCode.E))
        { 
            //이벤트 실행
            string eventID = _curInteractableEventID.GetComponent<EventTrigger>().eventID;
            if (CheckInteractionAvail(eventID))
            {
                StartCoroutine(EventManagerYKM.Instance.ExecuteEvent(eventID));
                _interactionMark.SetActive(false);
            }
            
            //상호작용 물체 초기화
            //_curInteractableEventID = null;
        }
        
        /*
        //상호작용 가능한데, E 버튼 클릭하면
        if (_canInteract && Input.GetKeyUp(KeyCode.E))
        {
            GetComponent<PlayerController>().isDialogueOn = true;
            //현재 상호작용 오브젝트 내의 public 함수 호출
            _uiManager._showPressBtnUI.SetActive(false);
            _uiManager._bookInfo.SetActive(true);
            isInteracting = true;
        }
        
        //진입 가능하며, space 버튼 클릭하면
        if (_canEnter && Input.GetKeyDown(KeyCode.Space))
        {
            if (_curEnterNPC.GetComponent<NPCController>()._canEnterMentalWorld)
            {
                //진입 가능 NPC
                _uiManager._showPressBtnUI.SetActive(false);
                _progressBar.SetActive(true);
                _startEnter = true;
            }
            else
            {
                //진입 불가능 NPC
                Debug.Log("여기 실행??");
                _uiManager._showPressBtnUI.GetComponent<TextMeshProUGUI>().text = "정신세계에 진입하기 올바른 대상이 아닙니다";
                _uiManager._showPressBtnUI.SetActive(true);
            }
        }
        
        //진입시작했고, 완료되지 않았고, 스페이스를 계속 누르고 있다면
        if (_canEnter && !_isComplete && Input.GetKey(KeyCode.Space))
        {
            float value = _progressBarFill.FillAmount();
            _uiManager.transitionAnimator.Play("TS_4_Normal_Reveal", 0, value);
            if (value >= 1f)
            {
                _isComplete = true;
                StartCoroutine(_uiManager.ActiveEndingMessage());
            }
        }
        else
        {
            _uiManager.transitionAnimator.speed = 0;
        }
        
        
        //팝업 열려있으면 ESC버튼을 통해 팝업 끌 수 있음.
        if (_uiManager.isPopUpOpen && Input.GetKeyUp(KeyCode.Escape))
        {
            GetComponent<PlayerController>().isDialogueOn = false;
            _uiManager._bookPopUp.SetActive(false);
            isInteracting = false;
            _uiManager.isPopUpOpen = false;
        }
        
        //게임이 완료되고, esc를 누르면 게임 종료
        if (isEnd && Input.GetKeyUp(KeyCode.Escape))
        {
            Debug.Log("게임 종료");
            Application.Quit();
        }
        */
    }

    void FixedUpdate()
    {
        /*
        if (_startEnter)
        {
            _timer += Time.fixedDeltaTime;

            if (_isComplete)
            {
                Debug.Log("시간 내에 진입 완료");
                InitProgressBar();
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || 
                Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                Debug.Log("진입 중에 움직임");
                InitProgressBar();
            }

            if (_timer >= _enterTime)
            {
                if (!_isComplete)
                {
                    Debug.Log("시간 내에 진입 완료하지 못함");
                    InitProgressBar();
                }
            }
        }
        */
    }


    private void OnTriggerEnter(Collider other)
    {
        /*
        if(other.CompareTag("InvestigateObj"))
        {
            _uiManager._showPressBtnUI.GetComponent<TextMeshProUGUI>().text = "E를 눌러 확인";
            _canInteract = true;
            _curInteractableObj = other.gameObject;
            //ui에 텍스트 띄우기
            _uiManager._showPressBtnUI.SetActive(true);
        }

        if (other.CompareTag("ProgressNPC"))
        {
            _uiManager._showPressBtnUI.GetComponent<TextMeshProUGUI>().text = "SPACE를 눌러 정신 세계 진입";
            _uiManager._showPressBtnUI.SetActive(true);
            _canEnter = true;
            _curEnterNPC = other.gameObject;
        }
        */
        //진입 시 바로 이벤트 실행되는 트리거에 진행하면
        if (other.CompareTag("EventTrigger"))
        {
            string eventID = other.GetComponent<EventTrigger>().eventID;
            StartCoroutine(EventManagerYKM.Instance.ExecuteEvent(eventID));
        }

        if (other.CompareTag("EventInteractionTrigger"))
        {
            _curInteractableEventID = other.gameObject;
            if(CheckInteractionAvail(other.GetComponent<EventTrigger>().eventID)) ShowInteractionMark();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EventInteractionTrigger"))
        {
            _curInteractableEventID = null; 
            _interactionMark.SetActive(false);
        }
        /*
        if(other.CompareTag("InvestigateObj"))
        {
            _canInteract = false;
            _curInteractableObj = null;
            //ui 텍스트 없애기
            _uiManager._showPressBtnUI.SetActive(false);
        }
        
        if (other.CompareTag("ProgressNPC"))
        {
            _canEnter = false;
            _curEnterNPC = null;
            _uiManager._showPressBtnUI.SetActive(false);
        }
        */
    }

    void InitProgressBar()
    {
        /*
        _progressBarFill.InitFillAmount();
        _timer = 0f;
        _progressBar.SetActive(false);
        if(!_isComplete) _uiManager.transitionAnimator.Play("TS_4_Normal_Reveal", 0, 0);
        _startEnter = false;
        */
    }

    void ShowInteractionMark()
    {
        _interactionMark.SetActive(true);
    }

    bool CheckInteractionAvail(string id)
    {
        if (DataManager.Instance._events.ContainsKey(id))
        {
            EventStructure _event = DataManager.Instance._events[id];
            Debug.Log("nextEventID : " + EventManagerYKM.Instance.nextEventID + ", thisID : " + id +" , eventCondition? : " + _event.CheckCondition());
            if ( EventManagerYKM.Instance.nextEventID == "" || EventManagerYKM.Instance.nextEventID == id)
            {
                if(_event.CheckCondition()) return true;
            }
        }

        return false;
    }
}
