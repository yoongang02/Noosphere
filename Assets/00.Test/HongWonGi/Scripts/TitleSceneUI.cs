using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneUI : MonoBehaviour
{
    [SerializeField] private Button _loadGameBtn;
    [SerializeField] private Button _newGameBtn;
    [SerializeField] private Button _creditBtn;
    [SerializeField] private Button _settingBtn;
    [SerializeField] private Button _exitBtn;
    public EventSystem _eventSys;
    [SerializeField] private AudioSource _sfxSource;

    private void OnEnable()
    {
        _eventSys.firstSelectedGameObject = _loadGameBtn.gameObject;
    }

    private void Start()
    {
        _loadGameBtn.onClick.AddListener(OnClickLoadBtn);
        _newGameBtn.onClick.AddListener(OnClickNewGameBtn);
        _creditBtn.onClick.AddListener(OnClickCreditBtn);
        _settingBtn.onClick.AddListener(OnClickSettingBtn);
        _exitBtn.onClick.AddListener(OnClickExitBtn);
    }

    private void OnClickLoadBtn()
    {
        _sfxSource.Play();
        
    }

    private void OnClickNewGameBtn()
    {
        _sfxSource.Play();
        SceneManager.LoadScene("LoadingScene");
    }

    private void OnClickCreditBtn()
    {
        _sfxSource.Play();
    }

    private void OnClickSettingBtn()
    {
        _sfxSource.Play();
    }

    private void OnClickExitBtn()
    {
        _sfxSource.Play();
        Application.Quit();
    }
}
