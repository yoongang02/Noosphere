using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;

class EventStructure
{
    //csv 필드
    public string event_id;
    public string description;
    public bool repeat_Type; //반복 여부 true,false
    public string condition_Type; //조건 타입 or,and
    public string condition1;
    public string condition2;
    public string condition3;
    public string result_id_1;
    public string result_id_2;
    public string result_id_3;
    public string evidence_id;
    public string lock_condition_id; //이벤트 실행 시 락되는 조건
    public string location_id;
    public string next_Event_id;
    
    //이벤트 실행 횟수
    public int executionCnt;
    
    //조건 체크
    public bool CheckCondition()
    {
        //반복 불가능인데 실행 횟수가 0 초과라면 실행 불가능
        if (!repeat_Type && executionCnt > 0) return false;
        //조건 타입 and 인데, 조건을 만족하지 못했다면
        if (condition_Type == "and" && !IsConditionMet(condition1))
        {
            return false;
        }
        
        return true;
    }
    
    //조건 충족하는지
    private bool IsConditionMet(string conditionID)
    {
        //Condition Manager
        return true;
        return false;
    }
}

public class EventManagerYKM : MonoBehaviour
{
    private Dictionary<string, EventStructure> _events = new Dictionary<string, EventStructure>();

    void Start()
    {
        LoadEvents("Event").Forget();
    }

    //이벤트 로드
    async UniTask LoadEvents(string fileName)
    {
        CSVParserYKM parser = new CSVParserYKM();
        _events = await parser.Parse<EventStructure>(fileName);
    }

    //이벤트 실행
    public void ExecuteEvent(string eventID)
    {
        if (!_events.ContainsKey(eventID))
        {
            Debug.Log(eventID + " 이벤트가 존재하지 않음.");
            return;
        }

        EventStructure eventStructure = _events[eventID];
        Debug.Log(eventStructure.description);
        //실행 조건 만족하는지 체크
        if (eventStructure.CheckCondition())
        {
            string resultType = eventStructure.result_id_1.Substring(0, eventStructure.result_id_1.IndexOf('_'));
            if (resultType == "dialogue")
            {
                StartDialogue(eventStructure.result_id_1);
            }
            else if (resultType == "effect")
            {
                StartEffect(eventStructure.result_id_1);
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