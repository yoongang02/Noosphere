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
            EffectManager.Instance.isEffectEnd = true;
            gameObject.SetActive(false);
        }
    }
}
