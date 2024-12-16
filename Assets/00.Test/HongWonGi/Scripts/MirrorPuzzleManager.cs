using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MirrorPuzzleManager : Singleton<MirrorPuzzleManager>
{
    [SerializeField] private List<GameObject> _mirrorPieces;
    [SerializeField] private GameObject _mirror;
    [SerializeField] private GameObject _brokeMirror;
    [SerializeField] private GameObject _mirrorPanel;
    public bool isPuzzleClear = false;
    public Action OnResetPuzzle;
    public bool isMirrorBroke = false;


    public void GetMirrorPiece(int mirrorIdx)
    {
        if (!isMirrorBroke)
        {
            Debug.Log("거울 안깨져서 조각 획득 x");
            return;
        }

        _mirrorPieces[mirrorIdx].SetActive(true);
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

        foreach (var piece in _mirrorPieces)
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
            _mirrorPanel.SetActive(false);
            _mirror.SetActive(true);
            _brokeMirror.SetActive(false);
            Debug.Log("퍼즐 클리어!");
        }
    }

    public void ResetAllPieces() //거울 퍼즐 초기화
    {
        OnResetPuzzle?.Invoke();
    }
}