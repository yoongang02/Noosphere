using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private GameObject _curInteractableObj;
    private bool _canInteract = false;
    public bool isInteracting = false;
    [SerializeField] private UIManager _uiManager;

    void Update()
    {
        //상호작용 가능한데, E 버튼 클릭하면
        if (_canInteract && Input.GetKeyUp(KeyCode.E))
        {
            //현재 상호작용 오브젝트 내의 public 함수 호출
            _uiManager._showPressBtnUI.SetActive(false);
            _uiManager._bookInfo.SetActive(true);
            isInteracting = true;
        }
        
        //팝업 열려있으면 ESC버튼을 통해 팝업 끌 수 있음.
        if (_uiManager.isPopUpOpen && Input.GetKeyUp(KeyCode.Escape))
        {
            _uiManager._bookPopUp.SetActive(false);
            isInteracting = false;
        }
    }
  
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("InvestigateObj"))
        {
            _canInteract = true;
            _curInteractableObj = other.gameObject;
            //ui에 텍스트 띄우기
            _uiManager._showPressBtnUI.SetActive(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("InvestigateObj"))
        {
            _canInteract = false;
            _curInteractableObj = null;
            //ui 텍스트 없애기
            _uiManager._showPressBtnUI.SetActive(false);
        }
    }
}
