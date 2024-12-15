using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class UIBase : MonoBehaviour
{
    protected static readonly string GreenColor = "#9AC4DB";
    protected static readonly string WhiteColor = "#FFFFFF";
    protected static readonly string BlackColor = "#000000";
    
    public UnityEvent OnClickEvent;
    public UnityEvent OnHoverEvent;
    public UnityEvent OnHoverExitEvent;
    public UnityEvent OnExitEvent;
    public UnityEvent OnForceQuitEvent;
    
    private void Update()
    {
        if (IsTopUI())
        {
            HandleKeyboardInput();
        }
    }
    
    public virtual void OnOpen()
    {
        Debug.Log($"#{gameObject.name}이(가) 열렸습니다.");
    }
    
    public virtual void OnOpen(EvidenceStructure evidence)
    {
        if (evidence == null)
        {
            Debug.LogError("🔥 evidence가 null이므로 OpenUI()를 호출할 수 없습니다.");
            return;
        }
        Debug.Log($"#{gameObject.name}이(가) 열렸습니다.");
    }
    
    public virtual void OnClose()
    {
        Debug.Log($"#{gameObject.name}이(가) 닫혔습니다.");
    }
    
    public virtual void HandleKeyboardInput()
    {
        
    }
    
    public virtual void HandleMouseInput()
    {
        //UI에 대해 공통적인 마우스 입력 상호작용이 있다면 여기서 작성.
        //각 UI마다 별도로 마우스 입력 상호작용이 있다면 해당 UI 스크립트에서 override해서 작성.
    }
    
    public bool IsTopUI()
    {
        return UIManager.Instance.GetTopUI() == this;
    }

    
    public void RemoveAllListeners()
    {
        OnHoverEvent.RemoveAllListeners();
        OnClickEvent.RemoveAllListeners();
        OnHoverExitEvent.RemoveAllListeners();
        OnExitEvent.RemoveAllListeners();
        OnForceQuitEvent.RemoveAllListeners();
    }
    
    public void SetButtonSelected(GameObject btn, Color color)
    {
        TextMeshProUGUI tmp = btn.GetComponent<TextMeshProUGUI>();
        tmp.color = color;
    }
    
    public void AddOnClickListener(UnityAction action)
    {
        OnClickEvent.AddListener(action);
    }

    public void AddOnHoverListener(UnityAction action)
    {
        OnHoverEvent.AddListener(action);
    }
    
    public void AddOnHoverExitListener(UnityAction action)
    {
        OnHoverExitEvent.AddListener(action);
    }
    
    public void AddOnExitListener(UnityAction action)
    {
        OnExitEvent.AddListener(action);
    }
    
    public void RemoveOnClickListener(UnityAction action)
    {
        OnClickEvent.RemoveListener(action);
    }

    public void RemoveOnHoverListener(UnityAction action)
    {
        OnHoverEvent.RemoveListener(action);
    }
    
    public void RemoveOnHoverExitListener(UnityAction action)
    {
        OnHoverExitEvent.RemoveListener(action);
    }
    
    public void RemoveOnExitListener(UnityAction action)
    {
        OnExitEvent.RemoveListener(action);
    }

}
