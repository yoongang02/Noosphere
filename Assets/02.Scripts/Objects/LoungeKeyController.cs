using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoungeKeyController : MonoBehaviour
{
    [SerializeField] private GameObject keyTriggerObject;
    [SerializeField] private List<GameObject> roomTriggerObjects = new List<GameObject>();

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

        if (DataManager.Instance._events["Event_C063"].isExecuted)
        {
            foreach (var room in roomTriggerObjects)
            {
                room.SetActive(false);
            }
        }
        else
        {
            foreach (var room in roomTriggerObjects)
            {
                room.SetActive(true);
            }
        }
    }
}
