using NooSphere;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArtResource0080 : MonoBehaviour
{
    private void Start()
    {
        if (EventManagerYKM.Instance.currentEventID != "Event_A001")
        {
            Destroy(this.gameObject);
        }
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}
