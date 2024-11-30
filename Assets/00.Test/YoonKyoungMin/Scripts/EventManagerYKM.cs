using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class EventManagerYKM : Singleton<EventManagerYKM>
{
    //스테이지 번호
    public enum ChapterInfo
    {
        Prologue,
        Stage1,
        Stage2
    }
    public ChapterInfo curStageInfo;
    
    //다음 이벤트 정보
    public string startEventID;
    public string currentEventID;
    public string nextEventID = "";
    
    void Awake()
    {
        //게임 시작 시, 스테이지 정보 초기화
        curStageInfo = ChapterInfo.Prologue;
        nextEventID = startEventID;
    }

    void Start()
    {
        StartCoroutine(ExecuteEvent(startEventID));
    }

    //이벤트 실행
    public IEnumerator ExecuteEvent(string eventID)
    {
        //실행하는 이벤트가 이벤트 목록 내에 존재하는지 체크
        if (!DataManager.Instance._events.ContainsKey(eventID))
        {
            Debug.Log(eventID + " 이벤트가 존재하지 않음.");
            yield return null;
        }
        
        //nextEventID가 비어있지 않은데, 실행하고자 하는 이벤트ID와 같지 않다면
        if (!String.IsNullOrEmpty(nextEventID) && nextEventID != eventID)
        {
            Debug.Log("현재 실행되어야 하는 이벤트는 " + nextEventID + "입니다.");
            yield return null;
        }

        
        EventStructure eventStructure = DataManager.Instance._events[eventID];
        
        //1. LockCondition 실행
        string lockConditionID = eventStructure.lock_condition_id;
        LockConditionStructure lockCondition = DataManager.Instance._lockConditions[lockConditionID];
        lockCondition.Lock();
        
        //2. 반복 가능한 이벤트인지 체크
        //반복 불가능인데 이미 실행된 이벤트라면 실행 불가능
        if (!eventStructure.repeat_Type && eventStructure.isExecuted)
        {
            Debug.Log(eventID + "는 이미 실행된 이벤트이며, 반복 불가능한 이벤트입니다.");
            
            /*
            //반복 불가능할 경우의 결과 출력
            if (!String.IsNullOrEmpty(eventStructure.repeatFalseResult))
            {
                //repeatFalseResult 실행
                ExecuteEvent(eventStructure.repeatFalseResult);
            }
            */
            
            //락 해제
            lockCondition.UnLock();
            yield return null;
        }
        
        //3. ConditionType 체크
        if (eventStructure.condition_Type == "and")
        {
            //3-1. Condition 만족하는지 체크
            //각 조건을 만족하는지 체크
            foreach (var condition in eventStructure.conditions)
            {
                if (!eventStructure.IsConditionMet(condition))
                {
                    //해당 condition에 대한 falseResult 실행 및 실행 될 때까지 기다리기
                    //ExecuteEvent(eventStructure.conditionFalseResultN);
                    //출력 후 이벤트 중간 종료
                    CloseEventFailure(eventStructure);
                    yield return null;
                }
            }
        }
        else if(eventStructure.condition_Type != "or")
        {
            Debug.Log(eventID +  "의 conditionType이 올바르지 않습니다.");
            yield return null;
        }
        
        //4. evidenceID가 비어있지 않으면 증거물 습득
        if (!String.IsNullOrEmpty(eventStructure.evidence_id) && DataManager.Instance._evidences.ContainsKey(eventStructure.evidence_id))
        {
            // 증거물 조사 UI 띄우기
            EvidenceStructure evidence = DataManager.Instance._evidences[eventStructure.evidence_id];
            UIManager.Instance.OpenInvestigateUI(evidence);

            // UI에서 입력을 기다림
            yield return new WaitUntil(() => !UIManager.Instance.isSelecting);
        }
        
        //5. 결과들 실행하기
        
        /*
        //실행 조건 만족하는지 체크
        if (eventStructure.CheckCondition())
        {
            yield return HandleEventWithEvidence(eventStructure);
        }
        
        //실행이 모두 끝나면 nextEventID 갱신하기
        yield return new WaitUntil(() => eventStructure.isExecuted);
        Debug.Log(eventID + "이벤트 실행 완료. nextEventID 갱신");
        nextEventID = eventStructure.next_Event_id;
        
        //락 조건 해제
        if (DataManager.Instance._lockConditions.ContainsKey(lockConditionId))
        {
            DataManager.Instance._lockConditions[lockConditionId].UnLock();
        }
        //이벤트 트리거를 삭제해야하는 트리거라면 확인 후 삭제
        GameObject mentalTrigger = PlayerInteract.Instance.curEnterNPCTrigger;
        GameObject interactableTrigger = PlayerInteract.Instance.curInteractableTrigger;
        
        if (mentalTrigger != null)
        {
            foreach (string key in mentalTrigger.GetComponent<EventTrigger>().eventIdList)
            {
                if((key == eventID) && mentalTrigger.GetComponent<EventTrigger>().canDestroyWhenExecutionComplete) Destroy(mentalTrigger);
            }
        }

        if (interactableTrigger != null)
        {
            foreach (string key in interactableTrigger.GetComponent<EventTrigger>().eventIdList)
            {
                if ((key == eventID) && interactableTrigger.GetComponent<EventTrigger>().canDestroyWhenExecutionComplete) Destroy(interactableTrigger);
            }
        }
        */
    }
    
    IEnumerator HandleEventWithEvidence(EventStructure eventStructure)
    {
        currentEventID = eventStructure.event_id;
        
        //락 조건이 있다면, 락 걸기
        if (DataManager.Instance._lockConditions.ContainsKey(eventStructure.lock_condition_id))
        {
            DataManager.Instance._lockConditions[eventStructure.lock_condition_id].Lock();
        }
        
        //분기 조건 체크 후, 분기에 따라 진행
        if (eventStructure.branch_Type)
        {
            //분기 조건이 input인지
            //분기 조건이 evidence인지
            string branchType = eventStructure.branch_Element.Substring(0, eventStructure.branch_Element.IndexOf('_'));
            string branchResult = "";
            if (branchType == "input")
            {
                InputFieldStructure input = DataManager.Instance._input[eventStructure.branch_Element];
                Debug.Log(eventStructure.branch_Element + "에 대해서 분기 체크 시작");
                branchResult = input.isSolved ? eventStructure.branch_True : eventStructure.branch_False;
                yield return ExecuteResult(branchResult);
            }
            else if (branchType == "evidence")
            {
                Debug.Log(eventStructure.branch_Element + "에 대해서 분기 체크 시작");
                /*
                branchResult = InventoryManager.Instance.IsAcquiredEvidence(eventStructure.branch_Element)
                    ? eventStructure.branch_True
                    : eventStructure.branch_False;
                    */
                // branchResult = eventStructure.branch_False;
                // yield return ExecuteResult(branchResult);
            }
            else if (branchType == "mental")
            {
                //진입 성공할 때까지 기다리기
                yield return new WaitUntil(() => PlayerInteract.Instance._isComplete);
                PlayerInteract.Instance._isComplete = false;
                branchResult = eventStructure.branch_True;
                if (!String.IsNullOrEmpty(branchResult))
                {
                    StartCoroutine(ExecuteResult(eventStructure, branchResult));
                }
            }
        }
        //결과 실행하기
        foreach (var resultID in eventStructure.resultIDs)
        {
            yield return ExecuteResult(resultID);
        }
        
        // 증거물 조사 UI 띄우기
        if (DataManager.Instance._evidences.ContainsKey(eventStructure.evidence_id))
        {
            Debug.Log("증거물 조사 띄우기 : " + eventStructure.evidence_id);
            EvidenceStructure evidence = DataManager.Instance._evidences[eventStructure.evidence_id];
            UIManager.Instance.OpenInvestigateUI(evidence);

            // UI에서 입력을 기다림
            yield return new WaitUntil(() => !UIManager.Instance.isSelecting);

            if (UIManager.Instance.isEvidenceAcquired)
            {
                eventStructure.isExecuted = true;
            }
        }
        else
        {
            eventStructure.isExecuted = true;
        }
    }

    //dialogue 시작
    void StartDialogue(string dialogueID)
    {
        Debug.Log(dialogueID + " 대화 시작");
        DialogueManager.Instance.SetDialogue(dialogueID);
    }

    //effect 시작
    void StartEffect(string effectID)
    {
        Debug.Log(effectID + " 효과 시작");
        EffectManager.Instance.SetEffect(effectID);
    }
    
    //결과 수행
    IEnumerator ExecuteResult(EventStructure _event, string resultID)
    {
        if (!string.IsNullOrEmpty(resultID))
        {
            string resultType = resultID.Substring(0, resultID.IndexOf('_'));
            if (resultType == "dialogue")
            {
                StartDialogue(resultID);
                
                // 대화가 끝날 때까지 대기
                bool isDialogueEnd = false;
                //DialogueManager.Instance.OnDialogueEnd += () => isDialogueEnd = true;
                yield return new WaitUntil(() => isDialogueEnd);
            }
            else if (resultType == "effect")
            {
                StartEffect(resultID);
                
                // 효과가 끝날 때까지 대기
                bool isEffectEnd = false;
                //EffectManager.Instance.OnEffectEnd += () => isEffectEnd = true;
                yield return new WaitUntil(() => isEffectEnd);
            }
            else if (resultType == "input")
            {
                //StartInput(resultID);
                InputFieldManager.Instance.SetQuestionField(resultID);
                
                // input이 끝날 때까지 기다리기
                bool isInputEnd = false;
                //InputFieldManager.Instance.OnInputEnd += () => isInputEnd = true;
                yield return new WaitUntil(() => isInputEnd);
                
            }
            else if (resultType == "Event")
            {
                CloseEventSuccess(_event);
                StartCoroutine(ExecuteEvent(resultID));
            }
        }
    }

    /*
    public void StopCoroutine()
    {
        StopAllCoroutines();
        string lockID = DataManager.Instance._events[currentEventID].lock_condition_id;
        if (!String.IsNullOrEmpty(lockID))
        {
            DataManager.Instance._lockConditions[lockID].UnLock();
        }
    }
    */
    
    public void CloseEventFailure(EventStructure _event)
    {
        Debug.Log("성공적이지 못하게 이벤트 종료");
        DataManager.Instance._lockConditions[_event.lock_condition_id].UnLock();
    }
    
    public void CloseEventSuccess(EventStructure _event)
    {
        Debug.Log("성공적으로 이벤트 종료");
        DataManager.Instance._lockConditions[_event.lock_condition_id].UnLock();
        _event.isExecuted = true;
        nextEventID = _event.next_Event_id;
    }
}