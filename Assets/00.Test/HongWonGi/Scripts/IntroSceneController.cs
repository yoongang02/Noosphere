using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;
using Random = UnityEngine.Random;


public class IntroSceneController : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeDuration = 0.8f;
    [SerializeField] private PlayableDirector _introTimeLine;
    [SerializeField] private VideoPlayer _introVideo;
    [SerializeField] private Button _skipBtn;
    [Header("대사관련")] 
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject _dialogueGroup;
    [SerializeField] private TextMeshProUGUI _speakerText;
    [SerializeField] float _letterDelay = 0.05f;
    private float _currentTextElapsedTime = 0f;
    private int _currentLetterIndex = 0;
    private bool isTyping = false;
    private ContentSizeFitter contentSizeFitter;
    private RectTransform textRectTransform;


    private void Start()
    {
        contentSizeFitter = dialogueText.gameObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        textRectTransform = dialogueText.GetComponent<RectTransform>();
        textRectTransform.pivot = new Vector2(0, 0.5f);
        // SoundManager.Instance.StopAllSFX();
        // _introTimeLine.p
       
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        SceneManager.LoadSceneAsync("PrologueMap_real");
    //    }
    //}
    public void OnLoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void StartFadeIn()
    {
        FadeIn().Forget();
    }

    private async UniTask FadeIn()
    {
        _fadeImage.color = new Color(0, 0, 0, 1);
        await _fadeImage.DOFade(0, _fadeDuration).AsyncWaitForCompletion();
        _skipBtn.gameObject.SetActive(true);
        // _introTimeLine.Play();
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
    public void StartVfx(string soundResource)
    {
        SoundManager.Instance.PlaySFXNoEffect(soundResource);
    }
    public void ShowDialogue(string fullText)
    {
        _dialogueGroup.SetActive(true);

        string speaker = "";
        string text = fullText;

        if (fullText.Contains(":"))
        {
            int colonIndex = fullText.IndexOf(":");
            speaker = fullText.Substring(0, colonIndex).Trim();
            text = fullText.Substring(colonIndex + 1).Trim();
        }

        _speakerText.text = speaker;
        TypeText(text).Forget();
    }

    public void CloseDialogue()
    {
        _dialogueGroup.SetActive(false);
    }

    private async UniTask TypeText(string text)
    {
        isTyping = true;
        dialogueText.alpha = 0;
        text = text.Replace("\\n", "\n");
        dialogueText.text = text;
        await UniTask.NextFrame();

        float totalWidth = dialogueText.preferredWidth;

        Canvas canvas = dialogueText.transform.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float canvasCenter = canvasRect.rect.width / 2f;
        float startX = canvasCenter - (totalWidth / 2f);
        textRectTransform.anchoredPosition = new Vector2(startX, textRectTransform.anchoredPosition.y);

        dialogueText.text = "";
        dialogueText.alpha = 1;

        StartDialogueSound(_speakerText.text);
        // SoundManager.Instance.PlayLoopingSound("Soundresource_027");
        if (!isTyping)
        {
            dialogueText.text = text;
            return;
        }

        for (int i = 0; i < text.Length && isTyping; i++)
        {
            dialogueText.text += text[i];
            await UniTask.Delay((int)(_letterDelay * 1000));
        }

        dialogueText.text = text;
        isTyping = false;
        SoundManager.Instance.StopAllSFX();
    }


    private void StartDialogueSound(string speaker)
    {
        switch (speaker)
        {
            case "(나)":
                SoundManager.Instance.PlayLoopingSound("Soundresource_026");
                break;
            case "(닥터 로만)":
                SoundManager.Instance.PlayLoopingSound("Soundresource_027");
                break;
            case "(레이)":
                SoundManager.Instance.PlayLoopingSound("Soundresource_077");
                break;
            case "(다프네)":
                SoundManager.Instance.PlayLoopingSound("Soundresource_076");
                break;
            case "":
                SoundManager.Instance.PlayLoopingSound("Soundresource_075");
                break;
            case "(엘리즈 레인)":
                SoundManager.Instance.PlayLoopingSound("Soundresource_075");
                break;
        }
    }

    #endregion
}