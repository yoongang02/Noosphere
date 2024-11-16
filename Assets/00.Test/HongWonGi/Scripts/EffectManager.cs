using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;


public class EffectManager : Singleton<EffectManager>
{
    public Dictionary<string, EffectStructure> _effect = new Dictionary<string, EffectStructure>();
    public Dictionary<string, SoundResourceStructure> _sound = new Dictionary<string, SoundResourceStructure>();
    private string _currentEffectID;
    public bool isEffectEnd = false;
    private void Start()
    {
        Initialize().Forget();
    }
    private async UniTaskVoid Initialize()
    {
        _effect = await LoadData<EffectStructure>("Effect");
        _sound = await LoadData<SoundResourceStructure>("SoundResource");
        Debug.Log("이펙트 완료");
        Debug.Log("사운드 완료");
    }

    public async UniTask<Dictionary<string, T>> LoadData<T>(string fileName) where T : new()
    {
        CSVParserYKM parser = new CSVParserYKM();
        return await parser.Parse<T>(fileName);
    }
    //
    //SetEffect불러오기
    //
    public void SetEffect(string id)
    {
        _currentEffectID = id;
        isEffectEnd = false;
        if (_effect.TryGetValue(_currentEffectID, out EffectStructure effect))
        {
            if (!string.IsNullOrEmpty(effect.artresource_id))
            {
                DoEffect(effect.artresource_id);
            }
            if (!string.IsNullOrEmpty(effect.soundresource_id))
            {
                SoundResourceStructure sound = _sound[effect.soundresource_id];
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
