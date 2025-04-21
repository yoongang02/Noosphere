using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgmstart : MonoBehaviour
{
    [SerializeField] private string bgmName;
    private void OnEnable()
    {
        SoundManager.Instance.StopForceBGM();
        SoundManager.Instance.PlayBGM(bgmName);
    }
}
