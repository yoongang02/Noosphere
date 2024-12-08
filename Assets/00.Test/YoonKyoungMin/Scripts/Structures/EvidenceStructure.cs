using System;
using UnityEngine;
public class EvidenceStructure
{
    //기본 csv 파일 내 필드 값들
    public string evidenceId;
    public string evidenceName;
    public char evidenceType;
    public string shapeType; //증거물 타입 종류. OnePage : 한페이지, TwoPage : 두페이지, Object : 물체
    public char acquisitionType;
    public char canUse;
    public string unlockCondition;
    public string evidenceTextDisplay;
    public string artresourceId;
    public string subEvidenceId;
    public string subEvidenceAcquisitionType;
    public int acquisitionPageNum;
    public string acquisitionPageResultId;
    
    //추가적으로 필요한 필드 값
    public int accessCnt = 0; //증거물 접근 횟수

    //획득할 수 있는지
    public void AcquireEvidence()
    {
        //언락 조건이 있다면 언락 조건을 만족했는지 체크
        if (!IsUnLockConditionMet())
        {
            Debug.Log(unlockCondition + " 언락 조건을 만족하지 못하여, 획득 불가능");
            return;
        }
        //언락 조건을 만족하여 획득할 수 있다면
        if (acquisitionType == 'Y') //인벤토리에 획득할 수 있다면
        {
            //이미 획득했다면 증거 정보 열면 안됨.
            if (InventoryManager.Instance.IsAcquiredEvidence(this.evidenceId))
            {
                Debug.Log(evidenceId + " 증거물은 이미 인벤토리에 획득된 증거물입니다.");
            }
            else
            {
                //획득하지 않은 증거라면 인벤토리에 획득.
                Debug.Log(evidenceId + " 증거물은 인벤토리에 존재하지 않는 증거물입니다.");
                InventoryManager.Instance.AddEvidence(this);
            }
        }
        else if (acquisitionType == 'N') //인벤토리에 획득할 수 없다면
        {
            accessCnt++;
            Debug.Log(evidenceId + " 증거물은 획득할 수 없는 증거물입니다.");
        }
    }

    bool IsUnLockConditionMet()
    {
        //이벤트 매니저 내에서 해당 unlock 조건의 이벤트가 실행되었는지 확인
        if (unlockCondition == "" ||
            DataManager.Instance._events.ContainsKey(unlockCondition) && DataManager.Instance._events[unlockCondition].isExecuted)
        {
            Debug.Log("unlock 조건 만족한 것을 확인");
            return true;
        }
        return false;
    }
    
    //현재 상세보기하고 있는 증거물에 획득 가능한 서브 증거물이 있는지
    public bool CheckSubEvidence()
    {
        if (!String.IsNullOrEmpty(subEvidenceId)) return true;
        return false;
    }
}
