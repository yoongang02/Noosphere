using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CreditUI : MonoBehaviour
{
    [SerializeField] private RawImage _creditImg;
    [SerializeField] private Button _exitBtn;

    private void Start()
    {
        _exitBtn.onClick.AddListener(() => { gameObject.SetActive(false); });
    }

    private void OnEnable()
    {
        _creditImg.rectTransform.anchoredPosition = new Vector2(0, -1024);
        _creditImg.rectTransform.DOAnchorPosY(2048f, 30f).SetEase(Ease.Linear);
    }

    private void OnDisable()
    {
        DOTween.Kill(_creditImg.rectTransform);
    }
}