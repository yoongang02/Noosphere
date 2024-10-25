using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.WebSockets;

// 플레이어 이동관련 함수
// ==> 추후 로직변경 필요
// 최초 작성자: 홍원기
// 수정자: 
// 최종 수정일: 2024-10-25
public class PlayerController : Singleton<PlayerController>
{ 
    [SerializeField] private float _moveSpeed;       
    [SerializeField] private float _sprintMultiplier = 2f; 
    
    private Vector3 _moveDirection;
    private Animator _animator; 
    private float _defaultSpeed;
    public bool isDialogueOn=false; //대화시작
    private bool isPlayerNearNPC = false;//플레이어 NPC가까이있나? 
    private GameObject _currentNPC;
    public delegate void MoveDelegate(Vector3 direction);
    public event MoveDelegate OnMove;
    public bool isPlayerMove = false;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _defaultSpeed = _moveSpeed;
        OnMove += Move;
    }
    
    private void Update()
    {
        HandleInput();
        if (isDialogueOn && !FindObjectOfType<UIManager>().isPopUpOpen)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ContinueDialogue();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                EndDialogue();
            }
        }
        else if (!isDialogueOn && isPlayerNearNPC && Input.GetKeyDown(KeyCode.E))
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        isDialogueOn = true;
        UIManager2.Instance.popUpUI.SetActive(false); 
        UIManager2.Instance.dialogueUI.gameObject.SetActive(true);
        NpcDialogue npcDialogue = _currentNPC.GetComponent<NpcDialogue>();
        if (npcDialogue != null)
        {
            npcDialogue.StartDialogue(); // NPC의 대화 시작 메서드 호출
        }
    }
    private void ContinueDialogue()
    {
        NpcDialogue npcDialogue = _currentNPC.GetComponent<NpcDialogue>();
        if (npcDialogue != null)
        {
            npcDialogue.AdvanceDialogue(); // NPC의 다음 대화 내용으로 이동
            if (!npcDialogue.isOnDialogue) // 대화가 끝났다면 대화 상태 종료
            {
                UIManager2.Instance.dialogueUI.gameObject.SetActive(false);
                isDialogueOn = false;
            }
        }
    }
    private void EndDialogue()
    {
        isDialogueOn = false;
        UIManager2.Instance.dialogueUI.gameObject.SetActive(false);
        NpcDialogue npcDialogue = _currentNPC.GetComponent<NpcDialogue>();
        if (npcDialogue != null)
        {
            npcDialogue.ResetDialogue();
        }
    }
    public void HandleInput()
    {
        if (isDialogueOn)
        {
            _animator.SetFloat("MoveSpeed",0f);
            return;
        }

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isPlayerNearNPC = true;  // 플레이어가 NPC 범위 안에 들어옴
            _currentNPC = other.gameObject; 
            UIManager2.Instance.popUpUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isPlayerNearNPC = false;  // 플레이어가 NPC 범위에서 벗어남
            _currentNPC = null;
            UIManager2.Instance.popUpUI.SetActive(false);
        }
    }
}