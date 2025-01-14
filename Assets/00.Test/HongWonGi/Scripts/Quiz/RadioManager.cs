using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class RadioManager : UIBase
{
    [SerializeField] private string _curQuizID;
    private QuizStructure _curQuiz;
    
    [SerializeField] private GameObject _radioPanel;
    [Header("RadioUI")] 
    [SerializeField] private List<DialBtn> _dialBtn;
    [SerializeField] private TextMeshProUGUI _radioText;
    [SerializeField] private Button _powerBtn;

    [Header("WorldDialogueUI")] 
    [SerializeField] private TextMeshProUGUI _realText;
    [SerializeField] private TextMeshProUGUI _mirrorText;

    [SerializeField] private string radioAnswer;
    private float _fadeDuration = 0.5f;
    private float _displayDuration = 1f;
    private int _tenDigit = 0;
    private int _oneDigit = 0;
    private int _decimalDigit = 0;

    //테스트용 나중에 삭제
    private Dictionary<string, DialogueStructure> _testDialogue = new Dictionary<string, DialogueStructure>();

    private bool isDataLoaded = false;

    public override void OnOpen(string quizID)
    {
        base.OnOpen(quizID);
        _curQuizID = quizID;
        _curQuiz = DataManager.Instance._quiz[_curQuizID];
        Debug.Log($"# quiz id : {quizID}, _curQuiz : {_curQuiz}");
        
        //정신세계인데
        //만약 거울이 깨졌는데 기믹을 미리 성공했다면
        if (_curQuiz.isSolved)
        {
            /*
            if (PlayerInteract.Instance.isInMental)
            {
                //조각을 이미 습득했다면 -> 라디오 켜기
                if (InventoryManager.Instance.IsAcquiredEvidence("Evidence_023") || !MirrorPuzzleManager.Instance.isMirrorBroke)
                {
                    DataManager.Instance._lockConditions["Lock_condition_003"].Lock();
                    ShowDialogue().Forget();
                }
            }
            else
            {
                DataManager.Instance._lockConditions["Lock_condition_003"].Lock();
                ShowRealDialogue().Forget();
            }
            */
            //퀴즈 실행되지 않음
            UIManager.Instance.CloseTopUI();
            QuizManager.Instance.OnQuizEnd?.Invoke();
            return;
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
        transform.GetChild(0).gameObject.SetActive(false);
        
        _curQuizID = "";
        _curQuiz = null;
    }
    /*
    //삭제
    private async void Awake()
    {
        await LoadTestDialogue();
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
*/
    /// <summary>
    /// //////
    /// </summary>
    private void CheckAnswer()
    {
        if (_radioText.text == radioAnswer)
        {
            SoundManager.Instance.PlaySound("Soundresource_049",1);
            if (PlayerInteract.Instance.isInMental)
            {
                //현실세계 정신세계 구분
                ShowDialogue().Forget();
            }
            else
            {
                ShowRealDialogue().Forget();
            }
            //퀴즈 해결되었다고 표시
            _curQuiz.isSolved = true;
            UIManager.Instance.CloseTopUI();
        }
        else
        {
            SoundManager.Instance.PlaySound("Soundresource_001",1);
            Debug.Log("오답");
        }
    }

    private async UniTask ShowRealDialogue()
    {
        DialogueStructure mirrorDialogue = DataManager.Instance._dialogue["Dialogue_0024"];
        // DialogueStructure mirrorDialogue = _testDialogue["Dialogue_0024"];
        
        _realText.gameObject.SetActive(true);
        PlayerInteract.Instance.HideInteractionMark();
        for (int i = 0; i < mirrorDialogue.Dialogue_Text_List.Count; i++)
        {
            DataManager.Instance._lockConditions["Lock_condition_003"].Lock();
            _realText.text = $"<mark=#00000055>{mirrorDialogue.Dialogue_Text_List[i]}</mark>";

            // 페이드 인
            await _realText.DOFade(1f, _fadeDuration).AsyncWaitForCompletion();

            // 표시 시간 대기
            await UniTask.Delay(TimeSpan.FromSeconds(_displayDuration));

            // 페이드 아웃
            await _realText.DOFade(0f, _fadeDuration).AsyncWaitForCompletion();

            // 마지막이 아니면 잠시 대기
            if (i < mirrorDialogue.Dialogue_Text_List.Count - 1)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            }
        }
        
        _realText.gameObject.SetActive(false);
        DataManager.Instance._lockConditions["Lock_condition_003"].UnLock();
        QuizManager.Instance.OnQuizEnd?.Invoke();
    }

    private async UniTaskVoid ShowDialogue()
    {
        // DialogueStructure realDialogue = _testDialogue["Dialogue_0027"];
        // DialogueStructure mirrorDialogue = _testDialogue["Dialogue_0028"];
        DialogueStructure realDialogue = DataManager.Instance._dialogue["Dialogue_0027"];
        DialogueStructure mirrorDialogue = DataManager.Instance._dialogue["Dialogue_0028"];

        DataManager.Instance._lockConditions["Lock_condition_003"].Lock();
        
        _realText.gameObject.SetActive(true);
        _mirrorText.gameObject.SetActive(true);
        // 더 긴 리스트의 길이만큼 반복
        int maxLength = Mathf.Max(realDialogue.Dialogue_Text_List.Count, mirrorDialogue.Dialogue_Text_List.Count);

        for (int i = 0; i < maxLength; i++)
        {
            DataManager.Instance._lockConditions["Lock_condition_003"].Lock();
            // 각 텍스트가 있을 경우에만 표시
            if (i < realDialogue.Dialogue_Text_List.Count)
            {
                _realText.text = $"<mark=#00000055>{realDialogue.Dialogue_Text_List[i]}</mark>";
            }

            if (i < mirrorDialogue.Dialogue_Text_List.Count)
            {
                _mirrorText.text = $"<mark=#00000055>{mirrorDialogue.Dialogue_Text_List[i]}</mark>";
            }

            // 동시에 페이드 인
            var fadeInTasks = new List<UniTask>();
            if (i < realDialogue.Dialogue_Text_List.Count)
            {
                fadeInTasks.Add(_realText.DOFade(1f, _fadeDuration).AsyncWaitForCompletion().AsUniTask());
            }

            if (i < mirrorDialogue.Dialogue_Text_List.Count)
            {
                fadeInTasks.Add(_mirrorText.DOFade(1f, _fadeDuration).AsyncWaitForCompletion().AsUniTask());
            }

            await UniTask.WhenAll(fadeInTasks);

            // 표시 시간 대기
            await UniTask.Delay(TimeSpan.FromSeconds(_displayDuration));

            // 동시에 페이드 아웃
            var fadeOutTasks = new List<UniTask>();
            if (i < realDialogue.Dialogue_Text_List.Count)
            {
                fadeOutTasks.Add(_realText.DOFade(0f, _fadeDuration).AsyncWaitForCompletion().AsUniTask());
            }

            if (i < mirrorDialogue.Dialogue_Text_List.Count)
            {
                fadeOutTasks.Add(_mirrorText.DOFade(0f, _fadeDuration).AsyncWaitForCompletion().AsUniTask());
            }

            await UniTask.WhenAll(fadeOutTasks);

            // 마지막이 아니면 잠시 대기
            if (i < maxLength - 1)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            }
        }

        _realText.gameObject.SetActive(false);
        _mirrorText.gameObject.SetActive(false);
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
}