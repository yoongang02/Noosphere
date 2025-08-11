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
        SoundManager.Instance.PlaySFX("Soundresource_037");
        Debug.Log($"#{gameObject.name}이(가) 열렸습니다.");
    }

    public virtual void OnClose()
    {
        SoundManager.Instance.PlaySFX("Soundresource_036");
        Debug.Log($"#{gameObject.name}이(가) 닫혔습니다.");
    }

    public virtual void HandleKeyboardInput() { }
    public virtual void HandleMouseInput() { }

    public bool IsTopUI()
    {
        return DefaultUIController.Instance.GetTopUI() == this;
    }
    public void OnClickSound()
    {
        SoundManager.Instance.PlaySFX("Soundresource_037");
    }

    public void OnHoverSound()
    {
        SoundManager.Instance.PlaySFX("Soundresource_035");
    }
}
