using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class VolumeByDistance : MonoBehaviour
{
    [Header("시작점과 도착점 설정")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("대상 (플레이어 등)")]
    public Transform target;

    [Header("조절할 Volume 컴포넌트")]
    public Volume volume;
    
    [Header("조절할 Light 컴포넌트")]
    public Light targetLight;
    [Header("Light Intensity 설정")]
    [Range(0f, 1f)]
    public float maxIntensity = 0.8f;
    public float minIntensity = 0f;
    void Update()
    {
        // 시작점과 도착점 사이의 전체 거리
        float totalDistance = Vector3.Distance(startPoint.position, endPoint.position);
        if(totalDistance == 0f) return;

        // 대상이 시작점으로부터 떨어진 거리
        float currentDistance = Vector3.Distance(startPoint.position, target.position);

        // 거리에 따른 비율을 0~1 사이로 계산
        float t = Mathf.Clamp01(currentDistance / totalDistance);

        // volume의 weight를 t값으로 조절 (0이면 효과 없음, 1이면 최대 효과)
        volume.weight = t;
        float reversedT = 1f - t;
        targetLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, reversedT);
    }
}
