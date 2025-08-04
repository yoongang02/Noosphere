using System.Collections;
using System.Collections.Generic;
using NooSphere;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class SetPlayerPos : MonoBehaviour
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
        if(NooSphere.SaveManager.Instance.CurrentLoadType == GameLoadType.ContinueGame) return;
        
        if (SceneTracker.previousMentalState == false && PlayerInteract.Instance.isInMental == false)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;

            if (NooSphere.SaveManager.Instance.OnDoorAutoSave)
            {
                NooSphere.Debug.LogWarning("문 상호작용 자동 저장");
                NooSphere.SaveManager.Instance.OnDoorAutoSave = false;
                NooSphere.SaveManager.Instance.DoAutoSaveDelay();
            }
        }
    }
}