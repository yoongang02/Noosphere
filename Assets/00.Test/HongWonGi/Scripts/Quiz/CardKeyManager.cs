using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using NooSphere;
using Debug = NooSphere.Debug;  

public class CardKeyManager : UIBase
{
    [SerializeField] private string _curQuizID;
    private QuizStructure _curQuiz;
    public string curCardEvidenceID;
    private static CardKeyManager _instance;

    public static CardKeyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CardKeyManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(nameof(CardKeyManager));
                    _instance = singletonObject.AddComponent<CardKeyManager>();
                }
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        curCardEvidenceID = string.Empty;
    }

    public override void OnOpen(string quizID)
    {
        base.OnOpen(quizID);
        _curQuizID = quizID;
        _curQuiz = DataManager.Instance._quiz[_curQuizID];
        Debug.Log($"# quiz id : {quizID}, _curQuiz : {_curQuiz}");
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        transform.GetChild(0).gameObject.SetActive(false);
        if (!string.IsNullOrEmpty(curCardEvidenceID))
        {
            //TODO: 닫았을때 인벤토리에 아이템 획득 or 갈아끼우는 동작
        }
        else
        {
            //TODO: 닫았을때 인벤토리에서 삭제
        }


        _curQuizID = "";
        _curQuiz = null;
    }
}