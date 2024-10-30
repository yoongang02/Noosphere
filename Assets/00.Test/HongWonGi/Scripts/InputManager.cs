using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Action moveAction = null;
    public Action selectBtnAction = null;
    public Action exitBtnAction = null;

    public void OnUpdate()
    {
        if(Input.anyKey==false)
            return;
        moveAction?.Invoke();

        if (Input.GetKeyDown(KeyCode.Escape))
            exitBtnAction?.Invoke();

        if (Input.GetKeyDown(KeyCode.E))
            selectBtnAction?.Invoke();
    }
}

