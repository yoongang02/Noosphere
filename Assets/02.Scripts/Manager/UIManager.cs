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
    public UIBase inventoryUI;
    
    public TextMeshProUGUI dialogueUI;
    
    public bool isInMap = true;
    
    public Action OnSelectEnd;

    void Start()
    {
        
    }
    
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
        // 상호작용 금지
        uiStack.Push(ui);
        ui.OnOpen();
    }

    public void OpenUI(UIBase ui, EvidenceStructure evidence)
    {
        if (ui == null) return;

        // 스택에 추가하고 UI를 활성화
        // 상호작용 금지
        uiStack.Push(ui);
        ui.OnOpen(evidence);
    }
    
    public void CloseTopUI()
    {
        if (uiStack.Count == 0) return;

        UIBase topUI = uiStack.Pop();
        topUI.OnClose();

        if (!IsAnyUIOpen())
        {
            //상호작용 금지 해제
        }
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
