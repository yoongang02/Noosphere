using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
public class LoadingSceneUI : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeDuration = 0.8f;
    
    private void Start()
    {
        StartLoading().Forget();
    }
    private async UniTaskVoid StartLoading()
    {
        Debug.LogWarning(NooSphere.SaveManager.Instance.CurrentLoadType);
        // 새 게임 로딩이면
        if (NooSphere.SaveManager.Instance.CurrentLoadType == NooSphere.GameLoadType.NewGame)
        {
            await DataManager.Instance.InitializeData();
            await FadeOut();
            SceneManager.LoadScene("IntroScene");
        }
        else if(NooSphere.SaveManager.Instance.CurrentLoadType == NooSphere.GameLoadType.ContinueGame) // 이어하기 로딩이면
        {
            await DataManager.Instance.LoadSaveData();
            await FadeOut();
            string sceneName = NooSphere.SaveManager.Instance.GetSceneName(NooSphere.SaveManager.Instance.selectSlotIndex);
            SceneManager.LoadScene(sceneName);
        }
    }

    private async UniTask FadeOut()
    {
        _fadeImage.color = new Color(0, 0, 0, 0);
        SoundManager.Instance.StopBGM();
        await _fadeImage.DOFade(1, _fadeDuration).AsyncWaitForCompletion();
    }
}
