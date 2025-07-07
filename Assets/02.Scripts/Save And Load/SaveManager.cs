using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager;
using UnityEngine;

namespace NooSphere
{
    public class SaveManager : Singleton<SaveManager>
    {
        public event Action OnSaveStart;
        public event Action<int> OnSaveFinish;

        // 세이브 슬롯1에 현재 진행상황을 자동 저장 시 호출되는 함수
        // TODO : 상태 값이 모두 저장되는 동안 플레이어의 상호작용을 막은 채, 저장 중 UI를 띄우는 작업 연결 해야 함.
        public IEnumerator DoAutoSave()
        {
            // Root 하위에 Saves 폴더로 이어지는 경로 찾기
            string folderPath = Path.Combine(Application.persistentDataPath, "Saves");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 저장 시작 준비 -> 저장 중 UI 활성화 & 상호작용 막기
            OnSaveStart?.Invoke();
            yield return null;

            SaveCurrentState(folderPath, 1);

            yield return null;
            // 저장 끝 -> 저장 중 UI 비활성화 & 상호작용 풀기
            OnSaveFinish?.Invoke(1);
        }

        // 자동 저장 혹은 사용자 임의 저장 시, 현재 상태를 저장하는 함수
        // 폴더 경로와, 저장하려는 파일 이름을 파라미터로 받음

        // TODO : 플레이타임 값 불러와서 저장하는 작업 진행해야 함.
        void SaveCurrentState(string folderPath, int slotIndex)
        {
            // 저장할 파일 경로 찾기
            string filePath = Path.Combine(folderPath, $"slot{slotIndex}.es3");

            // 현재 상태 저장하기, 저장할 데이터는 다음과 같음.
            // DataManager의 _events, _evidences, _quiz 리스트.
            // 저장 당시 플레이어의 Transform 정보
            // 저장 당시 플레이어가 위치한 공간 정보
            // 저장 일시(YYYY - MM - DD  HH: MM:SS 형식)
            // 저장 당시까지의 플레이 타임
            try
            {
                ES3.Save("EventDatas", DataManager.Instance._events, filePath);
                ES3.Save("EvidenceDatas", DataManager.Instance._evidences, filePath);
                ES3.Save("QuizDatas", DataManager.Instance._quiz, filePath);
                ES3.Save("PlayerTransform", PlayerController.Instance.transform, filePath);

                if (FindObjectOfType<RoomInfoManager>() is RoomInfoManager roomInfoManager)
                {
                    ES3.Save("Location",roomInfoManager.roomName, filePath);
                }
                else
                {
                    Debug.LogError("플레이어의 현재 Location 값을 찾을 수 없습니다.");
                }

                string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                ES3.Save("DateTime", currentTime, filePath);

                // 이 곳에 플레이타임 불러와서 저장해야 함.
            }
            catch (System.IO.IOException)
            {
                Debug.LogError("파일이 열려있거나, 충분한 저장공간이 없습니다.");
            }
            catch (System.Security.SecurityException)
            {
                Debug.LogError("권한이 없는 사용자입니다.");
            }
        }
    }
}