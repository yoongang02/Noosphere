using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag3DBook : MonoBehaviour
{
    private Vector3 m_Offset;
    private float m_ZCoord;
    private GameObject selectedNPC;
    private Camera mainCamera;
    
    [SerializeField] private LayerMask bookLayer; // NPC가 있는 레이어 설정
    
    private void Start()
    {
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        // 마우스 클릭 시 NPC 선택
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            // 레이어 마스크를 사용하여 NPC 레이어의 객체만 히트
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, bookLayer))
            {
                selectedNPC = hit.collider.gameObject;
                m_ZCoord = mainCamera.WorldToScreenPoint(selectedNPC.transform.position).z;
                m_Offset = selectedNPC.transform.position - GetMouseWorldPosition();
            }
        }
        
        // 드래그 처리
        if (Input.GetMouseButton(0) && selectedNPC != null)
        {
            // 선택된 NPC 이동
            selectedNPC.transform.position = GetMouseWorldPosition() + m_Offset;
        }
        
        // 마우스 버튼 놓을 때 선택 해제
        if (Input.GetMouseButtonUp(0))
        {
            selectedNPC = null;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = m_ZCoord;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}