using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Stage2SpiritManager : MonoBehaviour
{
    private void Start()
    {
        if (DataManager.Instance._events["Event_C074"].isExecuted) return;
        
        EffectManager.Instance.ReverseProcessEffect().Forget();
    }
}
