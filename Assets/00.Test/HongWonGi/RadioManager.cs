using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class RadioManager : MonoBehaviour
{
    [SerializeField] private GameObject radioPanel;
    [Header("RadioUI")]
    [SerializeField] private List<DialBtn> _dialBtn;
    [SerializeField] private TextMeshProUGUI _radioText;
    [SerializeField] private Button _powerBtn;

    [Header("WorldDialogueUI")]
    [SerializeField] private TextMeshProUGUI _realText;
    [SerializeField] private TextMeshProUGUI _mirrorText;

    [Header("Animation Settings")] 
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private float _displayDuration = 2f;
    private int _tenDigit = 0;
    private int _oneDigit = 0;
    private int _decimalDigit = 0;

    //테스트용 나중에 삭제
    private Dictionary<string, DialogueStructure> _testDialogue = new Dictionary<string, DialogueStructure>();

    private bool isDataLoaded = false;

    //삭제
    private async void Awake()
    {
        await LoadTestDialogue();
    }


    private void Start()
    {
        _powerBtn.onClick.AddListener(CheckAnswer);
    }

    private void OnEnable()
    {
        ResetText();
        if (_dialBtn != null && _dialBtn.Count >= 3)
        {
            _dialBtn[0].OnValueChanged += OnTenDigitChanged;
            _dialBtn[1].OnValueChanged += OnOneDigitChanged;
            _dialBtn[2].OnValueChanged += OnDecimalDigitChanged;
        }
    }

    private void OnDisable()
    {
        if (_dialBtn != null && _dialBtn.Count >= 3)
        {
            _dialBtn[0].OnValueChanged -= OnTenDigitChanged;
            _dialBtn[1].OnValueChanged -= OnOneDigitChanged;
            _dialBtn[2].OnValueChanged -= OnDecimalDigitChanged;
        }
    }

    /// <summary>
    /// ////////////////////
    /// </summary>
    private async UniTask LoadTestDialogue()
    {
        try
        {
            DialogueCSVParser parser = new DialogueCSVParser();
            _testDialogue = await parser.Parse("Dialogue");
            isDataLoaded = true;
            Debug.Log("테스트 대화 데이터 로드 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"대화 데이터 로드 실패: {e.Message}");
        }
    }

    /// <summary>
    /// //////
    /// </summary>
    private void CheckAnswer()
    {
        // if (_radioText.text == "95.3MHz")
        if (_radioText.text == "00.0MHz")
        {
            gameObject.SetActive(false);
            ShowDialogue().Forget();
        }
        else
        {
            Debug.Log("오답");
        }
    }

    private async UniTaskVoid ShowDialogue()
    {
        // _realText.text = $"<mark=#00000055>{DataManager.Instance._dialogue["Dialogue_0027"].dialogueText}</mark>";
        //_mirrorText.text=$"<mark=#00000055>{DataManager.Instance._dialogue["Dialogue_0028"].dialogueText}</mark>";
        _realText.text = $"<mark=#00000055>{_testDialogue["Dialogue_0027"].dialogueText}</mark>";
        _mirrorText.text = $"<mark=#00000055>{_testDialogue["Dialogue_0028"].dialogueText}</mark>";
        // 페이드 인 애니메이션
        var fadeInReal = _realText.DOFade(1f, _fadeDuration);
        var fadeInMirror = _mirrorText.DOFade(1f, _fadeDuration);

        await UniTask.WhenAll(
            fadeInReal.AsyncWaitForCompletion().AsUniTask(),
            fadeInMirror.AsyncWaitForCompletion().AsUniTask()
        );

        await UniTask.Delay(TimeSpan.FromSeconds(_displayDuration));


        var fadeOutReal = _realText.DOFade(0f, _fadeDuration);
        var fadeOutMirror = _mirrorText.DOFade(0f, _fadeDuration);

        await UniTask.WhenAll(
            fadeOutReal.AsyncWaitForCompletion().AsUniTask(),
            fadeOutMirror.AsyncWaitForCompletion().AsUniTask()
        );

        _realText.gameObject.SetActive(false);
        _mirrorText.gameObject.SetActive(false);
    }

    private void ResetText()
    {
        _tenDigit = 0;
        _oneDigit = 0;
        _decimalDigit = 0;
        _radioText.text = $"{_tenDigit}{_oneDigit}.{_decimalDigit}MHz";
    }

    private void OnTenDigitChanged(int value)
    {
        _tenDigit = value;
        UpdateRadioText();
    }

    private void OnOneDigitChanged(int value)
    {
        _oneDigit = value;
        UpdateRadioText();
    }

    private void OnDecimalDigitChanged(int value)
    {
        _decimalDigit = value;
        UpdateRadioText();
    }

    private void UpdateRadioText()
    {
        _radioText.text = $"{_tenDigit}{_oneDigit}.{_decimalDigit}MHz";
    }
}