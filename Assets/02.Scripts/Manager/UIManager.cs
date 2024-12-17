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
    public UIBase topUI;
    
    public UIBase investigateUI;
    public UIBase evidenceDetailUI;
    public UIBase inventoryUI;
    public UIBase dialogueUI;
    
    public GameObject inventoryIcon;
    
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
            if(IsUIOpen(dialogueUI)) return;
            CloseTopUI();
        }

        if (IsAnyUIOpen())
        {
            inventoryIcon.SetActive(false);
        }
        else
        {
            inventoryIcon.SetActive(true);
        }
    }
    
    public void OpenUI(UIBase ui)
    {
        if (ui == null) return;

        // 스택에 추가하고 UI를 활성화
        // 상호작용 금지
        LockPlayer();
        uiStack.Push(ui);
        topUI = ui;
        ui.OnOpen();
    }

    public void OpenUI(UIBase ui, EvidenceStructure evidence)
    {
        if (ui == null) return;
        
        if (evidence == null)
        {
            Debug.LogError("🔥 evidence가 null이므로 OpenUI()를 호출할 수 없습니다.");
            return;
        }
        // 스택에 추가하고 UI를 활성화
        // 상호작용 금지
        LockPlayer();
        uiStack.Push(ui);
        topUI = ui;
        ui.OnOpen(evidence);
    }
    
    public void CloseTopUI()
    {
        if (uiStack.Count == 0) return;

        UIBase topUI = uiStack.Pop();
        
        topUI.OnClose();

        if (uiStack.Count > 0)
        {
            this.topUI = uiStack.Peek();
        }
        else
        {
            this.topUI = null;
        }

        if (!IsAnyUIOpen())
        {
            //상호작용 금지 해제
            UnLockPlayer();
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

    public bool IsAcquiredInInvestigateUI()
    {
        return investigateUI.GetComponent<InvestigateUI>().isAquired;
    }
    
    public void LockPlayer()
    {
        Debug.Log("UI 열 때 LockPlayer 실행되나?");
        PlayerController.Instance.canMove = false;
        PlayerInteract.Instance.canInteract = false;
    }

    public void UnLockPlayer()
    {
        Debug.Log("UI 모두 닫히면 UnLockPlayer 실행되나?");
        PlayerController.Instance.canMove = true;
        PlayerInteract.Instance.canInteract = true;
    }
}
