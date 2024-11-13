using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;

public class DialogueManager : Singleton<DialogueManager>
{
    public TextMeshProUGUI dialogueUI; // 대화창 UI 컴포넌트
    public Dictionary<string, DialogueStructure> _dialogue = new Dictionary<string, DialogueStructure>();
    private string _currentDialogueId = "";
    private int _currentLineIndex = 0;
    private string _initialDialogueId = "";

    private float _letterDelay = 0.05f;
    private float _currentTextElapsedTime = 0f;
    private int _currentLetterIndex = 0;
    private bool _isTyping = false;

    private void Start()
    {
        InitializeDialogue().Forget();
    }

    private async UniTaskVoid InitializeDialogue()
    {
        await LoadDialogue("Dialogue");
    }

    private async UniTask LoadDialogue(string sheetName)
    {
        CSVParserYKM parser = new CSVParserYKM();
        _dialogue = await parser.Parse<DialogueStructure>(sheetName);
        Debug.Log("대화 로드 완료");
        
    }

    private void SetDialogue(string id)
    {
        _currentDialogueId = id;
        
        if (_dialogue.TryGetValue(_currentDialogueId, out DialogueStructure dialogue))
        {
            if (dialogue.trigger_type == "auto")//대화창 바로 뜨기
            {
                dialogueUI.gameObject.SetActive(true);
                ShowNextLine().Forget();
            }
            if(dialogue.trigger_type=="interact") 
            {
                //이때는 씬에 있는 npc이름으로 오브젝트 찾아서 npc dialogue 등록해놓고 대화창 data등록하기
                dialogueUI.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning($"Dialogue ID {_currentDialogueId} not found.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (string.IsNullOrEmpty(_currentDialogueId))
            {
                StartDialogue(_initialDialogueId);
            }
            else if (_isTyping) 
            {
                _isTyping = false; 
            }
            else 
            {
                ShowNextLine().Forget();
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            SetDialogue("dialogue_0001");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            SetDialogue("dialogue_0018");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RestartDialogue();
        }
    }

    public void StartDialogue(string dialogueId)
    {
        if (_dialogue.ContainsKey(dialogueId))
        {
            _currentDialogueId = dialogueId;
            _currentLineIndex = 0;
            ShowNextLine().Forget();
        }
        else
        {
            Debug.LogWarning($"Dialogue ID {dialogueId} not found.");
        }
    }

    public void RestartDialogue()
    {
        Debug.Log("대화를 처음부터 다시 시작합니다.");
        StartDialogue(_initialDialogueId);
    }

    private async UniTaskVoid ShowNextLine()
    {
        if (string.IsNullOrEmpty(_currentDialogueId))
        {
            Debug.LogWarning("현재 대화 ID가 설정되지 않았습니다.");
            return;
        }

        if (!_dialogue.TryGetValue(_currentDialogueId, out DialogueStructure dialogue))
        {
            Debug.LogWarning($"Dialogue ID {_currentDialogueId} not found.");
            return;
        }

        if (_currentLineIndex < dialogue.Dialogue_Text_List.Count)
        {
            // 타이핑 시작
            await TypeText(dialogue.Dialogue_Text_List[_currentLineIndex]);
            // 타이핑이 완료되었거나 스킵되었을 때만 다음 라인으로 진행
            _currentLineIndex++;
        }
        else
        {
            if (!string.IsNullOrEmpty(dialogue.next_dialouge_id))
            {
                if (_dialogue.ContainsKey(dialogue.next_dialouge_id))
                {
                    _currentDialogueId = dialogue.next_dialouge_id;
                    _currentLineIndex = 0;
                    ShowNextLine().Forget();
                }
                else
                {
                    Debug.LogWarning($"Next Dialogue ID {dialogue.next_dialouge_id} not found.");
                }
            }
            else
            {
                Debug.Log("대화가 종료되었습니다.");
                _currentDialogueId = "";
                _currentLineIndex = 0;
                dialogueUI.gameObject.SetActive(false);
            }
        }
    }


    private async UniTask TypeText(string text)
    {
        _isTyping = true;
        dialogueUI.text = "";
        
        if (!_isTyping)
        {
            dialogueUI.text = text;
            return;
        }

        for (int i = 0; i < text.Length && _isTyping; i++)
        {
            dialogueUI.text += text[i];
            await UniTask.Delay((int)(_letterDelay * 1000));
        }
        
        dialogueUI.text = text;
        _isTyping = false;
    }
}