using UnityEngine;

public class LockConditionStructure
{
    public string lock_condition_id;

    public void Lock()
    {
        switch (lock_condition_id)
        {
            case "lock_condition_001":
                LockPlayerMove();
                LockAllInteraction();
                break;
            case "lock_condition_002":
                LockPlayerMove();
                break;
            case "lock_condition_003":
                LockAllInteraction();
                break;
            case "lock_condition_004":
                LockEnterMentalWorld();
                break;
            case "lock_condition_005":
                LockComeBackToRealWorld();
                break;
            case "lock_condition_006":
                ForceQuitInteraction();
                break;
        }
    }

    void LockPlayerMove()
    {
        Debug.Log("플레이어 움직임 Lock");
    }

    void LockAllInteraction()
    {
        Debug.Log("모든 물체 및 캐릭터 상호작용 Lock");
    }

    void LockEnterMentalWorld()
    {
        Debug.Log("정신세계 입장 Lock");
    }

    void LockComeBackToRealWorld()
    {
        Debug.Log("현실 세계 돌아오기 Lock");
    }

    void ForceQuitInteraction()
    {
        Debug.Log("상호작용 강제 종료");
    }
}