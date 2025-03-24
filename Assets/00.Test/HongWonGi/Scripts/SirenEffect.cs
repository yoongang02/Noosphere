using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class SirenEffect : MonoBehaviour
{
    [Header("카메라 흔들림 설정")] 
    [SerializeField] [Range(0f, 1f)] private float _maxOffset = 0.5f; // 최대 위치 오프셋
    [SerializeField] [Range(0f, 360f)] private float _maxAngle = 10f; // 최대 회전 각도
    [SerializeField] [Range(0f, 1f)] private float _shakePower = 0.6f; // 기본 트라우마 강도 (0-1)
    [SerializeField] [Range(0f, 10f)] private float _shakeDuration = 10f; // 기본 지속 시간(초)

    private Camera _mainCamera;
    private Vector3 _originalPos;
    private Quaternion _originalRot;
    private float _seed;
    [Header("사이렌 효과")]
    [SerializeField]private Volume _volume;
    [SerializeField] [Range(0f, 1f)] private float _minVignetteIntensity = 0.2f; // Vignette 최소 강도
    [SerializeField] [Range(0f, 1f)] private float _maxVignetteIntensity = 0.8f; // Vignette 최대 강도
    [SerializeField] [Range(0.1f, 5f)] private float _vignettePulseSpeed = 1f;
    private Vignette _vignette;
    private float _originalVignetteIntensity;
    private bool _isVignetteEffectRunning = false;
    
    private void Awake()
    {
        if (!_volume.profile.TryGet<Vignette>(out _vignette))
        {
            Debug.Log("volume효과 존재 x");
        }
        else
        {
            Debug.Log("volume효과 존재");
        }
        _mainCamera = GetComponent<Camera>();
        _mainCamera = Camera.main;
        _originalPos = _mainCamera.transform.localPosition;
        _originalRot = _mainCamera.transform.localRotation;
        _seed = Random.value * 100f;
    }

    private void OnEnable()
    {
        ShakeCamera(_shakePower, _shakeDuration).Forget();
        StartVignetteEffect().Forget();
    }
    private async UniTaskVoid StartVignetteEffect()
    {
        if (_vignette == null) return;
        
        _isVignetteEffectRunning = true;
        float time = 0f;
        
        while (_isVignetteEffectRunning)
        {
            // Sin 함수를 사용하여 -1에서 1 사이의 값 생성 (주기적인 움직임)
            float pulse = Mathf.Sin(time * _vignettePulseSpeed * Mathf.PI);
            
            // -1~1 값을 0~1 범위로 변환
            float normalizedPulse = (pulse + 1f) * 0.5f;
            
            // 최소값과 최대값 사이에서 보간
            float intensity = Mathf.Lerp(_minVignetteIntensity, _maxVignetteIntensity, normalizedPulse);
            
            // Vignette intensity 설정
            _vignette.intensity.value = intensity;
            
            // 시간 업데이트
            time += Time.deltaTime;
            
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        
        // 원래 값으로 복원
        if (_vignette != null)
        {
            _vignette.intensity.value = _originalVignetteIntensity;
        }
    }
    
    // Vignette 효과 중지
    private void StopVignetteEffect()
    {
        _isVignetteEffectRunning = false;
    }
    
    public async UniTaskVoid ShakeCamera(float power, float duration)
    {
        float curPower = Mathf.Clamp01(power);
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float shake = curPower * curPower;
            
            float normalizedTime = elapsed / duration;
            float currentIntensity = Mathf.Lerp(power, 0f, normalizedTime);
            shake = currentIntensity * currentIntensity;
            
            float offsetX = _maxOffset * shake * (Mathf.PerlinNoise(_seed, Time.time * 10f) * 2f - 1f);
            float offsetY = _maxOffset * shake * (Mathf.PerlinNoise(_seed + 1f, Time.time * 10f) * 2f - 1f);
            float rotation = _maxAngle * shake * (Mathf.PerlinNoise(_seed + 2f, Time.time * 10f) * 2f - 1f);
            
            _mainCamera.transform.localPosition = _originalPos + new Vector3(offsetX, offsetY, 0f);
            _mainCamera.transform.localRotation = _originalRot * Quaternion.Euler(0f, 0f, rotation);
            
            elapsed += Time.deltaTime;
            
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        
        ResetCamera();
    }
    private void ResetCamera()
    {
        Debug.Log("ㅇㅇ");
        _mainCamera.transform.localPosition = _originalPos;
        _mainCamera.transform.localRotation = _originalRot;
    }
}