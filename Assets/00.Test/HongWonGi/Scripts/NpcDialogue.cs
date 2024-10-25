using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
// npc 대화 관련
// 최초 작성자: 홍원기
// 수정자: 
// 최종 수정일: 2024-10-25
public class NpcDialogue : MonoBehaviour
{
    [SerializeField] private float _detectionRange;
    [SerializeField] private DialogueSO _dialogueSO;
    [SerializeField] private float _typingSpeed;
    private int _currentDialogueIndex = 0;
    private bool _isTyping = false; 
    public bool isOnDialogue = false;
    public void StartDialogue()
    {
        isOnDialogue = true;
        _currentDialogueIndex = 0;
        DisplayDialogue().Forget(); 
    }

    public void AdvanceDialogue()
    {
        if (_isTyping) return; 

        _currentDialogueIndex++;
        if (_currentDialogueIndex < _dialogueSO.dialogueCount)
        {
            DisplayDialogue().Forget();
        }
        else
        {
            EndDialogue();
        }
    }

    private async UniTaskVoid DisplayDialogue()
    {
        _isTyping = true; 

        string fullText = _dialogueSO.dialogueText[_currentDialogueIndex];
        UIManager2.Instance.dialogueUI.text = ""; 
        
        foreach (char letter in fullText)
        {
            UIManager2.Instance.dialogueUI.text += letter;
            await UniTask.Delay(TimeSpan.FromSeconds(_typingSpeed)); 
        }

        _isTyping = false; 
    }

    private void EndDialogue()
    {
        isOnDialogue = false;
        UIManager2.Instance.popUpUI.SetActive(false);
    }

    public void ResetDialogue()
    {
        _currentDialogueIndex = 0;
        isOnDialogue = false;
    }
}