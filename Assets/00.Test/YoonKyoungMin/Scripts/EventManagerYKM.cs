using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            yield break;
        }
        
        //nextEventID가 비어있지 않은데, 실행하고자 하는 이벤트ID와 같지 않다면
        if (!string.IsNullOrEmpty(nextEventID) && nextEventID != eventID)
        {
            Debug.Log("#현재 실행되어야 하는 이벤트는 " + nextEventID + "입니다.");
            yield break;
        }
        
        EventStructure eventStructure = DataManager.Instance._events[eventID];
        currentEventID = eventStructure.eventId;
        Debug.Log("#0 : " + eventID + "이벤트 실행");
        
        //1. LockCondition 실행
        string lockConditionID = eventStructure.lockConditionId;
        if (!string.IsNullOrEmpty(lockConditionID) && DataManager.Instance._lockConditions.ContainsKey(lockConditionID))
        {
            Debug.Log("#1 : " + lockConditionID + "락 조건 실행");
            DataManager.Instance._lockConditions[lockConditionID].Lock();
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
                        Debug.Log($"FalseResult : {falseResult}");
                        Debug.Log("#3-3 : "+eventStructure.eventId+"의 falseCondition"+falseResultNum+ " "+ falseResult+ " 실행"); 
                        yield return StartCoroutine(DoResult(falseResult));
                        falseResultNum++;
                    }
                    yield break;
                }
                
                Debug.Log("#3-1 : "+eventStructure.eventId+"의 condition"+conditionNum+" 만족");

                conditionNum++;
            }
        }
        else if(eventStructure.conditionType != "or")
        {
            CloseEventFailure(eventStructure);
            Debug.Log("#"+ eventID + "의 conditionType이 올바르지 않습니다.");
            yield break;
        }
        
        //일단 결과까지 왔다면 이벤트가 성공적으로 실행된 것.
        //결과의 실행 여부는 각 결과ID에 따라 처리
        CloseEventSuccess(eventStructure);
        
        //4. 결과들 실행하기
        int resultNum = 1;
        foreach (var resultID in eventStructure.results)
        {
            Debug.Log("#4 : "+eventStructure.eventId+"의 결과" + resultNum +" " + resultID +" 실행");
            yield return StartCoroutine(DoResult(resultID));
            resultNum++;
        }
        //5. evidenceID가 비어있지 않으면 증거물 습득
        if (!string.IsNullOrEmpty(eventStructure.evidenceId) && DataManager.Instance._evidences.ContainsKey(eventStructure.evidenceId))
        {
            Debug.Log("#5 : "+eventStructure.eventId+"의 증거물"+eventStructure.evidenceId+" 습득");
            
            // 증거물 조사 UI 띄우기
            EvidenceStructure evidence = DataManager.Instance._evidences[eventStructure.evidenceId];
            
            if(!evidence.CanAcquireEvidence()) yield break;
            
            //임시로 사진만 예외처리 함. 기획과 논의 필요!!
            if (evidence.evidenceId == "Evidence_008")
            {
                evidence.AcquireEvidence();
            }
            else
            {
                UIManager.Instance.OpenUI(UIManager.Instance.investigateUI, evidence);
            }
            

            // UI에서 입력을 기다림
            bool isSelectEnd = false;
            UIManager.Instance.OnSelectEnd += () =>
            {
                isSelectEnd = true;
            };
            yield return new WaitUntil(() => isSelectEnd);
            Debug.Log("#5-1 : "+eventStructure.eventId+"의 증거물"+eventStructure.evidenceId+" 선택 완료");
            
            if (!UIManager.Instance.IsAcquiredInInvestigateUI())
            {
                Debug.Log($"No 버튼을 눌렀으니 {eventStructure} 실행 false");
                eventStructure.isExecuted = false;
            }
        
            //예외 이벤트 처리 코드
            if (!UIManager.Instance.IsAcquiredInInvestigateUI() && currentEventID == "Event_A007")
            {
                nextEventID = "Event_A007";
            }
            if (UIManager.Instance.IsAcquiredInInvestigateUI() && currentEventID == "Event_A018")
            {
                //일기 습득 성공하면 더이상 캐비넷에 접근할 수 없도록
                DataManager.Instance._events["Event_A025"].repeatType = false;
            }
            
            if (eventStructure.evidenceId == "Evidence_019"
                || eventStructure.evidenceId == "Evidence_023")
            {
                Debug.Log($"#현재 증거물 아이디 : {eventStructure.evidenceId}, 습득 여부 : {UIManager.Instance.IsAcquiredInInvestigateUI()}");
                if (!UIManager.Instance.IsAcquiredInInvestigateUI())
                {
                    //깨진 거울조각 증거물 조사 UI에서 NO를 눌렀을 경우
                    string id = "";
                    switch (eventStructure.evidenceId)
                    {
                        case ("Evidence_019"): //책장
                            id = "Event_B064";
                            break;
                        case ("Evidence_023"): //라디오
                            id = "Event_B063";
                            break;
                    }
                    Debug.Log($"#{id} repeatType 변경 전 : {DataManager.Instance._events[id].repeatType}");
                    DataManager.Instance._events[id].repeatType = true;
                    Debug.Log($"#{id} repeatType 변경 후 : {DataManager.Instance._events[id].repeatType}");
                }
                else
                {
                    //깨진 거울조각 증거물 조사 UI에서 YES를 눌렀을 경우
                    Debug.Log($"YES 버튼을 눌렀으니 {eventStructure.evidenceId} 거울 조각 얻음");
                    MirrorPuzzleManager.Instance.GetMirrorPiece(eventStructure.evidenceId);
                }
            }

            if (eventStructure.evidenceId == "Evidence_020"
                || eventStructure.evidenceId == "Evidence_021"
                || eventStructure.evidenceId == "Evidence_022")
            {
                if (UIManager.Instance.IsAcquiredInInvestigateUI())
                {
                    //깨진 거울조각 증거물 조사 UI에서 YES를 눌렀을 경우
                    Debug.Log($"YES 버튼을 눌렀으니 {eventStructure.evidenceId} 거울 조각 얻음");
                    MirrorPuzzleManager.Instance.GetMirrorPiece(eventStructure.evidenceId);
                    if (eventStructure.evidenceId == "Evidence_020")
                    {
                        
                    }
                }
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
    
    //Quiz 시작
    void StartQuiz(string quizID)
    {
        Debug.Log("#5-1 : " + quizID + " input 시작");
        QuizManager.Instance.SetQuiz(quizID);
    }
    
    public void CloseEventFailure(EventStructure _event)
    {
        Debug.Log("#6 : " + _event.eventId + " 성공적이지 못하게 이벤트 종료");
        if (!string.IsNullOrEmpty(_event.lockConditionId) &&
            DataManager.Instance._lockConditions.ContainsKey(_event.lockConditionId))
        {
            Debug.Log("#7 : " + _event.lockConditionId + "락 조건 해제");
            DataManager.Instance._lockConditions[_event.lockConditionId].UnLock();
        }
        //currentEventID = "";
    }
    
    public void CloseEventSuccess(EventStructure _event)
    {
        Debug.Log("#6 : " + _event.eventId + "성공적으로 이벤트 종료");
        if (!string.IsNullOrEmpty(_event.lockConditionId) &&
            DataManager.Instance._lockConditions.ContainsKey(_event.lockConditionId))
        {
            Debug.Log("#7 : " + _event.lockConditionId + "락 조건 해제");
            DataManager.Instance._lockConditions[_event.lockConditionId].UnLock();   
        }
        _event.isExecuted = true;
        nextEventID = _event.nextEventId;
    }

    public IEnumerator DoResult(string resultID)
    {
        if (!string.IsNullOrEmpty(resultID))
        {
            string resultType = resultID.Substring(0, resultID.IndexOf('_'));
            if (resultType == "Dialogue")
            {
                //라디오 다이얼로그 예외처리
                if (resultID == "Dialogue_0027" || resultID == "Dialogue_0028" || resultID == "Dialogue_0024")
                {
                    yield break;
                }

                StartDialogue(resultID);
                
                // 대화가 끝날 때까지 대기
                bool isDialogueEnd = false;
                DialogueManager.Instance.OnDialogueEnd += () => isDialogueEnd = true;
                yield return new WaitUntil(() => isDialogueEnd);
                Debug.Log("#4-2 : " + resultID + " 대화 끝");
            }
            else if (resultType == "Effect")
            {
                StartEffect(resultID);
                
                // 효과가 끝날 때까지 대기
                bool isEffectEnd = false;
                EffectManager.Instance.OnEffectEnd += () => isEffectEnd = true;
                yield return new WaitUntil(() => isEffectEnd);
                Debug.Log("#4-2 : " + resultID + " 효과 끝");
            }
            else if (resultType == "Quiz")
            {
                StartQuiz(resultID);
                // input이 끝날 때까지 기다리기
                bool isQuizEnd = false;
                QuizManager.Instance.OnQuizEnd += () => isQuizEnd = true;
                yield return new WaitUntil(() => isQuizEnd);
                Debug.Log("#4-2 : " + resultID + " quiz 끝");
                QuizStructure quizStructure = DataManager.Instance._quiz[resultID];

                if (quizStructure.quizId == "Quiz_001") yield break;
                
                if (quizStructure.isSolved)
                {
                    foreach (var id in quizStructure.quizCorrects)
                    {
                        yield return StartCoroutine(DoResult(id));
                    }
                }
                else
                {
                    yield return StartCoroutine(DoResult(quizStructure.quizWrong));
                }
            }
            else if (resultType == "Event")
            {
                StartCoroutine(ExecuteEvent(resultID));
            }
            else if (resultType == "Mental")
            {
                PlayerInteract.Instance.GetComponent<MentalEnterProcess>().StartEnter(resultID);
            }
        }
    }
}