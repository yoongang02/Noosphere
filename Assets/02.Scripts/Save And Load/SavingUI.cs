using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingUI : DefaultUIBase
{
    [SerializeField] private float _waitTime = 2f;
    public override void OnOpen()
    {
        base.OnOpen();
        PlayerInteract.Instance.canInteract = false;
        transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(EndSaving());
    }

    public override void OnClose()
    {
        base.OnClose();
        transform.GetChild(0).gameObject.SetActive(false);
        PlayerInteract.Instance.canInteract = true;
    }

    IEnumerator EndSaving()
    {
        yield return new WaitForSeconds(_waitTime);
        DefaultUIController.Instance.CloseTopUI();
    }
}
