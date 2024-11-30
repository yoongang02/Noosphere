using Unity.VisualScripting;
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
    public bool branch_Type;
    public string branch_Element;
    public string branch_True;
    public string branch_False;
    public string evidence_id;
    public string lock_condition_id; //이벤트 실행 시 락되는 조건
    public string location_id;
    public string next_Event_id;
    
    //이벤트 실행 여부
    public bool isExecuted = false;
    
    
    //조건 충족하는지
    public bool IsConditionMet(string conditionID)
    {
        //조건이 비어있으면 충족함으로 리턴
        if (string.IsNullOrEmpty(conditionID)) return true;
        
        bool isMet = false;
        
        //조건 id 앞에 !(not)이 붙어있는지 체크
        char boolType = conditionID[0];
        string conditionType;
        string id;
        
        if (boolType == '!')
        {
            conditionType = conditionID.Substring(1, conditionID.IndexOf('_')-1);
            id = conditionID.Substring(1, conditionID.Length-1);
        }
        else
        {
            conditionType = conditionID.Substring(0, conditionID.IndexOf('_'));
            id = conditionID;
        }
        
        Debug.Log($"boolType : {boolType} , conditonType : {conditionType} , id : {id}");
        
        if (conditionType == "evidence")
        {
            //증거 인벤토리에 있는지 확인, 혹은 사용했는지 구분
            EvidenceStructure evidence = DataManager.Instance._evidences[id];
            char canUse = evidence.can_Use;
            char acquisitionType = evidence.acquisition_Type;

            if (canUse == 'Y')
            {
                //아이템을 사용했는지 체크
            }
            else if (canUse == 'N')
            {
                //사용할 수 없는 아이템이나, 인벤토리에 획득 가능한 아이템이면 인벤토리에 있는지 체크
                if (acquisitionType == 'Y')
                {
                    if (InventoryManager.Instance.IsAcquiredEvidence(id))
                    {
                        isMet = true;
                    }
                    else
                    {
                        Debug.Log(id+"가 인벤토리 내에 존재하지 않습니다.");
                    }
                }
                else if(acquisitionType == 'N')
                {
                    //접근 횟수로 체크(나중에 필요하면 구현)
                }
            }
        }
        else if (conditionType == "Event")
        {
            if (DataManager.Instance._events.ContainsKey(id) && DataManager.Instance._events[id].isExecuted)
            {
                isMet = true;
            }
        }
        else if (conditionType == "input")
        {
            if (DataManager.Instance._input.ContainsKey(id) && DataManager.Instance._input[id].isSolved)
            {
                isMet = true;
            }
        }
        
        //혹시 not 조건이 있다면 반대로 값을 출력
        if (boolType == '!') return !isMet;
        
        return isMet;
    }
}