using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokeMirrorEffect : MonoBehaviour
{
    [SerializeField] private GameObject _brokeMirror;
    [SerializeField] private GameObject _mirror;

    private void OnEnable()
    {
        MirrorPuzzleManager.Instance.isMirrorBroke = true;
        _mirror.SetActive(false);
        _brokeMirror.SetActive(true);
    }
}
