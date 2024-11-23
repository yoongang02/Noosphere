using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgmstart : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayBGM(Define.porlogueBGM);
    }
}
