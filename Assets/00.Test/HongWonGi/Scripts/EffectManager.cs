using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;


public class EffectManager : Singleton<EffectManager>
{
    private string _currentEffectID;
    public bool isEffectEnd = false;
    //
    //SetEffect불러오기
    //
    public void SetEffect(string id)
    {
        _currentEffectID = id;
        // isEffectEnd = false;
        if (DataManager.Instance._effect.TryGetValue(_currentEffectID, out EffectStructure effect))
        {
            if (!string.IsNullOrEmpty(effect.artresource_id))
            {
                isEffectEnd = true;
                DoEffect(effect.artresource_id);
            }
            if (!string.IsNullOrEmpty(effect.soundresource_id))
            {
                SoundResourceStructure sound = DataManager.Instance._sound[effect.soundresource_id];
                if (sound.soundresource_Type=="Sound")
                {
                    SoundManager.Instance.PlaySound(effect.soundresource_id,sound.loop_count);
                }
                else
                {
                    SoundManager.Instance.PlayBGM(effect.soundresource_id);
                }
            }
        }
        else
        {
            Debug.LogWarning($" {_currentEffectID} 못찾음");
        }
    }

    private void DoEffect(string effectObjectName)
    {
        GameObject effectObj = GameObject.Find(effectObjectName);
        effectObj.transform.GetChild(0).gameObject.SetActive(true);
    }
}
