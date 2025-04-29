using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;
using DG.Tweening;
using Cinemachine;
public class SirenEffect : MonoBehaviour
{
    [Header("사이렌 효과")] 
    [SerializeField] private Light _sirenLight;

    [Tooltip("최저 intensity (여기서는 1)")] 
    [SerializeField] private float minIntensity;//최소 빛 밝기
    [SerializeField] private float maxIntensity;//불 밝기 
    [SerializeField] private float upDuration;//깜빡거림 시간
    Tween _sirenTween;
    [SerializeField] private CinemachineVirtualCamera _vcam;
    private CinemachineBasicMultiChannelPerlin _perlin;
    private void Awake()
    {
        _perlin = _vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void OnEnable()
    {
        StartSiren();
        DOTween.To(
                () => _perlin.m_FrequencyGain,
                x => _perlin.m_FrequencyGain = x,
                1f,
                1f 
            )
            .SetEase(Ease.Linear)
            .SetLoops(2, LoopType.Yoyo);
        
        SoundManager.Instance.PlaySFXNoEffect("Soundresource_099");
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
    
 
}