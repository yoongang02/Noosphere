using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = NooSphere.Debug;
public class RoomInfoManager : MonoBehaviour
{
    public EventManagerYKM.RoomInfo roomInfo;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
                    EventManagerYKM.Instance.curRoomInfo = EventManagerYKM.RoomInfo.Room_104;
                }
            }
            Debug.LogWarning($"현재 방 정보 : {roomInfo}");
        }
    }
}
