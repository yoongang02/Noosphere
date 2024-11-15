using UnityEngine;

public class EventStructure
{
    //csv 필드
    public string event_id;
    public string description;
    public bool repeat_Type; //반복 여부 true,false
    public string condition_Type; //조건 타입 or,and
    public string[] conditions;
    public string[] resultIDs;
    public string evidence_id;
    public string lock_condition_id; //이벤트 실행 시 락되는 조건
    public string location_id;
    public string next_Event_id;
    
    //이벤트 실행 여부
    public bool isExecuted = false;
    
    //조건 체크
    public bool CheckCondition()
    {
        //반복 불가능인데 실행 횟수가 0 초과라면 실행 불가능
        if (!repeat_Type && isExecuted)
        {
            Debug.Log("이미 실행된 이벤트이며, 반복 불가능한 이벤트입니다.");
            return false;
        }
        //조건 타입 and 인데, 조건을 만족하지 못했다면
        if (condition_Type == "and")
        {
            foreach (var condition in conditions)
            {
                if (!IsConditionMet(condition))
                {
                    return false;
                }
            }   
        }
        Debug.Log(this.event_id + " 이벤트 실행 조건을 만족함.");
        return true;
    }
    
    //조건 충족하는지
    private bool IsConditionMet(string conditionID)
    {
        bool isMet = false;
        //Condition Manager
        string conditionType = conditionID.Substring(0, conditionID.IndexOf('_'));
        if (conditionType == "evidence")
        {
            //증거 인벤토리에 있는지 확인, 혹은 
            isMet = true;
        }
        else if (conditionType == "dialogue")
        {
            
        }
        return isMet;
    }
}