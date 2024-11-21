using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


public class EffectManager : Singleton<EffectManager>
{
    private string _currentEffectID;
    public bool isEffectEnd = false;

    [SerializeField] private Camera _mainCamera;
    [SerializeField] private UnityEngine.Rendering.Universal.UniversalAdditionalCameraData cameraData;
    [SerializeField] private VideoPlayer vhsEffect;
    [SerializeField] private Volume vhsVolume;
    
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
        // 메인 카메라 찾기
        _mainCamera = Camera.main;
        if(_mainCamera != null)
        {
            // 카메라 데이터 할당
            cameraData = _mainCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
            // VideoPlayer 카메라 할당
            if(vhsEffect != null)
            {
                vhsEffect.targetCamera = _mainCamera;
            }
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

    public void StartMentalEffect(float value)
    {
        cameraData.renderPostProcessing = true;
   
        // VHS 이펙트의 알파값 조절 (0 ~ 1)
        var vhsColor = vhsEffect.targetCameraAlpha;
        vhsEffect.targetCameraAlpha = value;

        // Volume weight 조절 (0 ~ 1)
        vhsVolume.weight = value;
        if (value >= 1f)
        {
            EndMetnalEffect();
        }

    }

    public void EndMetnalEffect()
    {
        var vhsColor = vhsEffect.targetCameraAlpha;
        vhsEffect.targetCameraAlpha = 0;
        vhsVolume.weight = 0;
        cameraData.renderPostProcessing = false;
    }
}
