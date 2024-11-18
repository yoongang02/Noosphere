using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueObjectController : MonoBehaviour
{
     [SerializeField] private float _scaleSpeed = 0.1f; 
     [SerializeField] private float _minScale = 0.1f;   
     [SerializeField] private float _maxScale = 3f;    
     [SerializeField] private float _rotationSpeed = 100f;  
     private Vector3 _previousMousePosition;

     void Update()
     {
          HandleScaling();
          HandleRotation();
     }
     
     private void HandleScaling()
     {
          float scrollInput = Input.GetAxis("Mouse ScrollWheel");
          if (scrollInput != 0)
          {
               Vector3 newScale = transform.localScale + Vector3.one * scrollInput * _scaleSpeed;
               newScale = ClampScale(newScale, _minScale, _maxScale);
               transform.localScale = newScale;
          }
     }

     // 마우스 드래그를 이용한 회전 조정
     private void HandleRotation()
     {
          // 마우스 왼쪽 버튼을 눌렀을 때
          if (Input.GetMouseButtonDown(0))
          {
               _previousMousePosition = Input.mousePosition;
          }

          // 마우스 왼쪽 버튼을 누르고 있을 때
          if (Input.GetMouseButton(0))
          {
               Vector3 deltaMousePosition = Input.mousePosition - _previousMousePosition;
               _previousMousePosition = Input.mousePosition;

               float rotationX = deltaMousePosition.y * _rotationSpeed * Time.deltaTime;
               float rotationY = -deltaMousePosition.x * _rotationSpeed * Time.deltaTime;
               
               transform.Rotate(Vector3.up, rotationY, Space.World);
               transform.Rotate(Vector3.right, rotationX, Space.Self);
          }
     }
     
     private Vector3 ClampScale(Vector3 scale, float min, float max)
     {
          return new Vector3(
               Mathf.Clamp(scale.x, min, max),
               Mathf.Clamp(scale.y, min, max),
               Mathf.Clamp(scale.z, min, max)
          );
     }
}
