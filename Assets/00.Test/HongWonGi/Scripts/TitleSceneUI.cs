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
        SoundManager.Instance.PlaySound(Define.Soundresource_037,1);
        
    }

    private void OnClickNewGameBtn()
    {
        SoundManager.Instance.PlaySound(Define.Soundresource_037,1);
        SceneManager.LoadScene("LoadingScene");
    }

    private void OnClickCreditBtn()
    {
        SoundManager.Instance.PlaySound(Define.Soundresource_037,1);
    }

    private void OnClickSettingBtn()
    {
        SoundManager.Instance.PlaySound(Define.Soundresource_037,1);
    }

    private void OnClickExitBtn()
    {
        SoundManager.Instance.PlaySound(Define.Soundresource_037,1);
        Application.Quit();
    }
}
