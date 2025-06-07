using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = NooSphere.Debug;
public class RoomInfoManager : MonoBehaviour
{
    public EventManagerYKM.RoomInfo roomInfo;

    private void OnEnable()
    {
        EventManagerYKM.Instance.curRoomInfo = this.roomInfo;

        if (SceneManager.GetActiveScene().name == "Lounge")
        {
            if (EventManagerYKM.Instance.curChapterInfo == EventManagerYKM.ChapterInfo.Stage1)
            {
                EventManagerYKM.Instance.curRoomInfo = EventManagerYKM.RoomInfo.Room_102;
            }
            else if (EventManagerYKM.Instance.curChapterInfo == EventManagerYKM.ChapterInfo.Stage2)
            {
                // 스테이지1 클리어 안하고 나왔으면
                if (!DataManager.Instance._events["Event_B067"].isExecuted)
                {
                    EventManagerYKM.Instance.curChapterInfo = EventManagerYKM.ChapterInfo.Stage1;
                    EventManagerYKM.Instance.curRoomInfo = EventManagerYKM.RoomInfo.Room_102;
                }
                else
                {
                    EventManagerYKM.Instance.curRoomInfo = EventManagerYKM.RoomInfo.Room_104;
                }
            }
        }
        else
        {
            StartCoroutine(ShowRoomNumber());
        }
        Debug.LogWarning($"현재 방 정보 : {roomInfo}");
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
