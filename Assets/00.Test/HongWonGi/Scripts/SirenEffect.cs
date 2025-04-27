using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;
using DG.Tweening;

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
    [SerializeField] private Light _sirenLight;

    [Tooltip("최저 intensity (여기서는 1)")] 
    [SerializeField] private float minIntensity;//최소 빛 밝기
    [SerializeField] private float maxIntensity;//불 밝기 
    [SerializeField] private float upDuration;//깜빡거림 시간
    Tween _sirenTween;
    
    private void Awake()
    {
        _mainCamera = GetComponent<Camera>();
        _mainCamera = Camera.main;
        _originalPos = _mainCamera.transform.localPosition;
        _originalRot = _mainCamera.transform.localRotation;
        _seed = Random.value * 100f;
    }

    private void OnEnable()
    {
        ShakeCamera(_shakePower, _shakeDuration).Forget();
        StartSiren();
    }
    private void StartSiren()
    {
        _sirenLight.intensity = minIntensity;
        _sirenTween = _sirenLight
            .DOIntensity(maxIntensity, upDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);
    }
    public void StopBlink()
    {
        _sirenTween.Kill();
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