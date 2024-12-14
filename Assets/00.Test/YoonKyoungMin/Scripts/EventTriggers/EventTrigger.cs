using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    [Header("이벤트 목록")]
    public List<string> eventIdList = new List<string>();
    [Space(5)][Header("트리거 설정")]
    public bool destroyEvidence; //트리거가 장착된 증거물 오브젝트를 파괴
    public bool destroyTrigger; //실행 완료되면 트리거 자체를 파괴
    [Space(5)][Header("NPC 관련")]
    public GameObject npcCameraPoint;
    public bool isNpc;

}
