using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLineSignalManager : MonoBehaviour
{
    private Animator _player;
    [SerializeField] private Animator _npc;
    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Animator>();
    }

    public void StartPlayerAnim(string playerAnim)
   {
       _player.SetBool(playerAnim,true);
   }

   public void EndPlayerAnim(string playerAnim)
   {
       _player.SetBool(playerAnim,false);
   }

   public void StartNpcAnim(string npcAnim)
   {
       _npc.SetBool(npcAnim,true);
   }

   public void EndNpcAnim(string npcAnim)
   {
       _npc.SetBool(npcAnim,false);
   }
}
