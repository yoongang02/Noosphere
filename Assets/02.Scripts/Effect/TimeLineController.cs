using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimeLineController : MonoBehaviour
{
    [SerializeField] private PlayableDirector _playableDirector;
    [SerializeField] private GameObject _playerParent;
    private GameObject _player;
    [SerializeField] private float _angle;

    private void Awake()
    {
        _player = FindObjectOfType<PlayerController>().gameObject;
        _playableDirector.stopped += EndTimeLine;
    }


    private void OnEnable()
    {
        if (_playerParent != null)
        {
            _player.transform.SetParent(_playerParent.transform);
            Vector3 newPosition = _player.transform.position;
            newPosition.x = 0f;
            newPosition.z = 0f;
            _player.transform.localPosition = newPosition;
            _player.transform.localRotation = Quaternion.identity;
            _player.transform.localRotation = Quaternion.Euler(0, _angle, 0);
        }

        InventoryManager.Instance.canOpenInventory = true;
        UIManager.Instance.LockPlayer();
        UIManager.Instance.inventoryIcon.SetActive(false);
        _playableDirector.Play();
    }

    private void EndTimeLine(PlayableDirector obj)
    {
        Debug.Log("타임라인 끝");

        if (_playerParent != null)
        {
            _player.transform.SetParent(null);
        }
        
        EffectManager.Instance.OnEffectEnd?.Invoke();
        UIManager.Instance.UnLockPlayer();
        UIManager.Instance.inventoryIcon.SetActive(true);
        InventoryManager.Instance.canOpenInventory = false;
    }
}