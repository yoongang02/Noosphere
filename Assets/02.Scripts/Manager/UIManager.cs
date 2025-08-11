using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    //여러 UI 창을 관리하기 위해 스택 이용
    private Stack<UIBase> uiStack = new Stack<UIBase>();
    public UIBase topUI;
    
    public UIBase investigateUI;
    public UIBase evidenceDetailUI;
    public UIBase inventoryUI;
    public UIBase dialogueUI;
    public UIBase inputFieldUI;
    public UIBase mirrorDialogueUI;
    public GameObject cctvFrame;
    public GameObject keyGuideUI;
    
    public GameObject inventoryIcon;
    
    public bool isInMap = true;
    
    public Action OnSelectEnd;
    public bool isYesClicked = false;
    private void Update()
    {
        // ESC 버튼 입력 처리
        if (InputRouter.Instance.ConsumeEscape())
        {
            if (IsUIOpen(dialogueUI) || IsUIOpen(investigateUI) || IsUIOpen(mirrorDialogueUI))
            {
                return;
            }

            if (FindObjectOfType<MirrorDialogueManager>() != null)
            {
                if(FindObjectOfType<MirrorDialogueManager>().IsTopUI()) return;
            }
            CloseTopUI();
        }
        
        if (FindObjectOfType<MirrorDialogueManager>() != null)
        {
            if (FindObjectOfType<MirrorDialogueManager>().IsTopUI())
            {
                cctvFrame.SetActive(true);
            }
        }


        if (DataManager.Instance._events.ContainsKey("Event_A031"))
        {
            if (DataManager.Instance._events["Event_A031"].isExecuted)
            {
                keyGuideUI.SetActive(false);
            }
            else
            {
                keyGuideUI.SetActive(true);
            }
        }

        if (dialogueUI != null && dialogueUI.IsTopUI())
        {
            // 책장에서 거울 조각 습득 시 cctv frame
            if (EventManagerYKM.Instance.currentEventID == "Event_B044" &&
                DialogueManager.Instance.GetCurDialogueId() == "Dialogue_0065")
            {
                cctvFrame.SetActive(true);
            }
            
            if (EventManagerYKM.Instance.currentEventID == "Event_B065" &&
                DialogueManager.Instance.GetCurDialogueId() == "Dialogue_0031")
            {
                cctvFrame.SetActive(true);
            }
            
            if (EventManagerYKM.Instance.currentEventID == "Event_B066" &&
                DialogueManager.Instance.GetCurDialogueId() == "Dialogue_0032")
            {
                cctvFrame.SetActive(true);
            }
            
        }
        
        if (IsAnyUIOpen())
        {
            //인벤토리 아이콘 비활성화
            inventoryIcon.SetActive(false);
            //플레이어 Lock
            LockPlayer();
        }
        else
        {

            if (PlayerInteract.Instance.GetComponent<MentalEnterProcess>().IsEnterNow() || InventoryManager.Instance.canOpenInventory)
            {
                inventoryIcon.SetActive(false);
                keyGuideUI.SetActive(false);
                return;
            }
            
            

            if (FindObjectOfType<HintImage>() == null)
            {
                //인벤토리 아이콘 활성화
                inventoryIcon.SetActive(true);
            }
        }
    }
    
    public void OpenUI(UIBase ui)
    {
        if (ui == null) return;
        if (DefaultUIController.Instance.IsAnyUIOpen()) return;

        // 스택에 추가하고 UI를 활성화
        // 상호작용 금지
        LockPlayer();
        uiStack.Push(ui);
        topUI = ui;
        if (IsUIOpen(dialogueUI) || IsUIOpen(mirrorDialogueUI))
        {
            EscapeUI.Instance.DisActive();
        }
        ui.OnOpen();
    }

    public void OpenUI(UIBase ui, EvidenceStructure evidence)
    {
        if (ui == null) return;
        if (DefaultUIController.Instance.IsAnyUIOpen()) return;

        if (evidence == null)
        {
            return;
        }

        if (ui == investigateUI && evidenceDetailUI.IsTopUI())
        {
            //증거물 상세보기가 열려있는 경우, 조사 UI는 열려도, 위에 보이지 않기 때문에
            CloseTopUI();
        }
        
        // 스택에 추가하고 UI를 활성화
        // 상호작용 금지
        LockPlayer();
        uiStack.Push(ui);
        topUI = ui;
        ui.OnOpen(evidence);
    }

    public void OpenUI(UIBase ui, string quizID)
    {
        if (ui == null) return;
        if (DefaultUIController.Instance.IsAnyUIOpen()) return;

        if (string.IsNullOrEmpty(quizID) || !DataManager.Instance._quiz.ContainsKey(quizID))
        {
            return;
        }
        // 스택에 추가하고 UI를 활성화
        // 상호작용 금지
        LockPlayer();
        uiStack.Push(ui);
        topUI = ui;
        ui.OnOpen(quizID);
        if (quizID == "Quiz_007" && DataManager.Instance._quiz["Quiz_007"].isSolved)
        {
            return;
        }
        if (quizID == "Quiz_004" && DataManager.Instance._quiz["Quiz_004"].isSolved)
        {
            return;
        }
        cctvFrame.SetActive(false);
        if(dialogueUI.IsTopUI()) return;
        if (FindObjectOfType<MirrorDialogueManager>() != null)
        {
            if(FindObjectOfType<MirrorDialogueManager>().IsTopUI()) return;
        }
        EscapeUI.Instance.Active();
    }
    public void CloseTopUI()
    {
        if (uiStack.Count == 0) return;

        UIBase topUI = uiStack.Pop();

        if (topUI != dialogueUI)
        {
            SoundManager.Instance.PlaySFX("Soundresource_036");
        }
        
        topUI.OnClose();
        
        if (uiStack.Count > 0)
        {
            this.topUI = uiStack.Peek();
            LockPlayer();
        }
        else
        {
            this.topUI = null;
            UnLockPlayer();
            EscapeUI.Instance.DisActive();
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
    
    public void LockPlayer()
    {
        PlayerController.Instance.canMove = false;
        PlayerInteract.Instance.canInteract = false;
        //PlayerInteract.Instance.HideInteractionMark();
    }

    public void LockInteraction()
    {
        Debug.LogWarning("LockInteraction 실행");
        PlayerInteract.Instance.canInteract = false;
        //PlayerInteract.Instance.HideInteractionMark();
    }

    public void UnLockPlayer()
    {
        PlayerController.Instance.canMove = true;
        PlayerInteract.Instance.canInteract = true;
    }
}
