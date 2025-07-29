using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class InGameOptionUI : DefaultUIBase
{
    [SerializeField] private GameObject firstSelectable;
    public override void OnOpen()
    {
        base.OnOpen();

        // 게임 일시 정지
        Time.timeScale = 0f;

        EventSystem.current.firstSelectedGameObject = firstSelectable;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();

        // 게임 일시 정지 해제
        Time.timeScale = 1f;

        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();
    }
    public void OnClickResume()
    {
        DefaultUIController.Instance.CloseAllUI();
    }

    public void OnClickSave()
    {

    }

    public void OnClickOption()
    {
        DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.settingUI);
    }

    public void OnClickStartScene()
    {
        DefaultUIController.Instance.CloseTopUI();
        SceneChanger.Instance.ChangeScene("StartScene").Forget();
    }
}
