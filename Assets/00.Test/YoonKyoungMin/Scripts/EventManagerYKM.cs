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
        if (!DataManager.Instance._events.ContainsKey(eventID))
        {
            Debug.Log(eventID + " 이벤트가 존재하지 않음.");
            yield return null;
        }

        if (String.IsNullOrEmpty(nextEventID) && nextEventID != eventID)
        {
            Debug.Log("현재 실행되어야 하는 이벤트는 " + nextEventID + "입니다.");
            yield return null;
        }

        
        EventStructure eventStructure = DataManager.Instance._events[eventID];
        string lockConditionId = eventStructure.lock_condition_id;
        Debug.Log(eventID + "이벤트 실행 시도");
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
                    StartCoroutine(ExecuteResult(branchResult));
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
    IEnumerator ExecuteResult(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            string resultType = id.Substring(0, id.IndexOf('_'));
            if (resultType == "dialogue")
            {
                StartDialogue(id);
                yield return new WaitUntil(() => DialogueManager.Instance.isDialogeEnd);
            }
            else if (resultType == "effect")
            {
                StartEffect(id);
                yield return new WaitUntil(() => EffectManager.Instance.isEffectEnd);
            }
            else if (resultType == "input")
            {
                InputFieldManager.Instance.SetQuestionField(id);
                yield return new WaitUntil(() => InputFieldManager.Instance.isSubmitAnswer);
                PlayerInteract.Instance.canInteract = true;
            }
            else if (resultType == "Event")
            {
                yield return ExecuteEvent(id);
            }
            else if (resultType == "evidence")
            {
                EvidenceStructure evidenceStructure = DataManager.Instance._evidences[resultType];
                if(evidenceStructure != null) InventoryManager.Instance.AddEvidence(evidenceStructure);
            }
        }
    }

    public void StopCoroutine()
    {
        StopAllCoroutines();
        string lockID = DataManager.Instance._events[currentEventID].lock_condition_id;
        if (!String.IsNullOrEmpty(lockID))
        {
            DataManager.Instance._lockConditions[lockID].UnLock();
        }
    }
}