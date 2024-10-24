using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// 플레이어 이동관련 함수
// ==> 추후 로직변경 필요
// 최초 작성자: 홍원기
// 수정자: 
// 최종 수정일: 2024-10-25
public class PlayerController : MonoBehaviour
{ 
    [SerializeField] private float _moveSpeed;       
    [SerializeField] private float _sprintMultiplier = 2f; 
    
    private Vector3 _moveDirection;
    private Animator _animator; 
    private float _defaultSpeed;
    public delegate void MoveDelegate(Vector3 direction);
    public event MoveDelegate OnMove;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _defaultSpeed = _moveSpeed;
        OnMove += Move;
    }

    private void Update()
    {
        HandleInput();
    }
    public void HandleInput()
    {
        float moveX = 0f;
        float moveY = 0f;
        float currentSpeed = _moveDirection.magnitude * _moveSpeed;
        _animator.SetFloat("MoveSpeed",currentSpeed);

        // WASD 키 입력 처리
        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;

        _moveDirection = new Vector3(moveX, 0f, moveY).normalized;
        
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            _moveSpeed = _defaultSpeed * _sprintMultiplier;
        }
        else
        {
            _moveSpeed = _defaultSpeed; 
        }

        if (_moveDirection != Vector3.zero)
        {
            OnMove?.Invoke(_moveDirection);
        }
    }

    private void Move(Vector3 direction)
    {
        // 캐릭터가 이동하는 방식
        transform.rotation = Quaternion.LookRotation(direction);
        transform.Translate(Vector3.forward * Time.deltaTime * _moveSpeed);
    }

    private void OnDisable()
    {
        OnMove -= Move;
    }
}