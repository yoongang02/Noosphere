using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionMarkManager : Singleton<InteractionMarkManager>
{
    [Header("��ȣ�ۿ� �ȳ� Ű ������")]
    [SerializeField] private GameObject _interactionKeyPrefab; // ��ȣ�ۿ� Ű
    [SerializeField] private GameObject _enterMentalKeyPrefab; // ���ż��� ���� Ű
    [SerializeField] private GameObject _useEvidenceKeyPrefab; // ���Ź� ��� Ű


    // �ȳ� Ű UI Ȱ��ȭ �ϱ�
    public void EnableInteractionMarkUI(Transform trigger, string eventID)
    {
        // EventTrigger ��ũ��Ʈ�� �����Ǿ��ִ� ������Ʈ�� �ڽ� ������Ʈ �� InteractionMark ��ũ��Ʈ�� ���� �ִ� ������Ʈ ã��
        Transform parent = trigger.GetComponentInChildren<InteractionMark>(true).transform;

        // �ش� ������Ʈ�� �ڽ� ������Ʈ ��� �ı��ϱ� �ʱ�ȭ
        foreach (Transform chiild in parent)
        {
            Destroy(chiild.gameObject);
        }

        // ��ȣ�ۿ� Ʈ������ ���
        if (trigger.tag == "EventInteractionTrigger")
        {
            // ��ȣ�ۿ� Ű�� ������ ����
            GameObject interactionKey = Instantiate(_interactionKeyPrefab);
            interactionKey.transform.SetParent(parent, false);

            // �ش� �̺�Ʈ�� ���ǿ� ���Ź��� �ִ� ��쿡�� ���Ź� ��� Ű�� ���
            // �̹� �� �Լ��� ������� ���Ź� ���̵� ���� ������ �Ϸ�Ǿ��⿡ �߰������� ���� �������� ����
            EventStructure _event = DataManager.Instance._events[eventID];

            PlayerInteract.Instance.canUse = false;
            foreach (var condition in _event.conditions)
            {
                if (condition.StartsWith("Evidence"))
                {
                    EvidenceStructure evidence = DataManager.Instance._evidences[condition];
                    // �ش� ���Ź��� ��� ������ ���Ź��̰� �κ��丮�� �ִ��� üũ
                    if(evidence.canUse == 'Y' && InventoryManager.Instance.IsEvidenceInInventory(evidence.evidenceId))
                    {
                        GameObject evidenceKey = Instantiate(_useEvidenceKeyPrefab);
                        evidenceKey.transform.SetParent(parent, false);
                        PlayerInteract.Instance.canUse = true;
                        
                        Debug.LogWarning("OnEvidenceUse 액션에 메소드 등록");
                        PlayerInteract.Instance.OnEvidenceUse = null;
                        PlayerInteract.Instance.OnEvidenceUse += () =>
                        {
                            // ���Ź� ����ϱ� �������� �κ��丮 �ʱ�ȭ
                            InventoryManager.Instance.isUsingEvidence = true;
                            InventoryManager.Instance.GetComponent<InventoryNavigator>()
                                .SetEvidenceUseEventID(_event);
                            // �κ��丮 ���� �ڵ�
                            UIManager.Instance.OpenUI(UIManager.Instance.inventoryUI);
                        };
                        break;
                    }
                }
            }
        }
        else if (trigger.tag == "EventMentalEnterTrigger") // ���ż��� ���� Ʈ������ ���
        {
            GameObject mentalKey = Instantiate(_enterMentalKeyPrefab);
            mentalKey.transform.SetParent(parent, false);
        }
        else
        {
            Debug.LogError("��ȣ�ۿ� mark parent�� �߸��� �±׸� ���� �ֽ��ϴ�.");
        }

        // ��ȣ�ۿ� Ű UI �ʱ�ȭ �� Ȱ��ȭ -> �ڵ� �ִϸ��̼� ����
        parent.gameObject.SetActive(true);
    }

    // �ȳ� Ű UI ��Ȱ��ȭ �ϱ�
    public void DisableInteractionMarkUI(Transform trigger)
    {
        // EventTrigger ��ũ��Ʈ�� �����Ǿ��ִ� ������Ʈ�� �ڽ� ������Ʈ �� InteractionMark ��ũ��Ʈ�� ���� �ִ� ������Ʈ ã��
        Transform parent = trigger.GetComponentInChildren<InteractionMark>(true).transform;

        // �ִϸ����Ϳ��� ��Ȱ��ȭ �ִϸ��̼� ����
        parent.GetComponent<Animator>().SetTrigger("Hide");
    }
}
