using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using Cysharp.Threading.Tasks;
public class LoadingSceneUI : MonoBehaviour
{
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private RawImage fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    
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
        loadingSlider.value = progress;
        loadingText.text = $"로딩중 : {progress * 100f}%";
    }

    private async UniTask FadeIn()
    {
        float elapsed = 0f;
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            fadeImage.color = color;
            await UniTask.Yield();
        }
    }

    private async UniTask FadeOut()
    {
        float elapsed = 0f;
        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeImage.color = color;
            await UniTask.Yield();
        }
    }
}
