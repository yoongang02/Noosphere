using System.Collections;
using System.Collections.Generic;
using NooSphere;
using UnityEngine;

public class ArtResource0080 : MonoBehaviour
{
    void Start()
    {
        if (NooSphere.SaveManager.Instance.CurrentLoadType == GameLoadType.ContinueGame)
        {
            Destroy(this.gameObject);
        }
    }
}
