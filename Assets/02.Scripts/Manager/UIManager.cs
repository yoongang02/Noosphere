using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIManager : Singleton<UIManager>
{
    //여러 UI 창을 관리하기 위해 스택 이용
    private Stack<UIBase> uiStack = new Stack<UIBase>();

    public UIBase investigateUI;
    public UIBase evidenceDetailUI;
    /*
    public GameObject _showPressBtnUI;
    public GameObject _bookInfo;
    public GameObject _bookPopUp;
    public bool isPopUpOpen = false;
    public Animator transitionAnimator;
    public GameObject _endingMessage;
    */
    
    public TextMeshProUGUI dialogueUI;
    public TextMeshProUGUI popUI;
    
    public bool isInMap = true;
    
    public Action OnSelectEnd;
    
    private void Update()
    {
        // ESC 버튼 입력 처리
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTopUI();
        }
    }
    
    public void OpenUI(UIBase ui)
    {
        if (ui == null) return;

        // 스택에 추가하고 UI를 활성화
        uiStack.Push(ui);
        ui.OnOpen();
    }

    public void OpenUI(UIBase ui, EvidenceStructure evidence)
    {
        if (ui == null) return;

        // 스택에 추가하고 UI를 활성화
        uiStack.Push(ui);
        ui.OnOpen(evidence);
    }
    
    public void CloseTopUI()
    {
        if (uiStack.Count == 0) return;

        UIBase topUI = uiStack.Pop();
        topUI.OnClose();
    }
    
    public void CloseAllUI()
    {
        while (uiStack.Count > 0)
        {
            CloseTopUI();
        }
    }
    
    public UIBase GetTopUI()
    {
        if (uiStack.Count == 0) return null;
        return uiStack.Peek();
    }
    
    public bool IsAnyUIOpen()
    {
        return uiStack.Count > 0;
    }
    
    public bool IsUIOpen(UIBase ui)
    {
        return uiStack.Contains(ui);
    }
}
