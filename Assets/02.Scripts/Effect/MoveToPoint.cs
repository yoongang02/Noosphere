using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveToPoint : MonoBehaviour
{
    [SerializeField] private Transform _point;
    [SerializeField] private float _walkDuration;
    private Animator _playerAnim;
    private GameObject _player;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerAnim = _player.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        PlayerController.Instance.canMove = false;
        _playerAnim.SetBool("IsWalk", true);
        _player.transform.LookAt(_point.position);
        _player.transform.DOMove(_point.transform.position, _walkDuration).OnComplete(() =>
        {
            PlayerController.Instance.canMove = true;
            _playerAnim.SetBool("IsWalk", false);
            EffectManager.Instance?.OnEffectEnd.Invoke();
            gameObject.SetActive(false);
        });
    }
}