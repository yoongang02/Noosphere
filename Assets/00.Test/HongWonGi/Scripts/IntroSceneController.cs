using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroSceneController : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeDuration = 0.8f;
    [SerializeField] private PlayableDirector _introTimeLine;
    [SerializeField] private VideoPlayer _introVideo;
    
    private void Start()
    {
        FadeIn().Forget();
    }
    private async UniTask FadeIn()
    {
        _fadeImage.color = new Color(0, 0, 0, 1);
        await _fadeImage.DOFade(0, _fadeDuration).AsyncWaitForCompletion();
        _introTimeLine.Play();
    }

    public void StartFadeOut(float fadeDuration)
    {
        FadeOut(fadeDuration).Forget();
    }

    private async UniTask FadeOut(float time)
    {
        _fadeImage.color = new Color(0, 0, 0, 0);
        await _fadeImage.DOFade(1, time).AsyncWaitForCompletion();
    }

    #region TimeLineFunc
    public void PlayIntroVideo()
    {
        _introVideo.Play();
    }
    #endregion
}
