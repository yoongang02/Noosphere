using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;

public class InputFieldManager : Singleton<InputFieldManager>
{
    public bool isAnswer = false;
    public bool isSubmitAnswer = false;
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] private TMP_InputField _inputText;
    private string _currentAnswer;
    private string _currentID;

    private void Start()
    {
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
        isSubmitAnswer = false;
        
        if (!DataManager.Instance._input.TryGetValue(id, out InputFieldStructure structure))
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
           
            isSubmitAnswer = true;
            string formattedValue = value.Replace(" ", "");
            string formattedAnswer = _currentAnswer.Replace(" ", "");

            if (formattedValue == formattedAnswer)
            {
                DialogueManager.Instance.SetDialogue(DataManager.Instance._input[_currentID].input_correct);
                isAnswer = true;//isSubmitAnswer = true;
                DataManager.Instance._input[_currentID].isSolved = true;
                if (PlayerController.Instance._currentNPC != null)
                {
                    PlayerController.Instance._currentNPC.GetComponent<NpcDialogue>().dialogueId = string.Empty;
                }
            }
            else
            {
                DialogueManager.Instance.SetDialogue(DataManager.Instance._input[_currentID].input_wrong);
                //isSubmitAnswer = false;
                isAnswer = false;//isSubmitAnswer = false;
                // DataManager.Instance._input[_currentID].isSolved = false;
            }

            CloseInput();
        }
    }

    private void CloseInput()
    {
        if (_questionText.transform.parent.gameObject.activeSelf)
        {
            _questionText.transform.parent.gameObject.SetActive(false);
            _inputText.text = ""; 
        }
    }

    private void CloseInputField()
    {
        // if (_questionText.transform.parent.gameObject.activeSelf)
        // {
        //     isAnswer = false;
        //     isSubmitAnswer = false;
        //     // PlayerController.Instance.isDialogueOn = false;
        //     _questionText.transform.parent.gameObject.SetActive(false);
        //     _inputText.text = ""; 
        // }
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.exitBtnAction -= CloseInputField;
        }
    }
}