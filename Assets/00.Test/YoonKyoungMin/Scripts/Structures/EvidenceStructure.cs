public class EvidenceStructure
{
    //기본 csv 파일 내 필드 값들
    public string evidence_id;
    public string evidence_name;
    public char acquisition_Type;
    public string unlock_Condition;
    public string evidence_Text_Display;
    public string artresource_id;
    
    //추가적으로 필요한 필드 값
    private int _accessCnt = 0; //증거물 접근 횟수

    //획득할 수 있는지
    public void CanAcquire()
    {
        //언락 조건이 있다면 언락 조건을 만족했는지 체크
        if (!IsUnLockConditionMet())
        {
            //Debug.Log(unlock_Condition + " 언락 조건을 만족하지 못하여, 획득 불가능");
            return;
        }
        //언락 조건을 만족하여 획득할 수 있다면
        if (acquisition_Type == 'Y') //인벤토리에 획득할 수 있다면
        {
            
        }
        else if (acquisition_Type == 'N') //인벤토리에 획득할 수 없다면
        {
            _accessCnt++;
        }
    }

    bool IsUnLockConditionMet()
    {
        //이벤트 매니저 내에서 해당 unlock 조건의 이벤트가 실행되었는지 확인
        if (EventManagerYKM.Instance._events.ContainsKey(unlock_Condition) && EventManagerYKM.Instance._events[unlock_Condition].isExecuted)
        {
            //Debug.Log("unlock 조건 만족한 것을 확인");
            return true;
        }
        return false;
    }
}
