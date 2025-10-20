using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssignPlayer : MonoBehaviour
{
    private CinemachineVirtualCamera _loungeCamera;

    private void Awake()
    {
        _loungeCamera = GetComponent<CinemachineVirtualCamera>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        DoAssign();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DoAssign();
    }

    private void DoAssign()
    {
        if (NooSphere.SaveManager.Instance.CurrentLoadType == NooSphere.GameLoadType.ContinueGame) return;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _loungeCamera.LookAt = player.transform;
        }
        else
        {
            Debug.LogWarning("플레이어가 없습니다");
        }
    }

    public void AssignLoungeCamera()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _loungeCamera.LookAt = player.transform;
        }
        else
        {
            Debug.LogWarning("플레이어가 없습니다");
        }
    }
}
