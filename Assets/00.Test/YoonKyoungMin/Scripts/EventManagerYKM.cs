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
    public string nextEventID = "";
    
    void Awake()
    {
        //게임 시작 시, 스테이지 정보 초기화
        curStageInfo = ChapterInfo.Prologue;
    }

    //이벤트 실행
    public void ExecuteEvent(string eventID)
    {
        if (!DataManager.Instance._events.ContainsKey(eventID))
        {
            Debug.Log(eventID + " 이벤트가 존재하지 않음.");
            return;
        }

        if (nextEventID != "" && nextEventID != eventID)
        {
            Debug.Log("현재 실행되어야 하는 이벤트는 " + nextEventID + "입니다.");
            return;
        }

        EventStructure eventStructure = DataManager.Instance._events[eventID];

        //실행 조건 만족하는지 체크
        if (eventStructure.CheckCondition())
        {
            StartCoroutine(HandleEventWithEvidence(eventStructure));
        }
    }
    
    IEnumerator HandleEventWithEvidence(EventStructure eventStructure){
        
        //락 조건이 있다면, 락 걸기
        if (DataManager.Instance._lockConditions.ContainsKey(eventStructure.lock_condition_id))
        {
            DataManager.Instance._lockConditions[eventStructure.lock_condition_id].Lock();
        }
        //결과 실행하기
        foreach (var resultID in eventStructure.resultIDs)
        {
            if (resultID != "")
            {
                string resultType = resultID.Substring(0, resultID.IndexOf('_'));
                if (resultType == "dialogue")
                {
                    StartDialogue(resultID);
                }
                else if (resultType == "effect")
                {
                    StartEffect(resultID);
                }
            }
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
        StartCoroutine(HandleEventWithDialogue(dialogueID));
    }

    //대화 끝날 때까지 기다리기
    IEnumerator HandleEventWithDialogue(string dialogueID)
    {
        DialogueManager.Instance.SetDialogue(dialogueID);
        yield return new WaitUntil(() => !DialogueManager.Instance.isDialogeEnd);
    }

    //effect 시작
    void StartEffect(string effectID)
    {
        Debug.Log(effectID + " 효과 시작");
    }
}