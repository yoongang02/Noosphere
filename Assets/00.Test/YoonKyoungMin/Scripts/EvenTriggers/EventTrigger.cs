using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    public List<string> eventIdList = new List<string>();
    public bool canDestroyEvidence; //트리거가 장착된 증거물 오브젝트를 파괴
    public bool canDestroyWhenExecutionComplete; //실행 완료되면 트리거 자체를 파괴
}
