using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeOutEffect : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;

    private void OnEnable()
    {
        _fadeImage.color = new Color(0, 0, 0, 0);
        _fadeImage.DOFade(1f, 1f)
            .SetEase(Ease.InOutQuad).OnComplete(() => { gameObject.SetActive(false); });
    }
}