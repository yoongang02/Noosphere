using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionMarkManager : Singleton<InteractionMarkManager>
{
    [Header("상호작용 안내 키 프리팹")]
    [SerializeField] private GameObject _interactionKeyPrefab; // 상호작용 키
    [SerializeField] private GameObject _enterMentalKeyPrefab; // 정신세계 진입 키
    [SerializeField] private GameObject _useEvidenceKeyPrefab; // 증거물 사용 키


    // 안내 키 UI 활성화 하기
    public void EnableInteractionMarkUI(Transform trigger, string eventID)
    {
        // EventTrigger 스크립트가 부착되어있는 오브젝트의 자식 오브젝트 중 InteractionMark 스크립트를 갖고 있는 오브젝트 찾기
        Transform parent = trigger.GetComponentInChildren<InteractionMark>(true).transform;

        // 해당 오브젝트의 자식 오브젝트 모두 파괴하기 초기화
        foreach (Transform chiild in parent)
        {
            Destroy(chiild.gameObject);
        }

        // 상호작용 트리거인 경우
        if (trigger.tag == "EventInteractionTrigger")
        {
            // 상호작용 키는 무조건 존재
            GameObject interactionKey = Instantiate(_interactionKeyPrefab);
            interactionKey.transform.SetParent(parent, false);

            // 해당 이벤트의 조건에 증거물이 있는 경우에는 증거물 사용 키도 띄움
            // 이미 이 함수로 오기까지 증거물 아이디에 대한 검증이 완료되었기에 추가적으로 검증 진행하지 않음
            EventStructure _event = DataManager.Instance._events[eventID];

            PlayerInteract.Instance.canUse = false;
            foreach (var condition in _event.conditions)
            {
                if (condition.StartsWith("Evidence"))
                {
                    EvidenceStructure evidence = DataManager.Instance._evidences[condition];
                    // 해당 증거물이 사용 가능한 증거물이고 인벤토리에 있는지 체크
                    if(evidence.canUse == 'Y' && InventoryManager.Instance.IsEvidenceInInventory(evidence.evidenceId))
                    {
                        GameObject evidenceKey = Instantiate(_useEvidenceKeyPrefab);
                        evidenceKey.transform.SetParent(parent, false);
                        PlayerInteract.Instance.canUse = true;
                        PlayerInteract.Instance.OnEvidenceUse += () =>
                        {
                            // 증거물 사용하기 설정으로 인벤토리 초기화
                            InventoryManager.Instance.isUsingEvidence = true;
                            // 인벤토리 열기 코드
                            UIManager.Instance.OpenUI(UIManager.Instance.inventoryUI);
                        };
                        break;
                    }
                }
            }
        }
        else if (trigger.tag == "EventMentalEnterTrigger") // 정신세계 진입 트리거인 경우
        {
            GameObject mentalKey = Instantiate(_enterMentalKeyPrefab);
            mentalKey.transform.SetParent(parent, false);
        }
        else
        {
            Debug.LogError("상호작용 mark parent가 잘못된 태그를 갖고 있습니다.");
        }

        // 상호작용 키 UI 초기화 후 활성화 -> 자동 애니메이션 실행
        parent.gameObject.SetActive(true);
    }

    // 안내 키 UI 비활성화 하기
    public void DisableInteractionMarkUI(Transform trigger)
    {
        // EventTrigger 스크립트가 부착되어있는 오브젝트의 자식 오브젝트 중 InteractionMark 스크립트를 갖고 있는 오브젝트 찾기
        Transform parent = trigger.GetComponentInChildren<InteractionMark>(true).transform;

        // 애니메이터에서 비활성화 애니메이션 실행
        parent.GetComponent<Animator>().SetTrigger("Hide");
    }
}
