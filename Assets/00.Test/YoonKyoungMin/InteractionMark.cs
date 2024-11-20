using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionMark : MonoBehaviour
{
    [SerializeField] private Camera _curCamera;
    void LateUpdate()
    {
        if (_curCamera != null)
        {
            transform.LookAt(transform.position + _curCamera.transform.forward);
        }
    }
}
