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
    [SerializeField] private GameObject cardObj;
    private bool isTakenCard=false;
    

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }
    private void OnEnable()
    {
        _eventChannel.OnButtonClicked += OnOtherButtonClicked;
    }

    private void OnDisable()
    {
        _eventChannel.OnButtonClicked -= OnOtherButtonClicked;
    }

    private void OnClick()
    {
        if (!isTakenCard)
        {
            cardObj.SetActive(false);
            _buttonText.text = "반납 하기";
            _eventChannel.RaiseEvent(_buttonId);
        }
        else
        { 
            cardObj.SetActive(true);
            _buttonText.text = "사용 가능";
            isTakenCard = false;
            
        }
 
    }

    private void OnOtherButtonClicked(int clickedButtonId)
    {
        // 클릭된 버튼이 자신이 아닐 때의 동작
        if(clickedButtonId != _buttonId)
        {
            Debug.Log($"Button {_buttonId} received event from Button {clickedButtonId}");
            _buttonText.text = "사용 불가";
            _button.interactable = false;
            isTakenCard = false;
        }
    }
}
