using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveToPoint : MonoBehaviour
{
    [SerializeField] private Transform _point;
    public bool isTest;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isTest/*DataManager.Instance._events["Event_C082"].isExecuted*/)
        {
            other.transform.DOMove(_point.position, 1f);
        }
    }
}
