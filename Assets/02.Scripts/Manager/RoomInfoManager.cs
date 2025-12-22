using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = NooSphere.Debug;
public class RoomInfoManager : MonoBehaviour
{
    public string roomName;
    public EventManagerYKM.RoomInfo roomInfo;

    private void Start()
    {
        Debug.Log("이벤트 매니저 찾기!!" + FindObjectOfType<EventManagerYKM>().gameObject);
        Debug.Log(EventManagerYKM.Instance.curRoomInfo + "CurRoomInfo");
        Debug.Log(this.roomInfo + "roomInfo");
        EventManagerYKM.Instance.curRoomInfo = this.roomInfo;

        if (SceneManager.GetActiveScene().name == "Lounge")
        {
            if (DataManager.Instance._events.ContainsKey("Event_A031") && DataManager.Instance._events.ContainsKey("Event_B067"))
            {
                if(DataManager.Instance._events["Event_A031"].isExecuted)
                {
                    if (DataManager.Instance._events["Event_B067"].isExecuted)
                    {
                        EventManagerYKM.Instance.curChapterInfo = EventManagerYKM.ChapterInfo.Stage2;
                        EventManagerYKM.Instance.curRoomInfo = EventManagerYKM.RoomInfo.Room_104;
                    }
                    EventManagerYKM.Instance.curChapterInfo = EventManagerYKM.ChapterInfo.Stage1;
                    EventManagerYKM.Instance.curRoomInfo = EventManagerYKM.RoomInfo.Room_102;
                }
            }
        }
        else
        {
            StartCoroutine(ShowRoomNumber());
        }

        if(EventManagerYKM.Instance.currentEventID == "Event_D097")
        {
            DataManager.Instance._lockConditions["Lock_condition_001"].Lock();
        }
        Debug.LogWarning($"현재 방 정보 : {roomInfo}");
        InventoryManager.Instance.currentViewChapter = (int)EventManagerYKM.Instance.curRoomInfo;
    }

    IEnumerator ShowRoomNumber()
    {
        yield return new WaitForSeconds(2f);
        if (transform.GetChild(0) != null)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
