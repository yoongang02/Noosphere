using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DefaultUIBase : MonoBehaviour
{
    private void Update()
    {
        if (IsTopUI())
        {
            HandleKeyboardInput();
            HandleMouseInput();
        }
    }

    public virtual void OnOpen()
    {
        Debug.Log($"#{gameObject.name}이(가) 열렸습니다.");
    }

    public virtual void OnClose()
    {
        Debug.Log($"#{gameObject.name}이(가) 닫혔습니다.");
    }

    public virtual void HandleKeyboardInput() { }
    public virtual void HandleMouseInput() { }

    public bool IsTopUI()
    {
        return DefaultUIController.Instance.GetTopUI() == this;
    }
}
