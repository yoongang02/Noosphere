using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardKeyButton : MonoBehaviour
{
    [SerializeField] private ButtonEventChannel _eventChannel;
    [SerializeField] private int _buttonId;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private GameObject _cardObj;
    private bool _isTakenCard;
    

    private void Awake()
    {
        _button.onClick.AddListener(OnClickCard);
    }
    private void OnEnable()
    
    {
        _eventChannel.OnButtonClicked += OnOtherButtonClicked;
        _eventChannel.OnResetCardState += OnCardStateReturn; 
    }

    private void OnDisable()
    {
        _eventChannel.OnButtonClicked -= OnOtherButtonClicked;
        _eventChannel.OnResetCardState -= OnCardStateReturn; 
    }

    private void OnClickCard()
    {
        if (!_isTakenCard)//카드 가져갔을때
        {
            _cardObj.SetActive(false);
            _buttonText.text = "반납 하기";
            _isTakenCard = true;
            CardKeyManager.Instance.curCardID = _buttonId;
            //얻은 카드 인벤토리에 저장
            _eventChannel.RaiseEvent(_buttonId);
        }
        else//카드 반납할때
        { 
            _cardObj.SetActive(true);
            _buttonText.text = "사용 가능";
            _isTakenCard = false;
            CardKeyManager.Instance.curCardID = -1; 
            //인벤토리에 있는 카드 다시 반납
            _eventChannel.RaiseReturnEvent(_buttonId);
        }
 
    }

    private void OnOtherButtonClicked(int clickedButtonId)//클릭한 버튼이외에 다른 버튼들 이벤트 전달
    {
        if(clickedButtonId != _buttonId)
        {
            Debug.Log($"Button {_buttonId} received event from Button {clickedButtonId}");
            _buttonText.text = "사용 불가";
            _button.interactable = false;
            _isTakenCard = false;
        }
    }
    private void OnCardStateReturn(int returnedButtonId)
    {
        if(returnedButtonId != _buttonId)
        {
            _buttonText.text = "사용 가능";
            _button.interactable = true;
        }
    }
}
