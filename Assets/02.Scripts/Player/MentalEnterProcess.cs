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
    [SerializeField] private MentalStructure _mentalInfo;
    
    [Header("정신세계 진입 UI")]
    [SerializeField] private GameObject _progressBarUI;
    [SerializeField] private EnterProgressBar _progressBarFill;
    
    void Update()
    {
        if (!UIManager.Instance.IsAnyUIOpen())
        {
            if (PlayerInteract.Instance.canInteract && PlayerInteract.Instance.mentalTrigger != null && Input.GetKeyDown(KeyCode.Space))
            {
                foreach (string eventID in PlayerInteract.Instance.mentalTrigger.GetComponent<EventTrigger>().eventIdList)
                {
                    if (PlayerInteract.Instance.CheckInteractionAvail(eventID))
                    {
                        //이벤트 실행 가능하다면 실행
                        StartCoroutine(EventManagerYKM.Instance.ExecuteEvent(eventID));
                    }
                    else
                    {
                        Debug.Log("현재 정신세계 진입이 불가능함.");
                    }
                }
            }
            
            //진입시작했고, 완료되지 않았고, 스페이스를 계속 누르고 있다면
            if (_startEnter && !isComplete && Input.GetKey(KeyCode.Space))
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
                //스페이스에서 손 때면, 현 상태에서 연출 멈추는 효과 구현 코드 여기에 작성되면 됨.
                float value = _progressBarFill.DrainAmount();
                EffectManager.Instance.StartMentalEffect(value);

                if (value <= 0)
                {
                    FailEnter();
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
                Debug.Log($"#{_mentalInfo.mentalId} 시간 내에 진입 완료.");
                CompleteEnter();
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                Debug.Log($"#{_mentalInfo.mentalId} 진입 중에 움직여서 초기화 됨.");
                FailEnter();
            }

            if (_timer >= _enterTime)
            {
                if (!isComplete)
                {
                    Debug.Log($"#{_mentalInfo.mentalId} 시간 내에 진입하지 못함.");
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
        _startEnter = false;
    }

    public void StartEnter(MentalStructure mentalStructure)
    {
        _mentalInfo = mentalStructure;
        _startEnter = true;
        _progressBarUI.SetActive(true);
    }

    void CompleteEnter()
    {
        //바 초기화
        InitProgressBar();
        //씬 이동
        StartCoroutine(LoadSceneAsync(_mentalInfo.destination));
        //이동 성공 시 결과가 있다면 실행
        if (!string.IsNullOrEmpty(_mentalInfo.mentalTrueResult))
        {
            StartCoroutine(EventManagerYKM.Instance.ExecuteEvent(_mentalInfo.mentalTrueResult));
        }
    }

    void FailEnter()
    {
        //바 초기화
        InitProgressBar();
        //이동 실패 시 결과가 있다면 실행
        foreach (var result in _mentalInfo.mentalFalseResults)
        {
            if (!string.IsNullOrEmpty(result))
            {
                StartCoroutine(EventManagerYKM.Instance.ExecuteEvent(result));
            }
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
