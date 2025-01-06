using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardKeyManager : UIBase
{
    public int curCardID =-1;
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
        curCardID = -1; 
    }
}