using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = NooSphere.Debug;

public class EventTrigger : MonoBehaviour
{
    [Header("이벤트 목록")]
    public List<string> eventIdList = new List<string>();
    private EventStructure _curEvent;
    [Space(5)] [Header("트리거 설정")] 
    [SerializeField] private bool _canDestroy = false;
    [Space(5)][Header("NPC 관련")]
    public GameObject npcCameraPoint;
    public bool isNpc;
    
    
    //플레이어가 트리거 내에 진입한다면
    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && PlayerInteract.Instance.canInteract)
        {
            bool canExecute = false;
            foreach (var eventID in eventIdList)
            {
                if (EventManagerYKM.Instance.CheckExecutable(eventID))
                {
                    _curEvent = DataManager.Instance._events[eventID];
                    canExecute = true;
                    break;
                }
            }

            if (canExecute)
            {
                string[] results = EventManagerYKM.Instance.CheckConditions(_curEvent);
                
                //결과 실행
                if (results != null && results.Length > 0)
                {
                    //트리거 종류에 따라 할당하기
                    switch (tag)
                    {
                        case "EventTrigger":
                            //바로 실행 메소드 호출
                            break;
                        case "EventInteractionTrigger":
                            //플레이어에게 ? 띄우기
                            PlayerInteract.Instance.ShowInteractionMark();
                            //플레이어의 OnInteract 액션에 실행 메소드 할당
                            PlayerInteract.Instance.OnInteract += () =>
                            {
                                EventManagerYKM.Instance.DoResult(results).Forget();
                            };
                            break;
                        case "EventMentalEnterTrigger":
                            //플레이어에게 ? 띄우기
                            PlayerInteract.Instance.ShowInteractionMark();
                            //정신세계 진입 프로세스의 OnInteract 액션에 실행 메소드 할당
                            break;
                    }
                }   
            }
        }
    }
    
    //플레이어가 트리거 내에서 나간다면
    protected void OnTriggerExit(Collider other)
    {
        //플레이어에게 ? 없애기
        PlayerInteract.Instance.HideInteractionMark();
        
        //현재 이벤트 초기화하기
        _curEvent = null;
    }
}
