using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] private Transform cam;
     private void FixedUpdate()
     {
         transform.localPosition = cam.transform.localPosition;
         transform.localRotation = cam.transform.localRotation;
     }
}
