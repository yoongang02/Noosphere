using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class EventManagerYKM : MonoBehaviour
{
    private static EventManagerYKM _instance;

    public static EventManagerYKM Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject("EventManagerYKM");
                _instance = singletonObject.AddComponent<EventManagerYKM>();
                DontDestroyOnLoad(singletonObject);
            }
            return _instance;
        }
    }
    
    //csv 파일 데이터들
    public Dictionary<string, EventStructure> _events = new Dictionary<string, EventStructure>();
    public Dictionary<string, LockConditionStructure> _lockConditions = new Dictionary<string, LockConditionStructure>();
    public Dictionary<string, EvidenceStructure> _evidences = new Dictionary<string, EvidenceStructure>();
    
    
    //추가 변수
    public string nextEventID = "";
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        InitializeData().Forget();
    }

    private async UniTaskVoid InitializeData()
    {
        _events = await LoadData<EventStructure>("Event");
        _lockConditions = await LoadData<LockConditionStructure>("Lock_condition");
        _evidences = await LoadData<EvidenceStructure>("Evidence");
        Debug.Log("Event 데이터 로드 완료");
        Debug.Log("Lock_Condition 데이터 로드 완료");
        Debug.Log("Evidence 데이터 로드 완료");
    }

    public async UniTask<Dictionary<string, T>> LoadData<T>(string fileName) where T : new()
    {
        CSVParserYKM parser = new CSVParserYKM();
        return await parser.Parse<T>(fileName);
    }

    //이벤트 실행
    public void ExecuteEvent(string eventID)
    {
        if (!_events.ContainsKey(eventID))
        {
            Debug.Log(eventID + " 이벤트가 존재하지 않음.");
            return;
        }

        if (eventID != nextEventID)
        {
            Debug.Log("현재 실행되어야 하는 이벤트는 " + nextEventID + "입니다.");
            return;
        }

        EventStructure eventStructure = _events[eventID];

        //실행 조건 만족하는지 체크
        if (eventStructure.CheckCondition())
        {
            //락 조건이 있다면, 락 걸기
            if (_lockConditions.ContainsKey(eventStructure.lock_condition_id))
            {
                _lockConditions[eventStructure.lock_condition_id].Lock();
            }
            //결과 실행하기
            foreach (var resultID in eventStructure.resultIDs)
            {
                string resultType = resultID.Substring(0, resultID.IndexOf('_'));
                if (resultType == "dialogue")
                {
                    StartDialogue(resultID);
                }
                else if (resultType == "effect")
                {
                    StartEffect(resultID);
                }
            }
            
        }
    }

    //dialogue 시작
    void StartDialogue(string dialogueID)
    {
        Debug.Log(dialogueID + " 대화 시작");
    }

    //effect 시작
    void StartEffect(string effectID)
    {
        Debug.Log(effectID + " 효과 시작");
    }
}