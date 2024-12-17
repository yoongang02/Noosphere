using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokeMirrorEffect : MonoBehaviour
{
    [SerializeField] private GameObject _brokeMirror;
    [SerializeField] private GameObject _mirror;
    [SerializeField] private List<GameObject> _pointEffects;

    private void OnEnable()
    {
        MirrorPuzzleManager.Instance.isMirrorBroke = true;
        foreach(GameObject obj in _pointEffects)
        {
            obj.SetActive(true);
        }
        _mirror.SetActive(false);
        _brokeMirror.SetActive(true);
    }
}
