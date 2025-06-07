using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetPlayerLoungePos : MonoBehaviour
{
    [SerializeField] private List<Transform> _loungeInitTrans;
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
        GameObject player = GameObject.FindWithTag("Player");
        if (SceneTracker.previousSceneName == "PrologueMap_real")
        {
            player.transform.position = _loungeInitTrans[0].position;
            player.transform.rotation = _loungeInitTrans[0].rotation;
        }
        else if (SceneTracker.previousSceneName == "Stage1Map_real")
        {
            player.transform.position = _loungeInitTrans[1].position;
            player.transform.rotation = _loungeInitTrans[1].rotation;
        }
    }
}
