using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EscapeUI : Singleton<EscapeUI>, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        //젤 위에 있는 UI 아니면 작동 X
        if (!UIManager.Instance.IsUIOpen(UIManager.Instance.inventoryUI))
        {
            return;
        }
        
        //클릭한 오브젝트가 챕터인지 파악
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SoundManager.Instance.PlaySFX("Soundresource_036");
            UIManager.Instance.CloseTopUI();
        }
    }
}
