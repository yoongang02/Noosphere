using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;


public class EffectManager : Singleton<EffectManager>
{
    private string _currentEffectID;
    public bool isEffectEnd = false;
    [SerializeField] private RawImage _vhsImage;
    [SerializeField] private Volume _vhsVolume;
    public GameObject vhsObj;
    private Camera _mainCamera;
    private UnityEngine.Rendering.Universal.UniversalAdditionalCameraData _cameraData;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _mainCamera = Camera.main;
        if (_mainCamera != null)
        {
            _cameraData = _mainCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
        }
    }

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
                if (sound.soundresource_Type == "Sound")
                {
                    SoundManager.Instance.PlaySound(effect.soundresource_id, sound.loop_count);
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

    public void StartMentalEffect(float value)
    {
        vhsObj.SetActive(true);
        _cameraData.renderPostProcessing = true;

        Color color = _vhsImage.color;
        color.a = value;
        _vhsImage.color = color;
        _vhsVolume.weight = value;
        if (value >= 1f)
        {
            ResetMetalEffect();
        }
    }

    public void ResetMetalEffect()
    {
        vhsObj.SetActive(false);
        
        Color color = _vhsImage.color;
        color.a = 0f;
        _vhsImage.color = color;
        _vhsVolume.weight = 0;
        _cameraData.renderPostProcessing = false;
    }
}