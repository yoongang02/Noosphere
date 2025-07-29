using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRouter : Singleton<InputRouter>
{
    public bool SpacePressed { get; private set; }
    public bool EscapePressed { get; private set; }
    private bool spaceConsumed;
    private bool escapeConsumed;
    
    void Update()
    {
        SpacePressed = Input.GetKeyDown(KeyCode.Space);
        spaceConsumed = false;

        EscapePressed = Input.GetKeyDown(KeyCode.Escape);
        escapeConsumed = false;
    }

    public bool ConsumeSpace()
    {
        if (SpacePressed && !spaceConsumed)
        {
            spaceConsumed = true;
            return true;
        }
        return false;
    }

    public bool ConsumeEscape()
    {
        if (EscapePressed && !escapeConsumed)
        {
            escapeConsumed = true;
            return true;
        }
        return false;
    }
}
