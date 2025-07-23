using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private GameObject _creditUI;
    [Header("세팅 버튼")] [SerializeField] private Button _windowBtn;
    [SerializeField] private Button _fullScreenBtn;
    [SerializeField] private Button _exitBtn;
    [SerializeField] private Button _creditBtn;
    [SerializeField] private Button _okBtn;
    [SerializeField] private Slider _soundVolumeSlider;
    [SerializeField] private Slider _bgmVolumeSlider;
    [SerializeField] private Toggle _fullScreenToggle;
    [SerializeField]private Toggle _windowToggle;
    [Header("텍스트")] [SerializeField] private TextMeshProUGUI _soundText;
    [SerializeField] private TextMeshProUGUI _bgmText;

    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        _soundVolumeSlider.onValueChanged.AddListener(SoundVolumeChanged);
        _bgmVolumeSlider.onValueChanged.AddListener(BgmVolumeChanged);
        _okBtn.onClick.AddListener(OnClickOkBtn);
        _exitBtn.onClick.AddListener(OnClickExitBtn);
        _creditBtn.onClick.AddListener(() => { _creditUI.SetActive(true); });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnClickExitBtn();
        }
    }

    private void OnEnable()
    {
        _okBtn.interactable = false;
        _bgmVolumeSlider.value = SoundManager.Instance.bgmVolume;
        _soundVolumeSlider.value = SoundManager.Instance.sfxVolume;
        
    }

    private void OnClickOkBtn()
    {
        float bgmVolume = _bgmVolumeSlider.value;
        float soundVolume = _soundVolumeSlider.value;
        ES3.Save("BgmVolume", bgmVolume, "Setting.es3");
        ES3.Save("SoundVolume", soundVolume, "Setting.es3");
        ES3.Save("ScreenMode", _fullScreenToggle.isOn, "Setting.bgmVolume");
        gameObject.SetActive(false);
    }

    public void OnClickFullScreenBtn()
    {
        Screen.SetResolution(1920,1080,true);
    }
    public void OnClickWindowBtn()
    {
        Screen.SetResolution(1280, 720, false);
    }
    private void OnClickExitBtn()
    {
        if (ES3.KeyExists("SoundVolume","Setting.es3") || ES3.KeyExists("BgmVolume","Setting.es3"))
        {
            float BgmVolume = ES3.Load<float>("BgmVolume","Setting.es3");
            float SoundVolume = ES3.Load<float>("SoundVolume", "Setting.es3");
            SoundManager.Instance.bgmVolume = BgmVolume;
            SoundManager.Instance._bgmSource.volume = BgmVolume;
            SoundManager.Instance.sfxVolume = SoundVolume;
        }
        else
        {
            SoundManager.Instance._bgmSource.volume = 0.5f;
            SoundManager.Instance.bgmVolume = 0.5f;
            SoundManager.Instance.sfxVolume = 0.5f;
        }
        gameObject.SetActive(false);
    }

    private void SoundVolumeChanged(float value)
    {
        SoundManager.Instance.sfxVolume = value;
        int intValue = Mathf.RoundToInt(value * 100);
        _soundText.text = $"{intValue}%";
        _okBtn.interactable = true;
    }
    
    private void BgmVolumeChanged(float value)
    {
        SoundManager.Instance.bgmVolume = value;
        SoundManager.Instance._bgmSource.volume = value;
        int intValue = Mathf.RoundToInt(value * 100);
        _bgmText.text = $"{intValue}%";
        _okBtn.interactable = true;
    }

    //슬라이더에 있는 eventtrigger에 들어갈 이벤트 함수
    public void OnSoundVolumePointerUp(BaseEventData data)
    {
        SoundManager.Instance.PlaySFXNoEffect("Soundresource_037");
    }
}