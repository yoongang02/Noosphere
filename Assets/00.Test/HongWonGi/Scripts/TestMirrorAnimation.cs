using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMirrorAnimation : MonoBehaviour
{
    [SerializeField] private Animator mirror;
    [SerializeField] private GameObject black;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // mirror.SetBool("Is");
        }
    }
}
