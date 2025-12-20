using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.Localization.Settings;
using Debug = NooSphere.Debug;

public class InputFieldManager : UIBase
{
    [Header("Input Field 변수")] public bool isAnswer = false;
    [SerializeField] private GameObject _inputFieldUI;
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
        EscapeUI.Instance.Active();
    }
    

    public override void OnClose()
    {
        base.OnClose();
        _inputFieldUI.SetActive(false);
        
        InitInputField();
        _currentID = "";
        
        QuizManager.Instance.OnQuizEnd?.Invoke();
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

        // _currentAnswer = structure.correctAnswer;
        _currentAnswer = AnswerName();
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
            string local = LocalizationSettings.SelectedLocale.ToString();
            if (local == "English (en)")
            {
                formattedValue = formattedValue.ToLower();
                formattedAnswer = formattedAnswer.ToLower();
            }
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

            //Input Field UI 종료
            UIManager.Instance.CloseTopUI();
        }
    }

    private void InitInputField()
    {
        _inputText.text = "";
    }

    private string AnswerName()
    {
        string local = LocalizationSettings.SelectedLocale.ToString();
        string answer="";
        switch (local)
        {
            case"English (en)":
                answer= "Rachel Ko";
                break;
            case "Korean (ko)":
                answer="레이첼 코";
                break;
            case "Chinese (Simplified)(zh)" :
                answer="雷切尔科";
                break;
        }
        return answer;
    }
}