using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BtnHover : MonoBehaviour
{
    public void EnterHover()
    {
        this.GetComponent<TextMeshProUGUI>().color = Color.green;
    }

    public void ExitHover()
    {
        this.GetComponent<TextMeshProUGUI>().color = Color.white;
    }
}
