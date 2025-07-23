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
    [SerializeField] private GameObject _creditPanel;
    [SerializeField] private GameObject _settingPanel;
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
        
    }

    private void OnClickNewGameBtn()
    {
        SoundManager.Instance.PlaySFX("Soundresource_037");
        SceneManager.LoadScene("LoadingScene");
    }

    private void OnClickCreditBtn()
    {
        SoundManager.Instance.PlaySFX("Soundresource_037");
        _creditPanel.SetActive(true);
    }

    private void OnClickSettingBtn()
    {
        SoundManager.Instance.PlaySFX("Soundresource_037");
        _settingPanel.SetActive(true);
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
