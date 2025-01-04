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
        if (!_isTakenCard)
        {
            _cardObj.SetActive(false);
            _buttonText.text = "반납 하기";
            _isTakenCard = true;
            _eventChannel.RaiseEvent(_buttonId);
        }
        else
        { 
            _cardObj.SetActive(true);
            _buttonText.text = "사용 가능";
            _isTakenCard = false;
            _eventChannel.RaiseReturnEvent(_buttonId);
        }
 
    }

    private void OnOtherButtonClicked(int clickedButtonId)
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
