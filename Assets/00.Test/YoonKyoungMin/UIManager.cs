using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject _showPressBtnUI;
    public GameObject _bookInfo;
    public GameObject _bookPopUp;
    public bool isPopUpOpen = false;
    public Animator transitionAnimator;

    public void CancelInvestigate()
    {
        Debug.Log("no 클릭");
        _bookInfo.SetActive(false);
        FindObjectOfType<PlayerController>().isDialogueOn = false;
        FindObjectOfType<PlayerInteract>().isInteracting = false;
    }

    public void InvestigateBook()
    {
        Debug.Log("yes 클릭");
        _bookInfo.SetActive(false);
        _bookPopUp.SetActive(true);
        isPopUpOpen = true;
    }
}
