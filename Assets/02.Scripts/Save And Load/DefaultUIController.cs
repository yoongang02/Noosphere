using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultUIController : Singleton<DefaultUIController>
{
    public DefaultUIBase resumeUI;
    public DefaultUIBase slotDeleteUI;

    //여러 UI 창을 관리하기 위해 스택 이용
    private Stack<DefaultUIBase> uiStack = new Stack<DefaultUIBase>();
    public DefaultUIBase topUI;

    public void OpenUI(DefaultUIBase ui)
    {
        if (ui == null) return;

        uiStack.Push(ui);
        topUI = ui;

        ui.OnOpen();
    }

    public void CloseTopUI()
    {
        if (uiStack.Count == 0) return;

        DefaultUIBase topUI = uiStack.Pop();

        topUI.OnClose();

        if (uiStack.Count > 0)
        {
            this.topUI = uiStack.Peek();
        }
        else
        {
            this.topUI = null;
            //EscapeUI.Instance.DisActive();
        }
    }

    public void CloseAllUI()
    {
        while (uiStack.Count > 0)
        {
            CloseTopUI();
        }
    }

    public DefaultUIBase GetTopUI()
    {
        if (uiStack.Count == 0) return null;
        return uiStack.Peek();
    }

    public bool IsAnyUIOpen()
    {
        return uiStack.Count > 0;
    }

    public bool IsUIOpen(DefaultUIBase ui)
    {
        return uiStack.Contains(ui);
    }
}
