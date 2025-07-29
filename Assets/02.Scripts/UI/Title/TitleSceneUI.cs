using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
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
    private bool _hasFirstHoverOccurred = false;
    private void OnEnable()
    {
        _eventSys.firstSelectedGameObject = _newGameBtn.gameObject;
        _hasFirstHoverOccurred = false; 
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
        SoundManager.Instance.PlaySFX("Soundresource_037");
        DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.resumeUI);
    }

    // TODO : 세이브데이터가 있는 경우와 없는 경우를 구분해서 기획에 따라 다르게 구현해야 함.
    private void OnClickNewGameBtn()
    {
        SoundManager.Instance.PlaySFX("Soundresource_037");
        NooSphere.SaveManager.Instance.SetLoadType(NooSphere.GameLoadType.NewGame);
        SceneManager.LoadScene("LoadingScene");
    }

    private void OnClickCreditBtn()
    {
        SoundManager.Instance.PlaySFX("Soundresource_037");
        DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.creditUI);
    }

    private void OnClickSettingBtn()
    {
        SoundManager.Instance.PlaySFX("Soundresource_037");
        _eventSys.sendNavigationEvents = false;
        DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.settingUI);
    }

    private void OnClickExitBtn()
    {
        SoundManager.Instance.PlaySFX("Soundresource_037");
        Application.Quit();
    }

    public void OnClickSound()
    {
        SoundManager.Instance.PlaySFX("Soundresource_037");
    }

    public void OnHoverSound()
    {
        SoundManager.Instance.PlaySFX("Soundresource_035");
    }

    public void OnFirstHover()
    {
        if (!_hasFirstHoverOccurred)
        {
            _eventSys.SetSelectedGameObject(null);
            _hasFirstHoverOccurred = false;
        }

    }
}
