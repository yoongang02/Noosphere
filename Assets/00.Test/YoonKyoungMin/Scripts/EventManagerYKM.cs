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
            Debug.Log("#" + eventID + " 이벤트가 존재하지 않음.");
            yield return null;
        }
        
        //nextEventID가 비어있지 않은데, 실행하고자 하는 이벤트ID와 같지 않다면
        if (!string.IsNullOrEmpty(nextEventID) && nextEventID != eventID)
        {
            Debug.Log("#현재 실행되어야 하는 이벤트는 " + nextEventID + "입니다.");
            yield return null;
        }

        
        EventStructure eventStructure = DataManager.Instance._events[eventID];
        currentEventID = eventStructure.eventId;
        Debug.Log("#0 : " + eventID + "이벤트 실행");
        
        //1. LockCondition 실행
        string lockConditionID = eventStructure.lockConditionId;
        LockConditionStructure lockCondition = DataManager.Instance._lockConditions[lockConditionID];
        if (lockCondition != null)
        {
            Debug.Log("#1 : " + lockConditionID + "락 조건 실행");
            lockCondition.Lock();
        }
        
        //2. 반복 가능한 이벤트인지 체크
        //반복 불가능인데 이미 실행된 이벤트라면 실행 불가능
        if (!eventStructure.repeatType && eventStructure.isExecuted)
        {
            Debug.Log("#" + eventID + "는 이미 실행된 이벤트이며, 반복 불가능한 이벤트입니다.");
            //반복 불가능할 경우의 결과 출력
            if (!string.IsNullOrEmpty(eventStructure.repeatFalseResult))
            {
                //repeatFalseResult 실행
                Debug.Log("#" + eventStructure.repeatFalseResult + "RepeatFalseResult 실행");
                CloseEventFailure(eventStructure);
                StartCoroutine(ExecuteEvent(eventStructure.repeatFalseResult));
            }
            yield return null;
        }
        
        //3. ConditionType 체크
        if (eventStructure.conditionType == "and")
        {
            //3-1. Condition 만족하는지 체크
            //각 조건을 만족하는지 체크
            Debug.Log("#3 : " + eventStructure.eventId + "의 condition 만족하는지 검사 시작");
            int conditionNum = 1;
            foreach (var condition in eventStructure.conditions)
            {
                if (!eventStructure.IsConditionMet(condition))
                {
                    Debug.Log("#3-2 : "+eventStructure.eventId+"의 condition"+conditionNum+" 불만족");
                    //출력 후 이벤트 중간 종료
                    CloseEventFailure(eventStructure);
                    //해당 condition에 대한 falseResult 실행 및 실행 될 때까지 기다리기
                    int falseResultNum = 1;
                    foreach (var falseResult in eventStructure.conditionFalseResults)
                    {
                        Debug.Log("#3-3 : "+eventStructure.eventId+"의 falseCondition"+falseResultNum+" 실행");
                        StartCoroutine(ExecuteEvent(falseResult));
                    }
                    yield return null;
                }
                else
                {
                    Debug.Log("#3-1 : "+eventStructure.eventId+"의 condition"+conditionNum+" 만족");
                }

                conditionNum++;
            }
        }
        else if(eventStructure.conditionType != "or")
        {
            CloseEventFailure(eventStructure);
            Debug.Log("#"+ eventID + "의 conditionType이 올바르지 않습니다.");
            yield return null;
        }
        
        //4. evidenceID가 비어있지 않으면 증거물 습득
        if (!string.IsNullOrEmpty(eventStructure.evidenceId) && DataManager.Instance._evidences.ContainsKey(eventStructure.evidenceId))
        {
            Debug.Log("#4 : "+eventStructure.eventId+"의 증거물"+eventStructure.evidenceId+" 습득");
            
            // 증거물 조사 UI 띄우기
            EvidenceStructure evidence = DataManager.Instance._evidences[eventStructure.evidenceId];
            UIManager.Instance.OpenUI(UIManager.Instance.investigateUI);

            // UI에서 입력을 기다림
            bool isSelectEnd = false;
            UIManager.Instance.OnSelectEnd += () => isSelectEnd = true;
            yield return new WaitUntil(() => isSelectEnd);
            Debug.Log("#4-1 : "+eventStructure.eventId+"의 증거물"+eventStructure.evidenceId+" 습득 선택 완료");
        }
        
        //일단 결과까지 왔다면 이벤트가 성공적으로 실행된 것.
        //결과의 실행 여부는 각 결과ID에 따라 처리
        CloseEventSuccess(eventStructure);
        
        //5. 결과들 실행하기
        int resultNum = 1;
        foreach (var resultID in eventStructure.results)
        {
            if (!string.IsNullOrEmpty(resultID))
            {
                Debug.Log("#5 : "+eventStructure.eventId+"의 결과" + resultNum +" " + resultID +" 실행");
                string resultType = resultID.Substring(0, resultID.IndexOf('_'));
                if (resultType == "Dialogue")
                {
                    StartDialogue(resultID);
                
                    // 대화가 끝날 때까지 대기
                    bool isDialogueEnd = false;
                    DialogueManager.Instance.OnDialogueEnd += () => isDialogueEnd = true;
                    yield return new WaitUntil(() => isDialogueEnd);
                    Debug.Log("#5-2 : " + resultID + " 대화 끝");
                }
                else if (resultType == "Effect")
                {
                    StartEffect(resultID);
                
                    // 효과가 끝날 때까지 대기
                    bool isEffectEnd = false;
                    EffectManager.Instance.OnEffectEnd += () => isEffectEnd = true;
                    yield return new WaitUntil(() => isEffectEnd);
                    Debug.Log("#5-2 : " + resultID + " 효과 끝");
                }
                else if (resultType == "Input")
                {
                    StartInput(resultID);
                
                    // input이 끝날 때까지 기다리기
                    bool isInputEnd = false;
                    InputFieldManager.Instance.OnInputEnd += () => isInputEnd = true;
                    yield return new WaitUntil(() => isInputEnd);
                    Debug.Log("#5-2 : " + resultID + " input 끝");
                }
                else if (resultType == "Event")
                {
                    StartCoroutine(ExecuteEvent(resultID));
                }

                resultNum++;
            }
        }
    }

    //dialogue 시작
    void StartDialogue(string dialogueID)
    {
        Debug.Log("#5-1 : " + dialogueID + " 대화 시작");
        DialogueManager.Instance.SetDialogue(dialogueID);
    }

    //effect 시작
    void StartEffect(string effectID)
    {
        Debug.Log("#5-1 : " + effectID + " 효과 시작");
        EffectManager.Instance.SetEffect(effectID);
    }
    
    //Input 시작
    void StartInput(string inputID)
    {
        Debug.Log("#5-1 : " + inputID + " input 시작");
        InputFieldManager.Instance.SetQuestionField(inputID);
    }
    
    public void CloseEventFailure(EventStructure _event)
    {
        Debug.Log("#6 : " + _event.eventId + " 성공적이지 못하게 이벤트 종료");
        DataManager.Instance._lockConditions[_event.lockConditionId].UnLock();
        currentEventID = "";
    }
    
    public void CloseEventSuccess(EventStructure _event)
    {
        Debug.Log("#6 : " + _event.eventId + "성공적으로 이벤트 종료");
        DataManager.Instance._lockConditions[_event.lockConditionId].UnLock();
        _event.isExecuted = true;
        nextEventID = _event.nextEventId;
    }
}