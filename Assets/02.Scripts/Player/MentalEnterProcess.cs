using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MentalEnterProcess : MonoBehaviour
{
    //정신세계 진입 관련 변수
    [Header("정신세계 진입 관련 조건 변수")]
    public bool isPlayerInMetanlWorld = false;
    [SerializeField] private float _enterTime = 5.0f;
    public GameObject curEnterNPCTrigger;
    [SerializeField] bool _startEnter = false;
    public bool _isComplete = false;
    [SerializeField] float _timer = 0f;
    
    [Header("정신세계 진입 관련 UI 변수")]
    [SerializeField] private GameObject _progressBarUI;
    [SerializeField] private EnterProgressBar _progressBarFill;


    void Update()
    {
         /*
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
            */

            /*
            if (!InventoryManager.Instance.GetComponent<InventoryNavigator>().canEvidenceUse && !InventoryManager.Instance.isInventoryOpen && !UIManager.Instance._isDetailOpen && !UIManager.Instance._isInvestigateUIOpened &&_canEnter && isPlayerInMetanlWorld && Input.GetKeyDown(KeyCode.Space))
            {
                if (InventoryManager.Instance.IsAcquiredEvidence("evidence_001"))
                {
                    _progressBarUI.SetActive(true);
                    _startEnter = true;
                    if(EventManagerYKM.Instance.currentEventID == "Event_A008") StartCoroutine(EventManagerYKM.Instance.ExecuteEvent("Event_A009"));
                }
            }
            */
            
            //진입시작했고, 완료되지 않았고, 스페이스를 계속 누르고 있다면
            /*
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
            */
    }
    void FixedUpdate()
    {
        /*
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
        */
    }
    
    /*
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
    */
}
