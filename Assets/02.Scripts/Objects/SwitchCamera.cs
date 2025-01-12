using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SwitchCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera camera1;
    [SerializeField] private CinemachineVirtualCamera camera2;

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Switch();
        }
    }


    void Switch()
    {
        (camera1.Priority, camera2.Priority) = (camera1.Priority, camera2.Priority);
    }
}
