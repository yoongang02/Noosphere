using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoungeKeyController : MonoBehaviour
{
    [SerializeField] private GameObject keyTriggerObject;

    private void OnEnable()
    {
        if (DataManager.Instance._events["Event_C063"].isExecuted)
        {
            keyTriggerObject.SetActive(true);
        }
        else
        {
            keyTriggerObject.SetActive(false);
        }
    }
}
