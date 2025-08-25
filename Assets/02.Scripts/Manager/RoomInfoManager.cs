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
        //if (NooSphere.SaveManager.Instance.CurrentLoadType == NooSphere.GameLoadType.ContinueGame)
        //{
        //    WhenContinueGame();
        //}
        Debug.Log("이벤트 매니저 찾기!!" + FindObjectOfType<EventManagerYKM>().gameObject);
        Debug.Log(EventManagerYKM.Instance.curRoomInfo + "CurRoomInfo");
        Debug.Log(this.roomInfo + "roomInfo");
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

        if(EventManagerYKM.Instance.currentEventID == "Event_D097")
        {
            DataManager.Instance._lockConditions["Lock_condition_001"].Lock();
        }
        Debug.LogWarning($"현재 방 정보 : {roomInfo}");
        InventoryManager.Instance.currentViewChapter = (int)EventManagerYKM.Instance.curRoomInfo;
    }


    // 이어하기를 통해 이동된 씬일 경우의 작업
    void WhenContinueGame()
    {
        NooSphere.SaveManager.Instance.WhenContinueSceneLoaded();
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
