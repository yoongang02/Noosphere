using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FinalStageRealStart : MonoBehaviour
{
    private void OnEnable()
    {
        EventManagerYKM.Instance.ExecuteEvent("Event_D098").Forget();
    }
}
