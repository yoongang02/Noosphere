using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveCompleteUI : DefaultUIBase
{
    [SerializeField] private float _waitTime;
    [SerializeField] private GameObject _savingUI;
    [SerializeField] private GameObject _completeUI;
    [SerializeField] private GameObject _firstSelectable;
    public override void OnOpen()
    {
        base.OnOpen();
        StartCoroutine(DoSave());
    }

    public override void OnClose()
    {
        base.OnClose();
        _savingUI.SetActive(false);
        _completeUI.SetActive(false);
    }

    public void ClickYesBtn()
    {
        FindAnyObjectByType<SaveUI>().UpdateSaveUI();
        DefaultUIController.Instance.CloseTopUI();
    }

    IEnumerator DoSave()
    {
        _savingUI.SetActive(true);
        StartCoroutine(NooSphere.SaveManager.Instance.DoSave());
        yield return new WaitForSecondsRealtime(_waitTime);
        _savingUI.SetActive(false);
        _completeUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null); // 먼저 비우고
        EventSystem.current.SetSelectedGameObject(_firstSelectable); // 새로 지정
    }
}
