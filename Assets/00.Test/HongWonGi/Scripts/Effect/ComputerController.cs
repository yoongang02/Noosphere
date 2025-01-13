using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerController : UIBase
{
    [SerializeField] private ButtonEventChannel _eventChannel;
    [SerializeField] private Button _exitBtn;
    
    public override void OnOpen()
    {
        base.OnOpen();
        transform.GetChild(0).gameObject.SetActive(true);
        _eventChannel.RaiseEvent("0");//컴퓨터 열었을때 맨처음 버튼 눌리는 이벤트 전달
        _exitBtn.onClick.AddListener(()=>UIManager.Instance.CloseTopUI());
    }
    
    public override void OnClose()
    {
        base.OnClose();
        _exitBtn.onClick.RemoveAllListeners();
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
