using System.Collections;
using DG.Tweening;
using UnityEngine;

public class LockConditionStructure
{
    public string lockConditionId;

    public void Lock()
    {
        switch (lockConditionId)
        {
            case "Lock_condition_001":
                LockPlayerMove();
                LockAllInteraction();
                break;
            case "Lock_condition_002":
                LockPlayerMove();
                break;
            case "Lock_condition_003":
                LockAllInteraction();
                break;
            case "Lock_condition_004":
                LockEnterMentalWorld();
                break;
            case "Lock_condition_005":
                LockComeBackToRealWorld();
                break;
            case "Lock_condition_006":
                ForceQuitInteraction();
                break;
            case "Lock_condition_007":
                BreakMirror();
                break;
        }
    }

    public void UnLock()
    {
        switch (lockConditionId)
        {
            case "Lock_condition_001":
                UnLockPlayerMove();
                UnLockAllInteraction();
                break;
            case "Lock_condition_002":
                UnLockPlayerMove();
                break;
            case "Lock_condition_003":
                UnLockAllInteraction();
                break;
            case "Lock_condition_004":
                UnLockEnterMentalWorld();
                break;
            case "Lock_condition_005":
                UnLockComeBackToRealWorld();
                break;
            case "Lock_condition_006":
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
        PlayerInteract.Instance.HideInteractionMark();
        Debug.Log("모든 물체 및 캐릭터 상호작용 Lock");
    }
    
    void UnLockAllInteraction()
    {
        PlayerInteract.Instance.canInteract = true;
        Debug.Log("모든 물체 및 캐릭터 상호작용 UnLock");
    }

    void LockEnterMentalWorld()
    {
        MentalEnterProcess mentalEnterProcess = PlayerInteract.Instance.GetComponent<MentalEnterProcess>();
        mentalEnterProcess.LockEnterProcess();
        Debug.Log("정신세계 입장 Lock");
    }
    
    void UnLockEnterMentalWorld()
    {
        MentalEnterProcess mentalEnterProcess = PlayerInteract.Instance.GetComponent<MentalEnterProcess>();
        mentalEnterProcess.UnLockEnterProcess();
        Debug.Log("정신세계 입장 UnLock");
    }

    void LockComeBackToRealWorld()
    {
        Debug.Log("현실 세계 돌아오기 Lock");
        LockEnterMentalWorld();
    }
    
    void UnLockComeBackToRealWorld()
    {
        Debug.Log("현실 세계 돌아오기 Lock");
        UnLockEnterMentalWorld();
    }

    void ForceQuitInteraction()
    {
        Debug.Log("상호작용 강제 종료");
        UIManager.Instance.CloseAllUI();
    }

    void BreakMirror()
    {
        PlayerInteract.Instance.GetComponent<MentalEnterProcess>().ForceQuitMentalProcess();
    }
}