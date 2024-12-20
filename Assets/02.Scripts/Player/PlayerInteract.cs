using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteract : Singleton<PlayerInteract>
{
    [Header("상호작용")] 
    public bool canInteract = true; //상호작용을 할 수 있는지(lock 조건에 이용)
    public GameObject interactionTrigger;
    [SerializeField] private GameObject _interactionMark;
    [SerializeField] private GameObject _evidenceObjectInScene;

    [Space(5)] [Header("정신세계 진입")] public bool isInMental = false;
    public GameObject mentalTrigger;

    [Space(5)] [Header("증거물 사용")] public bool isUsingEvidence = false;
    void Update()
    {
        if (!UIManager.Instance.IsAnyUIOpen())
        {
            //E키를 이용한 상호작용
            if (canInteract && interactionTrigger != null && Input.GetKeyDown(KeyCode.E))
            { 
                //이벤트 실행
                EventTrigger trigger = interactionTrigger.GetComponent<EventTrigger>();
                foreach (string eventID in trigger.eventIdList)
                {
                    if (trigger.destroyEvidence)
                    {
                        _evidenceObjectInScene = trigger.transform.parent.gameObject;
                    }
                    else
                    {
                        _evidenceObjectInScene = null;
                    }
                    
                    if (CheckInteractionAvail(eventID))
                    {
                        StartCoroutine(EventManagerYKM.Instance.ExecuteEvent(eventID));
                        HideInteractionMark();
                        break;
                    }
                }
            }
        }
    }

 


    private void OnTriggerStay(Collider other)
    {
        //진입 시 바로 이벤트 실행되는 트리거에 진행하면
        if (other.CompareTag("EventTrigger"))
        {
            foreach (string eventID in other.GetComponent<EventTrigger>().eventIdList)
            {
                if (CheckInteractionAvail(eventID) && canInteract)
                {
                    StartCoroutine(EventManagerYKM.Instance.ExecuteEvent(eventID));
                    break;
                }
            }
        }
        else if (other.CompareTag("EventInteractionTrigger"))
        {
            //상호 작용 트리거라면
            interactionTrigger = other.gameObject;
            foreach (string eventID in other.GetComponent<EventTrigger>().eventIdList)
            {
                if (CheckInteractionAvail(eventID) && canInteract)
                {
                    ShowInteractionMark();
                    break;
                }
            }
        }
        else if (other.CompareTag("EventMentalEnterTrigger"))
        {
            //정신세계 진입 트리거에 들어가면
            mentalTrigger = other.gameObject;
            foreach (string eventID in other.GetComponent<EventTrigger>().eventIdList)
            {
                if (CheckInteractionAvail(eventID) && canInteract)
                {
                    ShowInteractionMark();
                    break;
                }
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        //상호작용 트리거에서 나가면
        if (other.CompareTag("EventInteractionTrigger"))
        {
            interactionTrigger = null;
            _evidenceObjectInScene = null;
            HideInteractionMark();
        }
        
        //정신세계 진입 트리거에 나가면
        if (other.CompareTag("EventMentalEnterTrigger"))
        {
            mentalTrigger = null;
            HideInteractionMark();
        }
    }

    

    void ShowInteractionMark()
    {
        _interactionMark.SetActive(true);
    }

    public void HideInteractionMark()
    {
        _interactionMark.SetActive(false);
    }

    public bool CheckInteractionAvail(string id)
    {
        if (DataManager.Instance._events.ContainsKey(id))
        {
            EventStructure eventStructure = DataManager.Instance._events[id];
            if (String.IsNullOrEmpty(EventManagerYKM.Instance.nextEventID) || EventManagerYKM.Instance.nextEventID == id)
            {
                //반복 가능한 이벤트인지 체크
                //반복 불가능인데 이미 실행된 이벤트라면 실행 불가능
                if (!eventStructure.repeatType && eventStructure.isExecuted)
                {
                    Debug.Log("#" + id + "는 이미 실행된 이벤트이며, 반복 불가능한 이벤트입니다.");
                    //반복 불가능할 경우의 결과 출력
                    if (!string.IsNullOrEmpty(eventStructure.repeatFalseResult))
                    {
                        //repeatFalseResult 실행
                        Debug.Log("#" + eventStructure.repeatFalseResult + "RepeatFalseResult 실행");
                        StartCoroutine(EventManagerYKM.Instance.ExecuteEvent(eventStructure.repeatFalseResult));
                    }

                    return false;
                }

                //예외 코드
                if ((eventStructure.results[0] == "Quiz_004" || eventStructure.results[0] == "Quiz_005" 
                                                             || eventStructure.results[0] == "Quiz_006"
                                                             || id == "Event_B050"
                                                             || id == "Event_B051")
                    && eventStructure.conditionType == "and")
                {
                    foreach (var conditionID in eventStructure.conditions)
                    {
                        if (!eventStructure.IsConditionMet(conditionID)) return false;
                    }
                }
                return true;
            }
        }
        return false;
    }
    
    public void InitUsingEvidence()
    {
        isUsingEvidence = false;
        InventoryManager.Instance.GetComponent<InventoryNavigator>().InitUsingEvidence();
    }
}
