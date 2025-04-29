using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FirstEndingBranch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 레이한테 앨리즈의 편지를 주었다면
            if (DataManager.Instance._events["Event_C066"].isExecuted)
            {
                Debug.LogWarning("[진엔딩] 레이 등장");
                EventManagerYKM.Instance.ExecuteEvent("Event_D100").Forget();
            }
            else
            {
                Debug.LogWarning("[일반 엔딩] 닥터와의 접촉");
                EventManagerYKM.Instance.ExecuteEvent("Event_D102").Forget();
            }
        }
    }
}
