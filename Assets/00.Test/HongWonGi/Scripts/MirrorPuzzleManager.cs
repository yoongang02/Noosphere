using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MirrorPuzzleManager : UIBase
{ 
    //싱글톤
    private static MirrorPuzzleManager _instance;
    
    public static MirrorPuzzleManager Instance 
    { 
        get 
        { 
            if (_instance == null) 
            {
                _instance = FindObjectOfType<MirrorPuzzleManager>();
                if (_instance == null) 
                {
                    GameObject singletonObject = new GameObject(nameof(MirrorPuzzleManager));
                    _instance = singletonObject.AddComponent<MirrorPuzzleManager>();
                }
            }
            return _instance;
        } 
    }

    void Awake(){
        
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        //DontDestroyOnLoad(gameObject); 
    }
    
    [SerializeField] private GameObject _mirrorPanel;
    [SerializeField] private List<GameObject> _mirrorPieces;
    [Header("World Mirror Object")]
    [SerializeField] private GameObject _mirror;
    [SerializeField] private GameObject _brokeMirror;
   
    public bool isPuzzleClear = false;
    public Action OnResetPuzzle;
    public bool isMirrorBroke = false;


    public override void OnOpen()
    {
        base.OnOpen();
        ResetAllPieces();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        ResetAllPieces();
        transform.GetChild(0).gameObject.SetActive(false);
    }
    
    public void GetMirrorPiece(int mirrorIdx)
    {
        if (!isMirrorBroke)
        {
            Debug.Log("거울 안깨져서 조각 획득 x");
            return;
        }

        _mirrorPieces[mirrorIdx].SetActive(true);
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
            Debug.Log("퍼즐 클리어!");
            isPuzzleClear = true;
            //거울 원상복구
            _mirror.SetActive(true);
            _brokeMirror.SetActive(false);
            //현실 세계로 돌아갈 수 있도록 변경
            DataManager.Instance._lockConditions["Lock_condition_005"].UnLock();
            PlayerInteract.Instance.GetComponent<MentalEnterProcess>().SetCombackEventId("Event_B059");
            
            UIManager.Instance.CloseTopUI();
        }
    }

    public void ResetAllPieces() //거울 퍼즐 초기화
    {
        OnResetPuzzle?.Invoke();
    }
}