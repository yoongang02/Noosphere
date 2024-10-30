using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 플레이어 이동관련 함수
//InputManager사용
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
    private InputManager _inputManager = new InputManager();
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _defaultSpeed = _moveSpeed;
        _inputManager.moveAction += HandleInput;
        _inputManager.exitBtnAction += EndDialogue;
        _inputManager.selectBtnAction += OnEKey;
    }
    
    private void Update()
    {
        _inputManager.OnUpdate();
    }
    private void OnEKey()
    {
        if (_currentNPC == null) return;
        if (isDialogueOn && !UIManager.Instance.isPopUpOpen)
        {
            ContinueDialogue();
        }
        else if (!isDialogueOn && isPlayerNearNPC)
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
        if (!isDialogueOn) return;
        NpcDialogue npcDialogue = _currentNPC.GetComponent<NpcDialogue>();
        if (npcDialogue != null)
        {
            npcDialogue.AdvanceDialogue();
            if (!npcDialogue.isOnDialogue) 
            {
                UIManager2.Instance.dialogueUI.gameObject.SetActive(false);
                isDialogueOn = false;
            }
        }
    }
    private void EndDialogue()
    {
        if (!isDialogueOn) return;
        isDialogueOn = false;
        UIManager2.Instance.dialogueUI.gameObject.SetActive(false);
        if (_currentNPC != null)
        {
            NpcDialogue npcDialogue = _currentNPC.GetComponent<NpcDialogue>();
            if (npcDialogue != null)
            {
                npcDialogue.ResetDialogue();
            }
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
            Move(_moveDirection);
        }
    }
    
    private void Move(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
        transform.Translate(Vector3.forward * Time.deltaTime * _moveSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isPlayerNearNPC = true;
            _currentNPC = other.gameObject; 
            UIManager2.Instance.popUpUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isPlayerNearNPC = false;
            _currentNPC = null;
            UIManager2.Instance.popUpUI.SetActive(false);
        }
    }
}