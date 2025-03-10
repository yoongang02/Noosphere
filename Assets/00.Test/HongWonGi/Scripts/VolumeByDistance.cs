using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeByDistance : MonoBehaviour
{
    [Header("시작점과 도착점 설정")] 
    [SerializeField]
    private Transform _startPoint;
    [SerializeField] 
    private Transform _endPoint;
    [SerializeField]
    private Transform _playerTrans;

    [SerializeField]
    private Volume _volume;
    // [SerializeField]
    // private Light targetLight;

    // [Header("Light Intensity 설정")] [Range(0f, 1f)]
    // public float maxIntensity = 0.8f;
    //
    // public float minIntensity = 0f;

    private Vignette _vignette;

    private void Start()
    {
        if (_volume != null && _volume.profile != null)
        {
            if (!_volume.profile.TryGet<Vignette>(out _vignette))
            {
                Debug.LogWarning("Volume Profile에 Vignette 효과가 없습니다.");
            }
        }
    }

    void Update()
    {
        // 시작점과 도착점 사이의 전체 거리
        float totalDistance = Vector3.Distance(_startPoint.position, _endPoint.position);
        if (totalDistance == 0f) return;

        // 대상이 시작점으로부터 떨어진 거리
        float currentDistance = Vector3.Distance(_startPoint.position, _playerTrans.position);

        // 거리에 따른 비율을 0~1 사이로 계산
        float t = Mathf.Clamp01(currentDistance / totalDistance);

        // volume의 weight를 t값으로 조절 (0이면 효과 없음, 1이면 최대 효과)
        _volume.weight = t;
        float reversedT = 1f - t;
        // targetLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, reversedT);
        if (_vignette != null)
        {
            Color originalColor = _vignette.color.value;
            float h, s, v;
            Color.RGBToHSV(originalColor, out h, out s, out v);
            // t가 0이면 V=1, t가 1이면 V=0 (즉, 100%에서 0%)
            float newV = Mathf.Lerp(1f, 0f, t);
            Color newColor = Color.HSVToRGB(h, s, newV);
            newColor.a = originalColor.a; // 기존 알파값 유지
            _vignette.color.value = newColor;
        }
    }
}