using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
public class InitPlayerPos : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      Transform player = GameObject.FindWithTag("Player").transform;
      player.position = gameObject.transform.position;
      player.rotation = gameObject.transform.rotation;

      EventManagerYKM.Instance.ExecuteEvent("Event_D085").Forget();
    }
}
