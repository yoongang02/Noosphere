using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PointEffect : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //EffectManager.Instance.OnEffectEnd?.Invoke();
            EventManagerYKM.Instance.ExecuteEvent(EventManagerYKM.Instance.nextEventID).Forget();
            gameObject.SetActive(false);
        }
    }
}
