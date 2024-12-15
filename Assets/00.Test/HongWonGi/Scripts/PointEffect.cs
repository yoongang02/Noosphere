using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointEffect : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //EffectManager.Instance.OnEffectEnd?.Invoke();
            CoroutineManager.Instance.StartManagedCoroutine(EventManagerYKM.Instance.ExecuteEvent(EventManagerYKM.Instance.nextEventID));
            gameObject.SetActive(false);
        }
    }
}
