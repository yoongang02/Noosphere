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
   private void Awake()
   {
      _player = GameObject.FindObjectOfType<PlayerController>().gameObject;
   }
   private void OnEnable()
   {
      _player.transform.SetParent(_playerParent.transform);
      _playableDirector.Play();
   }

   private void Start()
   {
      _playableDirector.stopped += EndTimeLine;
   }
   private void EndTimeLine(PlayableDirector obj)
   {
      EffectManager.Instance.OnEffectEnd?.Invoke();
      _player.transform.SetParent(null);
   }
}
