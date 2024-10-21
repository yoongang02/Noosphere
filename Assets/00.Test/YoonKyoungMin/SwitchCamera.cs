using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SwitchCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _corridorCamera;
    [SerializeField] private CinemachineVirtualCamera _roomCamera;

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Switch();
        }
    }


    void Switch()
    {
        (_corridorCamera.Priority, _roomCamera.Priority) = (_roomCamera.Priority, _corridorCamera.Priority);
    }
}
