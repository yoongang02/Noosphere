using System;
using UnityEngine;
public class EvidenceStructure
{
    //기본 csv 파일 내 필드 값들
    public string evidence_id;
    public string evidence_name;
    public char evidence_Type;
    public string shape_Type; //증거물 타입 종류. T : text, P : Prefab
    public char acquisition_Type;
    public char can_Use;
    public string unlock_Condition;
    public string evidence_Text_Display;
    public string artresource_id;
    public string sub_evidence_id;
    public string sub_Evidence_Acquisition_Type;
    public int acquisition_Page_Num;
    public string acquisition_Page_Result_id;
    
    //추가적으로 필요한 필드 값
    public int accessCnt = 0; //증거물 접근 횟수

    //획득할 수 있는지
    public void AcquireEvidence()
    {
        //언락 조건이 있다면 언락 조건을 만족했는지 체크
        if (!IsUnLockConditionMet())
        {
            Debug.Log(unlock_Condition + " 언락 조건을 만족하지 못하여, 획득 불가능");
            return;
        }
        //언락 조건을 만족하여 획득할 수 있다면
        if (acquisition_Type == 'Y') //인벤토리에 획득할 수 있다면
        {
            //이미 획득했다면 증거 정보 열면 안됨.
            if (InventoryManager.Instance.IsAcquiredEvidence(this.evidence_id))
            {
                Debug.Log(evidence_id + " 증거물은 이미 인벤토리에 획득된 증거물입니다.");
            }
            else
            {
                //획득하지 않은 증거라면 인벤토리에 획득.
                Debug.Log(evidence_id + " 증거물은 인벤토리에 존재하지 않는 증거물입니다.");
                InventoryManager.Instance.AddEvidence(this);
            }
        }
        else if (acquisition_Type == 'N') //인벤토리에 획득할 수 없다면
        {
            accessCnt++;
            Debug.Log(evidence_id + " 증거물은 획득할 수 없는 증거물입니다.");
        }
    }

    bool IsUnLockConditionMet()
    {
        //이벤트 매니저 내에서 해당 unlock 조건의 이벤트가 실행되었는지 확인
        if (unlock_Condition == "" ||DataManager.Instance._events.ContainsKey(unlock_Condition) && DataManager.Instance._events[unlock_Condition].isExecuted)
        {
            Debug.Log("unlock 조건 만족한 것을 확인");
            return true;
        }
        return false;
    }
    
    //현재 상세보기하고 있는 증거물에 획득 가능한 서브 증거물이 있는지
    public bool CheckSubEvidence()
    {
        if (!String.IsNullOrEmpty(sub_evidence_id)) return true;
        return false;
    }
}
