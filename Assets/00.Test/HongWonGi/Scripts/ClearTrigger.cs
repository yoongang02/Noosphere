using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearTrigger : MonoBehaviour
{
   [SerializeField] private string nextScene;
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         Debug.Log("트리거 엔터");
         SceneManager.LoadScene(nextScene);
         
         //일단은 임시로 이렇게 코드 설정
         if (EventManagerYKM.Instance.curStageInfo == EventManagerYKM.ChapterInfo.Prologue)
         {
            EventManagerYKM.Instance.curStageInfo = EventManagerYKM.ChapterInfo.Stage1;
         }
      }
   }
}
