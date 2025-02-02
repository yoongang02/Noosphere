using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenEffect : MonoBehaviour
{
   [SerializeField] private GameObject _closeDoor;
   [SerializeField] private GameObject _clearTrigger;
   [SerializeField] private GameObject _block;

   private void OnEnable()
   {
      _closeDoor.SetActive(false);
      _clearTrigger.SetActive(true);
      SoundManager.Instance.PlaySFX("Soundresource_082");
   }

   public void OnAnimationEnd()
   {
      _block.SetActive(false);
      EffectManager.Instance.OnEffectEnd?.Invoke();
   }
}
