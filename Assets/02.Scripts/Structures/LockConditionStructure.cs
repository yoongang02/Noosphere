using UnityEngine;

public class LockConditionStructure
{
    public string lockConditionId;

    public void Lock()
    {
        switch (lockConditionId)
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

    public void UnLock()
    {
        switch (lockConditionId)
        {
            case "lock_condition_001":
                UnLockPlayerMove();
                UnLockAllInteraction();
                break;
            case "lock_condition_002":
                UnLockPlayerMove();
                break;
            case "lock_condition_003":
                UnLockAllInteraction();
                break;
        }
    }

    void LockPlayerMove()
    {
        PlayerController.Instance.canMove = false;
        Debug.Log("플레이어 움직임 Lock");
    }

    void UnLockPlayerMove()
    {
        PlayerController.Instance.canMove = true;
        Debug.Log("플레이어 움직임 UnLock");
    }

    void LockAllInteraction()
    {
        PlayerInteract.Instance.canInteract = false;
        Debug.Log("모든 물체 및 캐릭터 상호작용 Lock");
    }
    
    void UnLockAllInteraction()
    {
        PlayerInteract.Instance.canInteract = true;
        Debug.Log("모든 물체 및 캐릭터 상호작용 UnLock");
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
        UIManager.Instance.CloseAllUI();
    }
}