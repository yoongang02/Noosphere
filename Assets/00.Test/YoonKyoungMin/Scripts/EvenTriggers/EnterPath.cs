using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterPath : MonoBehaviour
{
    public string departureScene = "";
    public string arrivalScene = "";

    public void StartEnterToPath()
    {
        PlayerInteract.Instance.curEnterNPCTrigger = null;
        if (PlayerInteract.Instance.isPlayerInMetanlWorld)
        {
            PlayerInteract.Instance.isPlayerInMetanlWorld = false;
            SceneManager.LoadScene(departureScene);
        }
        else
        {
            PlayerInteract.Instance.isPlayerInMetanlWorld = true;
            SceneManager.LoadScene(arrivalScene);
        }
    }
}
