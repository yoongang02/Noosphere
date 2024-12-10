using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteract : Singleton<PlayerInteract>
{
    [Header("상호작용 표식")] 
    public bool canInteract = true; //상호작용을 할 수 있는지(lock 조건에 이용)
    public GameObject curInteractableTrigger;
    public GameObject _interactionMark;
    public GameObject _evidenceGameObject;
   
    
    //정신세계 진입 관련 변수
    [Header("정신세계 진입 관련 조건 변수")]
    public bool isPlayerInMetanlWorld = false;
    [SerializeField] private float _enterTime = 5.0f;
    public GameObject curEnterNPCTrigger;
    [SerializeField] bool _canEnter = false;
    [SerializeField] bool _startEnter = false;
    public bool _isComplete = false;
    [SerializeField] float _timer = 0f;
    
    [Header("정신세계 진입 관련 UI 변수")]
    [SerializeField] private GameObject _progressBarUI;
    [SerializeField] private EnterProgressBar _progressBarFill;
    
    public bool isEnd = false;

    public bool isInteractObj;
    void Update()
    {
        if (!InventoryManager.Instance.isInventoryOpen)
        {
            //E키를 이용한 상호작용
            if (canInteract && curInteractableTrigger != null && Input.GetKeyDown(KeyCode.E))
            { 
                Debug.Log("이벤트 실행되나???");
                //이벤트 실행
                _evidenceGameObject = null;
                EventTrigger trigger = curInteractableTrigger.GetComponent<EventTrigger>();
                foreach (string eventID in trigger.eventIdList)
                {
                    if (trigger.canDestroyEvidence)
                    {
                        _evidenceGameObject = curInteractableTrigger.transform.parent.gameObject;
                    }
                    else
                    {
                        _evidenceGameObject = null;
                    }
                    
                    if (CheckInteractionAvail(eventID))
                    {
                        Debug.Log("상호작용 가능 조건 체크를 올바르게 만족하나???? 여기 실행???");
                        StartCoroutine(EventManagerYKM.Instance.ExecuteEvent(eventID));
                        _interactionMark.SetActive(false);
                    }
                }
            }
            
            //Space 키를 이용한 정신세계 진입 상호작용
            if (_canEnter && curEnterNPCTrigger != null && Input.GetKeyDown(KeyCode.Space))
            {
                //현재 진입 가능한지 체크
                foreach (string eventID in curEnterNPCTrigger.GetComponent<EventTrigger>().eventIdList)
                {
                    if (CheckInteractionAvail(eventID))
                    {
                        Debug.Log("현재 정신세계 진입 가능한 이벤트임.");
                        //이벤트 실행 가능하다면 실행
                        StartCoroutine(EventManagerYKM.Instance.ExecuteEvent(eventID));
                        _progressBarUI.SetActive(true);
                        _startEnter = true;
                    }
                    else
                    {
                        Debug.Log("현재 정신세계 진입이 불가능함.");
                    }
                }
            }

            if (!InventoryManager.Instance.GetComponent<InventoryNavigator>().canEvidenceUse && !InventoryManager.Instance.isInventoryOpen && !UIManager.Instance._isDetailOpen && !UIManager.Instance._isInvestigateUIOpened &&_canEnter && isPlayerInMetanlWorld && Input.GetKeyDown(KeyCode.Space))
            {
                if (InventoryManager.Instance.IsAcquiredEvidence("evidence_001"))
                {
                    _progressBarUI.SetActive(true);
                    _startEnter = true;
                    if(EventManagerYKM.Instance.currentEventID == "Event_A008") StartCoroutine(EventManagerYKM.Instance.ExecuteEvent("Event_A009"));
                }
            }
            
            //진입시작했고, 완료되지 않았고, 스페이스를 계속 누르고 있다면
            if (_startEnter && !_isComplete && Input.GetKey(KeyCode.Space))
            {
                float value = _progressBarFill.FillAmount();
                EffectManager.Instance.StartMentalEffect(value);
                
                if (value >= 1f)
                {
                    _isComplete = true;
                    //씬 이동 함수 실행하면 됨.
                    if (curEnterNPCTrigger != null)
                    {
                        curEnterNPCTrigger.GetComponent<EnterPath>().StartEnterToPath();
                    }
                    else
                    {
                        StartEnterToPath("PrologueMap_real");
                        if (EventManagerYKM.Instance.currentEventID == "Event_A009")
                        {
                            StartCoroutine(EventManagerYKM.Instance.ExecuteEvent("Event_A010"));
                        }
                        else
                        {
                            StartCoroutine(EventManagerYKM.Instance.ExecuteEvent("Event_A028"));
                        }
                        
                    }
                }
            }
            else
            {
                //스페이스에서 손 때면, 현 상태에서 연출 멈추는 효과 구현 코드 여기에 작성되면 됨.
                //예시 : _uiManager.transitionAnimator.speed = 0;
            }
        }
    }

    void FixedUpdate()
    {
        //정신세게 진입이 시작되었다면
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
    }


    private void OnTriggerStay(Collider other)
    {
        //진입 시 바로 이벤트 실행되는 트리거에 진행하면
        if (other.CompareTag("EventTrigger"))
        {
            foreach (string eventID in other.GetComponent<EventTrigger>().eventIdList)
            {
                StartCoroutine(EventManagerYKM.Instance.ExecuteEvent(eventID));
            }
        }
        else if (other.CompareTag("EventInteractionTrigger"))
        {
            curInteractableTrigger = other.gameObject;
            foreach (string eventID in other.GetComponent<EventTrigger>().eventIdList)
            {
                if (CheckInteractionAvail(eventID))
                {
                    ShowInteractionMark();
                    isInteractObj = true;
                }
            }
        } //정신세계 진입 트리거에 들어가면
        else if (other.CompareTag("EventMentalEnterTrigger"))
        {
            curEnterNPCTrigger = other.gameObject;
            _canEnter = true;
            foreach (string eventID in other.GetComponent<EventTrigger>().eventIdList)
            {
                if(CheckInteractionAvail(eventID)) ShowInteractionMark();
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EventInteractionTrigger"))
        {
            curInteractableTrigger = null; 
            _interactionMark.SetActive(false);
            isInteractObj = false;
        }
        
        //정신세계 진입 트리거에 나가면
        if (other.CompareTag("EventMentalEnterTrigger"))
        {
            curEnterNPCTrigger = null;
            _canEnter = false;
            _interactionMark.SetActive(false);
        }
    }

    public void InitProgressBar()
    {
        _progressBarFill.InitFillAmount();
        _timer = 0f;
        _progressBarUI.SetActive(false);
        if (!_isComplete)
        {
            EffectManager.Instance.ResetMetalEffect();
            //여기에 연출 초기화 하는 코드 작성되면 됨.
            // 예시 : _uiManager.transitionAnimator.Play("TS_4_Normal_Reveal", 0, 0);
        }
        _startEnter = false;
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
            
            if ( String.IsNullOrEmpty(EventManagerYKM.Instance.nextEventID) || EventManagerYKM.Instance.nextEventID == id)
            {
                Debug.Log("nextEventID 관련해서는 만족함.");
                /*
                if (_event.CheckCondition())
                {
                    Debug.Log("checkInteractionAvail이 true로 리턴됨.");
                    return true;
                }
                */
            }
        }
        return false;
    }

    public Dictionary<string,string> GetPlayeCanUseEvidenceID()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
       
        //플레이어가 현재 상호작용 중인 트리거가 있어야 함.
        if (curInteractableTrigger != null)
        {
            //해당 트리거에서 이벤트 아이디 가져오기
            foreach (string eventID in curInteractableTrigger.GetComponent<EventTrigger>().eventIdList)
            {
                data.Add("eventID",eventID);
                EventStructure eventStructure = DataManager.Instance._events[eventID];
            
                /*
                //해당 이벤트가 분기점이 있는지 체크
                if (eventStructure.branch_Type)
                {
                    data[eventID] = eventStructure.branch_Element;
                }
                */
            }
            return data;
        }
        return data;
    }
    
    public void StartEnterToPath(string destination)
    {
        PlayerInteract.Instance.curEnterNPCTrigger = null;
        PlayerInteract.Instance._interactionMark.SetActive(false);
        PlayerInteract.Instance.InitProgressBar();
   
        PlayerInteract.Instance.isPlayerInMetanlWorld = false;
        SceneManager.LoadScene(destination);
    }
}
