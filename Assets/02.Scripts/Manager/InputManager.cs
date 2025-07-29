using System;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public Action moveAction = null;
    public Action selectBtnAction = null;
    public Action exitBtnAction = null;

    public void OnUpdate()
    {
        // if(Input.anyKey==false)
        //     return;
        // moveAction?.Invoke();

        if (InputRouter.Instance.ConsumeEscape())
            exitBtnAction?.Invoke();

        if (InputRouter.Instance.ConsumeE())
            selectBtnAction?.Invoke();
    }
    public void FixedUpdate()
    {
        if(Input.anyKey==false)
            return;
        moveAction?.Invoke();
    }
}

