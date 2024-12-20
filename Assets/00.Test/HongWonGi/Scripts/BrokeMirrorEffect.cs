using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokeMirrorEffect : MonoBehaviour
{
    [SerializeField] private GameObject _brokeMirror;
    [SerializeField] private GameObject _mirror;
    [SerializeField] private GameObject _pointEffects;
    [SerializeField]private Animator _mirrorAnim;


    private void OnEnable()
    {
        MirrorPuzzleManager.Instance.isMirrorBroke = true;
        _mirrorAnim.SetBool("IsBroke",true);
        _pointEffects.SetActive(true);
        _mirror.SetActive(false);
        _brokeMirror.SetActive(true);
    }
}
