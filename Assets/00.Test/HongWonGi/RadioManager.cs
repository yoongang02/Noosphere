using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RadioManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI radioText;
    [SerializeField] public List<DialBtn> dialBtn;

    [SerializeField] private Button _powerBtn;
    

    private int _tenDigit = 0;
    private int _oneDigit = 0;
    private int _decimalDigit = 0;

    private void Start()
    {
        _powerBtn.onClick.AddListener(CheckAnswer);
    }

    private void CheckAnswer()
    {
        if (radioText.text == "95.3MHz")
        {
            gameObject.SetActive(false);
            Debug.Log("정답");
        }
        else
        {
            Debug.Log("오답");
        }
    }

    private void OnEnable()
    {
        ResetText();
        if (dialBtn != null && dialBtn.Count >= 3)
        {
            dialBtn[0].OnValueChanged += OnTenDigitChanged;
            dialBtn[1].OnValueChanged += OnOneDigitChanged;
            dialBtn[2].OnValueChanged += OnDecimalDigitChanged;
        }
    }

    private void OnDisable()
    {
        // 이벤트 해제
        if (dialBtn != null && dialBtn.Count >= 3)
        {
            dialBtn[0].OnValueChanged -= OnTenDigitChanged;
            dialBtn[1].OnValueChanged -= OnOneDigitChanged;
            dialBtn[2].OnValueChanged -= OnDecimalDigitChanged;
        }
    }

    private void ResetText()
    {
        // 각 다이얼 버튼의 이벤트 등록
        _tenDigit = 0;
        _oneDigit = 0;
        _decimalDigit = 0;
        radioText.text = $"{_tenDigit}{_oneDigit}.{_decimalDigit}MHz";
    }

    private void OnTenDigitChanged(int value)
    {
        _tenDigit = value;
        UpdateRadioText();
    }

    private void OnOneDigitChanged(int value)
    {
        _oneDigit = value;
        UpdateRadioText();
    }

    private void OnDecimalDigitChanged(int value)
    {
        _decimalDigit = value;
        UpdateRadioText();
    }

    private void UpdateRadioText()
    {
        radioText.text = $"{_tenDigit}{_oneDigit}.{_decimalDigit}MHz";
    }
}