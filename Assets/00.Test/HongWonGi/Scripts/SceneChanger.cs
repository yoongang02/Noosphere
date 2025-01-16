using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class SceneChanger : Singleton<SceneChanger>
{
    public CanvasGroup Fade_img;
    float fadeDuration = 1.5f;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Fade_img.DOFade(0, fadeDuration)
            .OnComplete(() =>
            {
                Fade_img.blocksRaycasts = false;
                if (PlayerController.Instance!=null)
                {
                    PlayerController.Instance.canMove =true;
                }
            });
    }

    public async UniTaskVoid ChangeScene(string sceneName)
    {
        await Fade_img.DOFade(1, fadeDuration)
            .OnStart(() => {
                Fade_img.blocksRaycasts = true;
                if (PlayerController.Instance!=null)
                {
                    PlayerController.Instance.canMove = false;
                }
            })
            .AsyncWaitForCompletion();

        await LoadScene(sceneName);
    }
    
    private async UniTask LoadScene(string sceneName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
       
        while (!async.isDone)
        {
            await UniTask.Yield();
        }
    }
}