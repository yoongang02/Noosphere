using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResetUI : DefaultUIBase
{
    [SerializeField] private GameObject firstSelectable;

    public override void OnOpen()
    {
        base.OnOpen();
        EventSystem.current.SetSelectedGameObject(null); // 먼저 비우고
        EventSystem.current.SetSelectedGameObject(firstSelectable); // 새로 지정
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ClickYesBtn()
    {
        NooSphere.SaveManager.Instance.ResetAllSaveData();
        ClickNoBtn();

        NooSphere.SaveManager.Instance.SetLoadType(NooSphere.GameLoadType.NewGame);
        SceneChanger.Instance.ChangeScene("LoadingScene").Forget();
    }

    public void ClickNoBtn()
    {
        DefaultUIController.Instance.CloseTopUI();
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();

        if (InputRouter.Instance.ConsumeEscape())
        {
            ClickNoBtn();
        }
    }
}
