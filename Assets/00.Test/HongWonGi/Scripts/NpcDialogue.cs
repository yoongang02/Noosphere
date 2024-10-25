using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// npc 대화 관련
// 최초 작성자: 홍원기
// 수정자: 
// 최종 수정일: 2024-10-25
public class NpcDialogue : MonoBehaviour
{
    [SerializeField] private float _detectionRange;
    [SerializeField] private DialogueSO _dialogueSO;
    private GameObject _player;
    private int currentDialogueIndex = 0;
    public bool isOnDialogue = false;
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    public void StartDialogue()
    {
        isOnDialogue = true;
        currentDialogueIndex = 0;
        DisplayDialogue();
    }

    public void AdvanceDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < _dialogueSO.dialogueCount)
        {
            DisplayDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    private void DisplayDialogue()
    {
        UIManager.Instance.dialogueUI.text = _dialogueSO.dialogueText[currentDialogueIndex];
    }

    private void EndDialogue()
    {
        isOnDialogue = false;
        UIManager.Instance.popUpUI.SetActive(false);
    }
    public void ResetDialogue()
    {
        currentDialogueIndex = 0;
        isOnDialogue = false;
    }
}