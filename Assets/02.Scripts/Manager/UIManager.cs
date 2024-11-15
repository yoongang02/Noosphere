using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    public GameObject _showPressBtnUI;
    public GameObject _bookInfo;
    public GameObject _bookPopUp;
    public bool isPopUpOpen = false;
    public Animator transitionAnimator;
    public GameObject _endingMessage;
    public TextMeshProUGUI dialogueUI;
    public TextMeshProUGUI popUI;

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

    public IEnumerator ActiveEndingMessage()
    {
        yield return new WaitForSeconds(1.5f);
        _endingMessage.SetActive(true);
        FindObjectOfType<PlayerInteract>().isEnd = true;
    }

    public void PopUp(bool isActive,string text="")
    {
        popUI.gameObject.SetActive(isActive);
        popUI.text = text;
    }
    
}
