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
    
    [Space(5)][Header("정신세계 진입")] 
    public GameObject mentalTrigger;
    
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
                    }
                }
            }
        }
    }

 


    private void OnTriggerEnter(Collider other)
    {
        //진입 시 바로 이벤트 실행되는 트리거에 진행하면
        if (other.CompareTag("EventTrigger"))
        {
            foreach (string eventID in other.GetComponent<EventTrigger>().eventIdList)
            {
                if (CheckInteractionAvail(eventID))
                {
                    StartCoroutine(EventManagerYKM.Instance.ExecuteEvent(eventID));
                    return;
                }
            }
        }
        else if (other.CompareTag("EventInteractionTrigger"))
        {
            //상호 작용 트리거라면
            interactionTrigger = other.gameObject;
            foreach (string eventID in other.GetComponent<EventTrigger>().eventIdList)
            {
                if (CheckInteractionAvail(eventID))
                {
                    ShowInteractionMark();
                    return;
                }
            }
        }
        else if (other.CompareTag("EventMentalEnterTrigger"))
        {
            //정신세계 진입 트리거에 들어가면
            mentalTrigger = other.gameObject;
            foreach (string eventID in other.GetComponent<EventTrigger>().eventIdList)
            {
                if (CheckInteractionAvail(eventID))
                {
                    ShowInteractionMark();
                    return;
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

    void HideInteractionMark()
    {
        _interactionMark.SetActive(false);
    }

    public bool CheckInteractionAvail(string id)
    {
        if (DataManager.Instance._events.ContainsKey(id))
        {
            if (String.IsNullOrEmpty(EventManagerYKM.Instance.nextEventID) || EventManagerYKM.Instance.nextEventID == id)
            {
                return true;
            }
        }
        return false;
    }

    public Dictionary<string,string> GetPlayeCanUseEvidenceID()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
       
        //플레이어가 현재 상호작용 중인 트리거가 있어야 함.
        if (interactionTrigger != null)
        {
            //해당 트리거에서 이벤트 아이디 가져오기
            foreach (string eventID in interactionTrigger.GetComponent<EventTrigger>().eventIdList)
            {
                data.Add("eventID",eventID);
                EventStructure eventStructure = DataManager.Instance._events[eventID];
            
                /*
                //해당 이벤트가 분기점이 있는지 체크
                if (eventStructure.branch_Type)
                {
                    data[eventID] = eventStructure.branch_Element;
                }
                */
            }
            return data;
        }
        return data;
    }
}
