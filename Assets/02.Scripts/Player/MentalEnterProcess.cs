using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MentalEnterProcess : MonoBehaviour
{
    //정신세계 진입 관련 변수
    [Header("정신세계 진입")]
    [SerializeField] private float _enterTime = 5.0f;
    [SerializeField] bool _startEnter = false;
    [SerializeField] float _timer = 0f;
    public bool isComplete = false;
    public MentalStructure mentalInfo;
    [SerializeField] private string _comebackEventId;
    
    [Header("정신세계 진입 UI")]
    [SerializeField] private GameObject _progressBarUI;
    [SerializeField] private EnterProgressBar _progressBarFill;
    
    void Update()
    {
        if (!UIManager.Instance.IsAnyUIOpen())
        {
            //현실세계 -> 정신세계 진입
            if (!_startEnter && PlayerInteract.Instance.canInteract && PlayerInteract.Instance.mentalTrigger != null && Input.GetKeyDown(KeyCode.Space))
            {
                foreach (string eventID in PlayerInteract.Instance.mentalTrigger.GetComponent<EventTrigger>().eventIdList)
                {
                    if (PlayerInteract.Instance.CheckInteractionAvail(eventID))
                    {
                        //이벤트 실행 가능하다면 실행
                        StartCoroutine(EventManagerYKM.Instance.ExecuteEvent(eventID));
                        PlayerInteract.Instance.HideInteractionMark();
                        break;
                    }
                    else
                    {
                        Debug.Log($"{eventID} 는 현재 정신세계 진입이 불가능함.");
                    }
                }
            }
            
            //정신세계 -> 현실세계 진입
            if (!_startEnter && PlayerInteract.Instance.canInteract && mentalInfo != null &&
                PlayerInteract.Instance.isInMental && Input.GetKeyDown(KeyCode.Space))
            {
                if (PlayerInteract.Instance.CheckInteractionAvail(_comebackEventId))
                {
                    //이벤트 실행 가능하다면 실행
                    StartCoroutine(EventManagerYKM.Instance.ExecuteEvent(_comebackEventId));
                    return;
                }
                else
                {
                    Debug.Log($"{mentalInfo.combackEventId} 는 현재 현실세계 진입이 불가능함.");
                }
            }
            
            //진입시작했고, 완료되지 않았고, 스페이스를 계속 누르고 있다면
            if (_startEnter && !isComplete)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    float value = _progressBarFill.FillAmount();
                    EffectManager.Instance.StartMentalEffect(value);
                
                    if (value >= 1f)
                    {
                        isComplete = true;
                    }
                }
                else
                {
                    //스페이스에서 손 때면, 현 상태에서 게이지 감소하는 코드
                    float value = _progressBarFill.DrainAmount();
                    EffectManager.Instance.StartMentalEffect(value);

                    if (value <= 0)
                    {
                        FailEnter();
                    }
                }
            }
        }
        
    }
    void FixedUpdate()
    {
        //정신세게 진입이 시작되었다면
        if (_startEnter)
        {
            _timer += Time.fixedDeltaTime;

            if (isComplete)
            {
                Debug.Log($"#{mentalInfo.mentalId} 시간 내에 진입 완료.");
                CompleteEnter();
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                Debug.Log($"#{mentalInfo.mentalId} 진입 중에 움직여서 초기화 됨.");
                FailEnter();
            }

            if (_timer >= _enterTime)
            {
                if (!isComplete)
                {
                    Debug.Log($"#{mentalInfo.mentalId} 시간 내에 진입하지 못함.");
                    FailEnter();
                }
            }
        }
    }
    
    public void InitProgressBar()
    {
        _progressBarUI.SetActive(false);
        EffectManager.Instance.ResetMetalEffect();
        
        _timer = 0f;
        _progressBarFill.InitFillAmount();
        isComplete = false;
        _startEnter = false;
    }

    public void StartEnter(string mentalId)
    {
        if (DataManager.Instance._mental.ContainsKey(mentalId))
        {
            mentalInfo = DataManager.Instance._mental[mentalId];
            if (!PlayerInteract.Instance.isInMental) _comebackEventId = mentalInfo.combackEventId;
            _startEnter = true;
            _progressBarUI.SetActive(true);
        }
        else
        {
            Debug.Log($"${mentalId} 키의 MentalStructure이 존재하지 않습니다.");
        }
       
    }

    void CompleteEnter()
    {
        //씬 이동
        StartCoroutine(LoadSceneAsync(mentalInfo.destination));
        PlayerInteract.Instance.isInMental = !PlayerInteract.Instance.isInMental;
        
        //이동 성공 시 결과가 있다면 실행
        if (!string.IsNullOrEmpty(mentalInfo.mentalTrueResult))
        {
            StartCoroutine(EventManagerYKM.Instance.ExecuteEvent(mentalInfo.mentalTrueResult));
        }

        if (!PlayerInteract.Instance.isInMental)
        {
            mentalInfo = null;
            _comebackEventId = "";
        }
        
        //바 초기화
        InitProgressBar();
        Debug.Log($"{PlayerInteract.Instance.isInMental} 정신세계에 도착했어. 도착 여부는 제대로 반영되었나?");
    }

    void FailEnter()
    {
        //이동 실패 시 결과가 있다면 실행
        foreach (var result in mentalInfo.mentalFalseResults)
        {
            if (!string.IsNullOrEmpty(result))
            {
                StartCoroutine(EventManagerYKM.Instance.ExecuteEvent(result));
            }
        }
        
        //바 초기화
        InitProgressBar();

        if (EventManagerYKM.Instance.currentEventID == "Event_A026")
        {
            EventManagerYKM.Instance.nextEventID = "Event_A026";
            DataManager.Instance._events["Event_A026"].isExecuted = false;
        }

        if (EventManagerYKM.Instance.currentEventID == "Event_A009")
        {
            EventManagerYKM.Instance.nextEventID = "Event_A009";
            DataManager.Instance._events["Event_A009"].isExecuted = false;
        }
    }
    
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        
        while (!operation.isDone)
        {
            yield return null;
        }
    }
}
