using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    /*
    public GameObject _showPressBtnUI;
    public GameObject _bookInfo;
    public GameObject _bookPopUp;
    public bool isPopUpOpen = false;
    public Animator transitionAnimator;
    public GameObject _endingMessage;
    public TextMeshProUGUI dialogueUI;
    */

    //증거물 조사 UI
    [SerializeField] private GameObject _investigateUI;

    
    //증거물 조사 UI의 정보 세팅하기
    public void SetInvestigateUI(EvidenceStructure evidence)
    {
        TextMeshProUGUI[] texts = _investigateUI.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var text in texts)
        {
            if (text.gameObject.name == "Evidence Name")
            {
                text.text = evidence.evidence_name;
            }
            else if (text.gameObject.name == "Evidence Text")
            {
                text.text = evidence.evidence_Text_Display;
            }
        }
        
        Image[] images = _investigateUI.GetComponentsInChildren<Image>(true);
        foreach (var image in images)
        {
            if (image.gameObject.name == "Evidence Img")
            {
                //아트 리소스 불러오기
                if (EventManagerYKM.Instance._artResources.ContainsKey(evidence.artresource_id))
                {
                    image.sprite = EventManagerYKM.Instance._artResources[evidence.artresource_id].GetSpriteFromFilePath();
                }
                else
                {
                    Debug.Log(evidence.artresource_id +"가 리소스 내에 존재하지 않습니다.");
                }
                break;
            }
        }
    }

    public void OpenInvestigateUI(EvidenceStructure evidence)
    {
        SetInvestigateUI(evidence);
        _investigateUI.SetActive(true);
    }
    
    public void CancelInvestigate()
    {
        /*
        Debug.Log("no 클릭");
        _bookInfo.SetActive(false);
        FindObjectOfType<PlayerController>().isDialogueOn = false;
        FindObjectOfType<PlayerInteract>().isInteracting = false;
        */
    }

    public void InvestigateBook()
    {
        /*
        Debug.Log("yes 클릭");
        _bookInfo.SetActive(false);
        _bookPopUp.SetActive(true);
        isPopUpOpen = true;
        */
    }

    /*
    public IEnumerator ActiveEndingMessage()
    {
        
        yield return new WaitForSeconds(1.5f);
        _endingMessage.SetActive(true);
        FindObjectOfType<PlayerInteract>().isEnd = true;
    }
    */
}
