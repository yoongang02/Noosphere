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
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         Debug.Log("트리거 엔터");
         EventManagerYKM.Instance.ExecuteEvent(eventID).Forget();
         SceneManager.LoadScene(nextScene);
      }
   }
}
