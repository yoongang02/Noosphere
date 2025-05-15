using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (SceneTracker.previousMentalState == false && PlayerInteract.Instance.isInMental == false)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;
        }
    }
}