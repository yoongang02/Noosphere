using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRouter : Singleton<InputRouter>
{
    public bool SpacePressed { get; private set; }
    private bool spaceConsumed;
    
    void Update()
    {
        SpacePressed = Input.GetKeyDown(KeyCode.Space);
        spaceConsumed = false;
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
}
