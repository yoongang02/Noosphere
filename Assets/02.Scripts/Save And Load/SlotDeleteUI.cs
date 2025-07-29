using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotDeleteUI : DefaultUIBase
{
    public override void OnOpen()
    {
        base.OnOpen();

        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();
    }
}
