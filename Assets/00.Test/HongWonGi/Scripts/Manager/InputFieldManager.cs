using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;

public class InputFieldManager : UIBase
{
    [Header("Input Field 변수")]
    public bool isAnswer = false;
    public Action OnInputEnd;
    [SerializeField] private GameObject _inputFieldUI;
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] private TMP_InputField _inputText;
    private string _currentAnswer;
    private string _currentID;
    

    private void Start()
    {
        _inputText.onEndEdit.AddListener(OnInputFieldEndEdit);
    }

    public override void OnOpen()
    {
        base.OnOpen();
        _inputFieldUI.SetActive(true);
    }
    
    public override void OnClose()
    {
        base.OnClose();
        _inputFieldUI.SetActive(false);

        if (isAnswer)
        {
            //맞았을 때 결과가 있다면 결과 실행
            StartCoroutine(DoCorrectResult());
        }
        else
        {
            //틀렸을 때 결과가 있다면 결과 실행
            StartCoroutine(DoWrongResult());
        }
        
        InitInputField();
        _currentID = "";
    }
    
    /// <summary>
    // inputfield 활성화 되고 inputfield 활성화
    /// </summary>
    /// <param name="id"> input_id 값</param>
    public void SetQuestionField(string id)
    {
        _currentID = id;
        isAnswer = false;
        
        if (!DataManager.Instance._quiz.TryGetValue(id, out QuizStructure structure))
        {
            Debug.LogWarning($"ID {id}에 해당하는 InputFieldStructure를 찾을 수 없습니다.");
            return;
        }

        _questionText.text = structure.questionText;
        _currentAnswer = structure.correctAnswer;
        InitInputField();
        
        //input 입력창 열기
        UIManager.Instance.OpenUI(UIManager.Instance.inputFieldUI);
        
        //input 입력창 활성화
        _inputText.Select();
        _inputText.ActivateInputField();
    }
    private void OnInputFieldEndEdit(string value)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            string formattedValue = value.Replace(" ", "");
            string formattedAnswer = _currentAnswer.Replace(" ", "");

            if (formattedValue == formattedAnswer)
            {
                Debug.Log("#input 정답 맞춤");
                isAnswer = true;
                DataManager.Instance._quiz[_currentID].isSolved = true;
            }
            else
            {
                Debug.Log("#input 정답 못 맞춤");
                isAnswer = false;
                DataManager.Instance._quiz[_currentID].isSolved = false;
            }
            OnInputEnd?.Invoke();
            
            //Input Field UI 종료
            UIManager.Instance.CloseTopUI();
        }
    }

    private void InitInputField()
    {
        _inputText.text = "";
    }

    IEnumerator DoCorrectResult()
    {
        if (!string.IsNullOrEmpty(_currentID) && DataManager.Instance._quiz.ContainsKey(_currentID))
        {
            QuizStructure input = DataManager.Instance._quiz[_currentID];
            string resultType = input.quizCorrects[0].Substring(0, input.quizCorrects[0].IndexOf("_"));

            if (resultType == "Dialogue")
            {
                DialogueManager.Instance.SetDialogue(input.quizCorrects[0]);
                
                bool isDialogueEnd = false;
                DialogueManager.Instance.OnDialogueEnd += () => isDialogueEnd = true;
                yield return new WaitUntil(() => isDialogueEnd);
            }
        }
    }

    IEnumerator DoWrongResult()
    {
        if (!string.IsNullOrEmpty(_currentID) && DataManager.Instance._quiz.ContainsKey(_currentID))
        {
            QuizStructure input = DataManager.Instance._quiz[_currentID];
            string resultType = input.quizWrong.Substring(0, input.quizWrong.IndexOf("_"));

            if (resultType == "Dialogue")
            {
                DialogueManager.Instance.SetDialogue(input.quizWrong);
                
                bool isDialogueEnd = false;
                DialogueManager.Instance.OnDialogueEnd += () => isDialogueEnd = true;
                yield return new WaitUntil(() => isDialogueEnd);
            }
        }
    }
}