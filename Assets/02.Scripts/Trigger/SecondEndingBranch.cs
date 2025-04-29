using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SecondEndingBranch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 다프네에게 연구소 도면을 받았다면
            if (DataManager.Instance._events["Event_B065"].isExecuted && InventoryManager.Instance.IsEvidenceInInventory("Evidence_016"))
            {
                Debug.LogWarning("[일반 엔딩] 탈출 성공");
                EventManagerYKM.Instance.ExecuteEvent("Event_D103").Forget();
            }
            else
            {
                Debug.LogWarning("[일반 엔딩] 탈출 실패");
                EventManagerYKM.Instance.ExecuteEvent("Event_D104").Forget();
            }
        }
    }
}