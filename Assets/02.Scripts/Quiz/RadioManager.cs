using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Random = UnityEngine.Random;

public class RadioManager : UIBase
{
    [SerializeField] private string _curQuizID;
    private QuizStructure _curQuiz;

    [SerializeField] private GameObject _radioPanel;
    [Header("RadioUI")] [SerializeField] private List<DialBtn> _dialBtn;
    [SerializeField] private TextMeshProUGUI _radioText;
    [SerializeField] private Button _powerBtn;

    [Header("WorldDialogueUI")] [SerializeField]
    private TextMeshProUGUI _realText;

    [SerializeField] private TextMeshProUGUI _mirrorText;

    [SerializeField] private string radioAnswer;
    private float _fadeDuration = 0.5f;
    private float _displayDuration = 1f;
    private int _tenDigit = 0;
    private int _oneDigit = 0;
    private int _decimalDigit = 0;

    private bool _isSkipping = false;
    private static bool _alreadyShowDialogue = false;
    private static bool _alreadyShowOnSpirit = false;

    [SerializeField] private Button _skipBtn;

    private void Awake()
    {
        _skipBtn.onClick.RemoveAllListeners();
        _skipBtn.onClick.AddListener(() => _isSkipping = true);
    }

    public override async void OnOpen(string quizID)
    {
        base.OnOpen(quizID);
        _curQuizID = "";
        _curQuiz = null;
        _curQuizID = quizID;
        _curQuiz = DataManager.Instance._quiz[_curQuizID];
        Debug.Log($"# quiz id : {quizID}, _curQuiz : {_curQuiz}");
        
        //정신세계인데
        //만약 거울이 깨졌는데 기믹을 미리 성공했다면
        transform.GetChild(0).gameObject.SetActive(false);
        if (_curQuiz.isSolved)
        {
            await UniTask.Yield();
            //퀴즈 실행되지 않음
            UIManager.Instance.CloseTopUI();

            if (PlayerInteract.Instance.isInMental)
            {
                //거울이 깨져있다면
                if (MirrorPuzzleManager.Instance.isMirrorBroke)
                {
                    //조각을 습득하지 않았다면 습득 먼저 진행
                    if (!DataManager.Instance._evidences["Evidence_023"].isAcquired)
                    {
                        await GetMirrorPiece();
                    }

                    DataManager.Instance._lockConditions["Lock_condition_003"].Lock();
                    await ShowDialogue();
                }
                else //거울이 깨져있지 않다면
                {
                    DataManager.Instance._lockConditions["Lock_condition_003"].Lock();
                    await ShowDialogue();
                }
            }
            else
            {
                DataManager.Instance._lockConditions["Lock_condition_003"].Lock();
                await ShowRealDialogue();
            }

            return;
        }
        
        // 스킵 버튼 상태 설정
        if (_skipBtn != null)
        {
            if (_skipBtn != null)
            {
                bool showSkip = PlayerInteract.Instance.isInMental
                    ? _alreadyShowOnSpirit
                    : _alreadyShowDialogue;
                _skipBtn.gameObject.SetActive(showSkip);
            }
        }

        ResetText();
        if (_dialBtn != null && _dialBtn.Count >= 3)
        {
            _dialBtn[0].OnValueChanged += OnTenDigitChanged;
            _dialBtn[1].OnValueChanged += OnOneDigitChanged;
            _dialBtn[2].OnValueChanged += OnDecimalDigitChanged;
        }

        _powerBtn.onClick.AddListener(CheckAnswer);
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        if (_dialBtn != null && _dialBtn.Count >= 3)
        {
            _dialBtn[0].OnValueChanged -= OnTenDigitChanged;
            _dialBtn[1].OnValueChanged -= OnOneDigitChanged;
            _dialBtn[2].OnValueChanged -= OnDecimalDigitChanged;
        }

        // 스킵 플래그 초기화
        _isSkipping = false;

        // 스킵 버튼 비활성화
        if (_skipBtn != null)
        {
            _skipBtn.gameObject.SetActive(false);
        }

        transform.GetChild(0).gameObject.SetActive(false);
        UIManager.Instance.cctvFrame.SetActive(true);
    }

    private async void CheckAnswer()
    {
        SoundManager.Instance.PlaySFX("Soundresource_071");
        if (_radioText.text == radioAnswer)
        {
            //퀴즈 해결되었다고 표시
            _curQuiz.isSolved = true;
            SoundManager.Instance.PlaySFX("Soundresource_083");
            UIManager.Instance.CloseTopUI();

            if (PlayerInteract.Instance.isInMental)
            {
                //조각을 습득하지 않았다면
                if (!DataManager.Instance._evidences["Evidence_023"].isAcquired &&
                    MirrorPuzzleManager.Instance.isMirrorBroke)
                {
                    await GetMirrorPiece();
                }

                //현실세계 정신세계 구분
                await ShowDialogue();
            }
            else
            {
                await ShowRealDialogue();
            }
        }
        else
        {
            SoundManager.Instance.PlaySFX("Soundresource_001");
            Debug.Log("오답");
        }
    }

    private async UniTask ShowRealDialogue()
    {
        // 스킵 관련 초기화
        _isSkipping = false;

        // 스킵 버튼 상태 설정
        if (_skipBtn != null)
        {
            _skipBtn.gameObject.SetActive(_alreadyShowDialogue);
        }

        DialogueStructure mirrorDialogue = DataManager.Instance._dialogue["Dialogue_0024"];
        DataManager.Instance._lockConditions["Lock_condition_003"].Lock();
        _realText.gameObject.SetActive(true);
        _realText.alpha = 0f;
        //PlayerInteract.Instance.HideInteractionMark();
        
        for (int i = 0; i < mirrorDialogue.Dialogue_Text_List.Count; i++)
        {
            // 첫 대화가 아니고 스킵 요청이 있을 경우 체크
            if (_alreadyShowDialogue && _isSkipping) break;

            DataManager.Instance._lockConditions["Lock_condition_003"].Lock();
            int randomNum = Random.Range(48, 54); // 48~53
            SoundManager.Instance.PlaySFX($"Soundresource_0{randomNum}");
            _realText.text = $"<mark=#00000055>{mirrorDialogue.Dialogue_Text_List[i].text}</mark>";

            // 페이드 인
            Tween fadeInTween = _realText.DOFade(1f, _fadeDuration);
            await fadeInTween.AsyncWaitForCompletion();
            if (_alreadyShowDialogue && _isSkipping) break;

            // 표시 시간 대기
            float startTime = Time.time;
            while (Time.time - startTime < _displayDuration)
            {
                if (_alreadyShowDialogue && _isSkipping) break;
                await UniTask.Yield();
            }

            if (_alreadyShowDialogue && _isSkipping) break;

            // 페이드 아웃
            Tween fadeOutTween = _realText.DOFade(0f, _fadeDuration);
            await fadeOutTween.AsyncWaitForCompletion();
            if (_alreadyShowDialogue && _isSkipping) break;

            // 마지막이 아니면 잠시 대기
            if (i < mirrorDialogue.Dialogue_Text_List.Count - 1)
            {
                startTime = Time.time;
                while (Time.time - startTime < 0.5f)
                {
                    if (_alreadyShowDialogue && _isSkipping) break;
                    await UniTask.Yield();
                }

                if (_alreadyShowDialogue && _isSkipping) break;
            }
        }

        // 첫 대화 표시 완료 표시
        _alreadyShowDialogue = true;

        // 정리 작업
        _realText.DOKill();
        _realText.alpha = 0f;
        _realText.gameObject.SetActive(false);

        // 스킵 버튼 비활성화
        if (_skipBtn != null)
        {
            _skipBtn.gameObject.SetActive(false);
        }

        SoundManager.Instance.StopAllSFX();
        DataManager.Instance._lockConditions["Lock_condition_003"].UnLock();
        QuizManager.Instance.OnQuizEnd?.Invoke();
    }

    private async UniTask ShowDialogue()
    {
        // 스킵 관련 초기화
        _isSkipping = false;

        // 스킵 버튼 상태 설정
        if (_skipBtn != null)
        {
            _skipBtn.gameObject.SetActive(_alreadyShowOnSpirit);
        }

        DialogueStructure realDialogue = DataManager.Instance._dialogue["Dialogue_0027"];
        DialogueStructure mirrorDialogue = DataManager.Instance._dialogue["Dialogue_0028"];

        DataManager.Instance._lockConditions["Lock_condition_003"].Lock();

        _realText.gameObject.SetActive(true);
        _mirrorText.gameObject.SetActive(true);
        _realText.alpha = 0f;
        _mirrorText.alpha = 0f;

        // 더 긴 리스트의 길이만큼 반복
        int maxLength = Mathf.Max(realDialogue.Dialogue_Text_List.Count, mirrorDialogue.Dialogue_Text_List.Count);

        for (int i = 0; i < maxLength; i++)
        {
            // 첫 대화가 아니고 스킵 요청이 있을 경우 체크
            if (_alreadyShowOnSpirit && _isSkipping) break;

            DataManager.Instance._lockConditions["Lock_condition_003"].Lock();

            // 각 텍스트가 있을 경우에만 표시
            if (i < realDialogue.Dialogue_Text_List.Count)
            {
                _realText.text = $"<mark=#00000055>{realDialogue.Dialogue_Text_List[i].text}</mark>";
                int randomNum = Random.Range(48, 54); // 48~53
                SoundManager.Instance.PlaySFX($"Soundresource_0{randomNum}");
            }

            if (i < mirrorDialogue.Dialogue_Text_List.Count)
            {
                _mirrorText.text = $"<mark=#00000055>{mirrorDialogue.Dialogue_Text_List[i].text}</mark>";
            }

            // 동시에 페이드 인
            List<Tween> fadeInTweens = new List<Tween>();
            if (i < realDialogue.Dialogue_Text_List.Count)
            {
                fadeInTweens.Add(_realText.DOFade(1f, _fadeDuration));
            }

            if (i < mirrorDialogue.Dialogue_Text_List.Count)
            {
                fadeInTweens.Add(_mirrorText.DOFade(1f, _fadeDuration));
            }

            foreach (var tween in fadeInTweens)
            {
                await tween.AsyncWaitForCompletion();
                if (_alreadyShowOnSpirit && _isSkipping) break;
            }

            if (_alreadyShowOnSpirit && _isSkipping) break;

            // 표시 시간 대기
            float startTime = Time.time;
            while (Time.time - startTime < _displayDuration)
            {
                if (_alreadyShowOnSpirit && _isSkipping) break;
                await UniTask.Yield();
            }

            if (_alreadyShowOnSpirit && _isSkipping) break;

            // 동시에 페이드 아웃
            List<Tween> fadeOutTweens = new List<Tween>();
            if (i < realDialogue.Dialogue_Text_List.Count)
            {
                fadeOutTweens.Add(_realText.DOFade(0f, _fadeDuration));
            }

            if (i < mirrorDialogue.Dialogue_Text_List.Count)
            {
                fadeOutTweens.Add(_mirrorText.DOFade(0f, _fadeDuration));
            }

            foreach (var tween in fadeOutTweens)
            {
                await tween.AsyncWaitForCompletion();
                if (_alreadyShowOnSpirit && _isSkipping) break;
            }

            if (_alreadyShowOnSpirit && _isSkipping) break;

            // 마지막이 아니면 잠시 대기
            if (i < maxLength - 1)
            {
                startTime = Time.time;
                while (Time.time - startTime < 0.5f)
                {
                    if (_alreadyShowOnSpirit && _isSkipping) break;
                    await UniTask.Yield();
                }

                if (_alreadyShowOnSpirit && _isSkipping) break;
            }
        }

        // 첫 대화 표시 완료 표시
        _alreadyShowOnSpirit = true;

        // 정리 작업
        _realText.DOKill();
        _mirrorText.DOKill();
        _realText.alpha = 0f;
        _mirrorText.alpha = 0f;
        _realText.gameObject.SetActive(false);
        _mirrorText.gameObject.SetActive(false);

        // 스킵 버튼 비활성화
        if (_skipBtn != null)
        {
            _skipBtn.gameObject.SetActive(false);
        }

        SoundManager.Instance.StopAllSFX();
        DataManager.Instance._lockConditions["Lock_condition_003"].UnLock();
        QuizManager.Instance.OnQuizEnd?.Invoke();
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

    private async UniTask WaitForInvestigateEndAsync()
    {
        UIManager.Instance.OnSelectEnd = null;
        var tcs = new UniTaskCompletionSource();
        UIManager.Instance.OnSelectEnd += () => tcs.TrySetResult();
        await tcs.Task;
    }

    private async UniTask GetMirrorPiece()
    {
        /*
        UIManager.Instance.OpenUI(UIManager.Instance.investigateUI, DataManager.Instance._evidences["Evidence_023"]);
        //yes, no 선택 기다리기
        await WaitForInvestigateEndAsync();
        Debug.LogWarning($"Investigate UI 버튼 선택 다 기다림.");

        //yes라면
        if (UIManager.Instance.isYesClicked)
        {
            Debug.LogWarning("Investigate UI에서 YES를 선택함.");

            //증거물 상세 ui가 닫힐 때까지 기다리기
            await UniTask.WaitUntil(() => !UIManager.Instance.IsAnyUIOpen());
            Debug.LogWarning("창 닫힐 때까지 다 기다림.");
        }
        else
        {
            //no라면
            Debug.LogWarning("Investigate UI에서 NO를 선택함.");
        }
        */
        DialogueManager.Instance.SetDialogue("Dialogue_0065");
        DataManager.Instance._evidences["Evidence_023"].AcquireEvidence();
        MirrorPuzzleManager.Instance.GetMirrorPiece("Evidence_023");
        await UniTask.Yield();
    }
}