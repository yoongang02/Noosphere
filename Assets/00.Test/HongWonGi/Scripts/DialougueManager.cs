using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

// class DialogueStructure
// {
//     public string dialogue_id;
//     public string character_id;
//     public string dialogue_Text;
//     public string next_dialouge_id;
//
//     // 리스트로 분리된 대사
//     public List<string> Dialogue_Text_List
//     {
//         get
//         {
//             return new List<string>(dialogue_Text.Split('/'));
//         }
//     }
// }

public class DialougueManager : MonoBehaviour
{
    private Dictionary<string, DialogueStructure> _dialogue = new Dictionary<string, DialogueStructure>();
    private string currentDialogueId = "";
    private int currentLineIndex = 0;
   private string initialDialogueId = "";
    void Start()
    {
        InitializeDialogue().Forget();
    }
    private async UniTaskVoid InitializeDialogue()
    {
        await LoadDialogue("Dialogue");
    }
    //이벤트 로드
    private async UniTask LoadDialogue(string sheetName)
    {
        CSVParserYKM parser = new CSVParserYKM();
        _dialogue = await parser.Parse<DialogueStructure>(sheetName);
        Debug.Log("로드 완료");
        SetDialogue("dialogue_0001");
    }

    private void SetDialogue(string id)
    {
        currentDialogueId = id;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (string.IsNullOrEmpty(currentDialogueId))
            {
                // 대화가 시작되지 않은 경우 초기 대화 시작
                StartDialogue(initialDialogueId);
            }
            else
            {
                // 대화가 진행 중인 경우 다음 대사 보여주기
                ShowNextLine();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Esc 키 입력 시 대화를 처음부터 다시 시작
            RestartDialogue();
        }
    }

    public void StartDialogue(string dialogueId)
    {
        if (_dialogue.ContainsKey(dialogueId))
        {
            currentDialogueId = dialogueId;
            currentLineIndex = 0;
            ShowNextLine();
        }
        else
        {
            Debug.LogWarning($"Dialogue ID {dialogueId} not found.");
        }
    }
    public void RestartDialogue()
    {
        Debug.Log("대화를 처음부터 다시 시작합니다.");
        StartDialogue(initialDialogueId);
    }
    private void ShowNextLine()
    {
        if (string.IsNullOrEmpty(currentDialogueId))
        {
            Debug.LogWarning("현재 대화 ID가 설정되지 않았습니다.");
            return;
        }

        if (!_dialogue.TryGetValue(currentDialogueId, out DialogueStructure dialogue))
        {
            Debug.LogWarning($"Dialogue ID {currentDialogueId} not found.");
            return;
        }

        if (currentLineIndex < dialogue.Dialogue_Text_List.Count)
        {
            Debug.Log(dialogue.Dialogue_Text_List[currentLineIndex]);
            currentLineIndex++;
        }
        else
        {
            if (!string.IsNullOrEmpty(dialogue.next_dialouge_id))
            {
                if (_dialogue.ContainsKey(dialogue.next_dialouge_id))
                {
                    currentDialogueId = dialogue.next_dialouge_id;
                    currentLineIndex = 0;
                    ShowNextLine();
                }
                else
                {
                    Debug.LogWarning($"Next Dialogue ID {dialogue.next_dialouge_id} not found.");
                }
            }
            else
            {
                Debug.Log("대화가 종료되었습니다.");
                currentDialogueId = "";
                currentLineIndex = 0;
            }
        }
    }
}
