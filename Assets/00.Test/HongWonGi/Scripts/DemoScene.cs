using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoScene : MonoBehaviour
{
    [SerializeField] private Button _restartBtn;

    private void Start()
    {
        _restartBtn.onClick.AddListener(RestartScene);
    }

    private void RestartScene()
    {
        {
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.scene.name == "DontDestroyOnLoad")
                {
                    Destroy(obj);
                }
            }

            // 첫 번째 씬 로드
            // SceneManager.LoadScene("StartScene");
            SceneChanger.Instance.ChangeScene("StartScene");
        }
    }
}
