using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearTrigger : MonoBehaviour
{
   [SerializeField] private string nextScene;
   [SerializeField] private string eventID;
   [SerializeField] private EventManagerYKM.ChapterInfo _chapterInfo;
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         Debug.Log("트리거 엔터");
         EventManagerYKM.Instance.ExecuteEvent(eventID).Forget();
         EventManagerYKM.Instance.curChapterInfo = _chapterInfo;
         SceneManager.LoadScene(nextScene);
      }
   }
}
