using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
public class LoadingSceneUI : MonoBehaviour
{
    [SerializeField] private Slider _loadingSlider;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private RawImage _fadeImage;
    [SerializeField] private float _fadeDuration = 0.8f;
    
    private void Start()
    {
        StartLoading().Forget();
    }
    private async UniTaskVoid StartLoading()
    {
        await LoadData();
        await FadeOut();
        SceneManager.LoadScene("map1");
    }

    private async UniTask LoadData()
    {
        await DataManager.Instance.InitializeData(UpdateLoadingProgress);
    }

    private void UpdateLoadingProgress(float progress)
    {
        _loadingSlider.value = progress;
        _loadingText.text = $"로딩중 : {Mathf.RoundToInt(progress * 100)}%";
    }

   
    private async UniTask FadeOut()
    {
        _fadeImage.color = new Color(0, 0, 0, 0);
        await _fadeImage.DOFade(1, _fadeDuration).AsyncWaitForCompletion();
    }
}
