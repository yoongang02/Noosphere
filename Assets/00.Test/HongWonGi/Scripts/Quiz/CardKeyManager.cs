using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using NooSphere;
using Debug = NooSphere.Debug;  

public class CardKeyManager : UIBase
{
    string[] _cardKeys = {"Evidence_011","Evidence_012","Evidence_013","Evidence_014"};
    
    void OnEnable()
    {
        UIManager.Instance.OpenUI(this);
    }

    public override void OnOpen()
    {
        base.OnOpen();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        transform.GetChild(0).gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }
}