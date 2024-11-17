using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
// 플레이어 이동관련 함수
//플레이어 이동방식 변경
// 최초 작성자: 홍원기
// 수정자: 홍원기
// 최종 수정일: 2024-11-01
public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private float _moveSpeed;       
    [SerializeField] private float _sprintMultiplier = 2f;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private CinemachineVirtualCamera _dialogueCamera;
    
    private Vector3 _moveDirection;
    private Animator _animator; 
    private float _defaultSpeed;
    public bool isDialogueOn = false; //대화시작
    private bool isPlayerNearNPC = false;//플레이어 NPC가까이있나? 
    private GameObject _currentNPC;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _defaultSpeed = _moveSpeed;
        InputManager.Instance.moveAction += HandleInput;
        InputManager.Instance.selectBtnAction += OnEKey;
        if (_mainCamera == null)
            _mainCamera = Camera.main;
    }
    
    private void Update()
    {
        _animator.SetFloat("MoveSpeed", 0);
        InputManager.Instance.OnUpdate();
    }

    private void OnEKey()
    {
        if (_currentNPC == null) return;
        
        if (!isDialogueOn && isPlayerNearNPC)
        {
            StartDialogue();
        }
        else if (isDialogueOn) 
        {
            // interact 타입일 때만 처리
            NpcDialogue npcDialogue = _currentNPC.GetComponent<NpcDialogue>();
            if (npcDialogue != null && 
                DataManager.Instance._dialogue[npcDialogue.dialogueId].trigger_type == "interact")
            {
                if (DialogueManager.Instance.isTyping)
                {
                    DialogueManager.Instance.isTyping = false;
                }
                else
                {
                    DialogueManager.Instance.ShowNextLine().Forget();
                }
            }
        }
        
        /*
        if (isDialogueOn && !UIManager.Instance.isPopUpOpen)
        {
            ContinueDialogue();
        }
        else if (!isDialogueOn && isPlayerNearNPC)
        {
            StartDialogue();
        }
        */
    }

    private void StartDialogue()
    {
        //Dialogue Camera On
        // var transposer = _dialogueCamera.GetCinemachineComponent<CinemachineTransposer>();
        // if (transposer != null)
        // {
        //     if (transform.position.x > _currentNPC.transform.position.x)
        //     {
        //         transposer.m_FollowOffset = new Vector3(-2,transposer.m_FollowOffset.y, transposer.m_FollowOffset.z);
        //     }
        //     else
        //     {
        //         transposer.m_FollowOffset = new Vector3(2,transposer.m_FollowOffset.y, transposer.m_FollowOffset.z);
        //     }
        // }
        // _dialogueCamera.LookAt = _currentNPC.transform;
        // _dialogueCamera.Priority = 20;
        
        isDialogueOn = true;
        NpcDialogue npcDialogue = _currentNPC.GetComponent<NpcDialogue>();
        if (npcDialogue != null && !string.IsNullOrEmpty(npcDialogue.dialogueId))
        {
            UIManager.Instance.dialogueUI.gameObject.SetActive(true);
            DialogueManager.Instance.StartDialogue(npcDialogue.dialogueId);
        }
    }

    public void ResetCamera()
    {
        _dialogueCamera.LookAt = null;
        _dialogueCamera.Priority = 0;
    }
    private void HandleInput()
    {
        if (isDialogueOn)
        {
            _animator.SetFloat("MoveSpeed", 0f);
            return;
        }

        float moveX = 0f;
        float moveY = 0f;
        
        // WASD 키 입력 처리
        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;
    
        Vector3 inputDirection = new Vector3(moveX, 0f, moveY).normalized;
        
        if (inputDirection != Vector3.zero)
        {
            Vector3 cameraForward = _mainCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();
            
            Vector3 cameraRight = _mainCamera.transform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();
            
            _moveDirection = (cameraForward * inputDirection.z + cameraRight * inputDirection.x).normalized;
            
            // 이동 속도 설정
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                _moveSpeed = _defaultSpeed * _sprintMultiplier;
            }
            else
            {
                _moveSpeed = _defaultSpeed; 
            }

            Move(_moveDirection);
        }
        else
        {
            _moveDirection = Vector3.zero;
        }

        float currentSpeed = _moveDirection.magnitude * _moveSpeed;
        _animator.SetFloat("MoveSpeed", currentSpeed);
    }
    
    private void Move(Vector3 direction)
    {
        // if (direction != Vector3.zero)
        // {
        //     // 부드러운 회전 구현
        //     transform.rotation = Quaternion.Lerp(transform.rotation, 
        //                                       Quaternion.LookRotation(direction), 
        //                                       Time.deltaTime * 10f);
        //     
        //     // 로컬 좌표계 기준으로 전방 이동
        //     transform.Translate(Vector3.forward * Time.deltaTime * _moveSpeed, Space.Self);
        // }
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            
            transform.Translate(Vector3.forward * Time.deltaTime * _moveSpeed, Space.Self);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isPlayerNearNPC = true;
            _currentNPC = other.gameObject; 
            UIManager.Instance.PopUp(true,"E를 눌러 대화시작");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isPlayerNearNPC = false;
            _currentNPC = null;
            UIManager.Instance.PopUp(false);
        }
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.moveAction -= HandleInput;
            InputManager.Instance.selectBtnAction -= OnEKey;
        }
    }
}