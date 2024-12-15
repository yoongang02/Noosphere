using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MirrorPuzzleManager :Singleton<MirrorPuzzleManager>
{
    [SerializeField] private List<GameObject> mirrorPieces;
    public bool isPuzzleClear = false;
    public Action OnResetPuzzle;
    public GameObject ui;

    public void GetMirrorPiece(int mirrorIdx)
    {
        mirrorPieces[mirrorIdx].SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ResetAllPieces();
        }
    }

    public void CheckAnswer()
    { 
        bool allCorrect = true;
        
        foreach (var piece in mirrorPieces)
        {
            if (!piece.GetComponent<PuzzlePiece>().isRight)
            {
                allCorrect = false;
                break;
            }
        }
            
        if (allCorrect)
        {
            isPuzzleClear = true;
            ui.SetActive(false);
            Debug.Log("퍼즐 클리어!");
        }
        
    }

    public void ResetAllPieces()//거울 퍼즐 초기화
    {
        OnResetPuzzle?.Invoke();
    }
    
}
