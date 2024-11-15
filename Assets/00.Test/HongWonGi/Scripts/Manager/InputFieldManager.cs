using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;

public class InputFieldManager : Singleton<InputFieldManager>
{
    private Dictionary<string, InputFieldStructure> _input = new Dictionary<string, InputFieldStructure>();
    public bool isAnswer = false;
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] private TMP_InputField _inputText;
    private string _currentAnswer;
    private string _currentID;

    private void Start()
    {
        InitializeDialogue().Forget();
        _inputText.onEndEdit.AddListener(OnInputFieldEndEdit);
        InputManager.Instance.exitBtnAction += CloseInputField;
    }
    /// <summary>
    // inputfield 활성화 되고 inputfield 활성화
    /// </summary>
    /// <param name="id"> input_id 값</param>
    public void SetQuestionField(string id)
    {
        _questionText.transform.parent.gameObject.SetActive(true);
        PlayerController.Instance.isDialogueOn = true;
        _currentID = id;
        isAnswer = false;
        if (!_input.TryGetValue(id, out InputFieldStructure structure))
        {
            Debug.LogWarning($"ID {id}에 해당하는 InputFieldStructure를 찾을 수 없습니다.");
            return;
        }

        _questionText.text = structure.question_text;
        _currentAnswer = structure.correct_answer;
        _inputText.text = "";
        
        _inputText.Select();
        _inputText.ActivateInputField();
    }
    private void OnInputFieldEndEdit(string value)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (value == _currentAnswer)
            {
                DialogueManager.Instance.SetDialogue(_input[_currentID].input_correct);
                isAnswer = true;
            }
            else
            {
                DialogueManager.Instance.SetDialogue(_input[_currentID].input_wrong);
                isAnswer = false;
            }

            CloseInputField();
        }
    }
    private async UniTaskVoid InitializeDialogue()
    {
        await LoadDialogue("Input");
    }
    private async UniTask LoadDialogue(string sheetName)
    {
        CSVParserYKM parser = new CSVParserYKM();
        _input = await parser.Parse<InputFieldStructure>(sheetName);
        Debug.Log("Input 로드 완료");
    }
    private void CloseInputField()
    {
        if (_questionText.transform.parent.gameObject.activeSelf)
        {
            _questionText.transform.parent.gameObject.SetActive(false);
            PlayerController.Instance.isDialogueOn = false;
            _inputText.text = ""; 
        }
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.exitBtnAction -= CloseInputField;
        }
    }
}