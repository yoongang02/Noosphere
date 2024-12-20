using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

// 플레이어 이동관련 함수
//플레이어 이동방식 변경
// 최초 작성자: 홍원기
// 수정자: 홍원기
// 최종 수정일: 2024-11-01
public class PlayerController : Singleton<PlayerController>
{
    [Header("플레이어 설정")] [SerializeField] private float _moveSpeed;
    [SerializeField] private float _sprintMultiplier = 2f;
    public Camera _mainCamera;
    public Canvas _uiCanvas;
    [SerializeField] private CinemachineVirtualCamera _dialogueCamera;
    [Header("플레이어 사운드")] [SerializeField] private List<AudioClip> _walkSounds;
    [SerializeField] private List<AudioClip> runSounds;
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private float stepInterval = 0.5f;
    [SerializeField] private float runInterval = 0.2f;

    private Vector3 _moveDirection;
    private Animator _animator;
    private float _defaultSpeed;
    public bool canMove = false; //대화시작
    public bool isPlayerNearNPC = false; //플레이어 NPC가까이있나?
    // public bool isNpcRayOn=false;
    public GameObject _currentNPC;
    public GameObject _swapNpc;
    private float lastStepTime = 0f;
    [Header("Ray Settings")]
    [SerializeField] private float _rayDistance;
    [SerializeField] private float _rayHeight;

    private GameObject npcCam;
    public NpcState npcState;
    private void OnDrawGizmos()
    {
        Vector3 rayStart = transform.position + Vector3.up * _rayHeight; // Ray 시작점을 위로 올림
        Gizmos.color = Color.red;
        Vector3 direction = transform.forward * _rayDistance;
        Gizmos.DrawRay(rayStart, direction);
    }
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _defaultSpeed = _moveSpeed;
        InputManager.Instance.moveAction += HandleInput;
        // InputManager.Instance.selectBtnAction += OnEKey;
        if (_mainCamera == null)
            _mainCamera = Camera.main;
    }

    private void Update()
    {
        _animator.SetFloat("MoveSpeed", 0);
        InputManager.Instance.OnUpdate();
    }

    public void SetInteract(bool state)
    {
        canMove = !state;
        _animator.SetBool("Interact", state);
    }
    public void NpcCameraOn()
    {
        // _currentNPC.GetComponent<NpcState>().SetState(NPCState.IsTalking);
        npcState.SetState(NPCState.IsTalking);
        _dialogueCamera.transform.gameObject.SetActive(true);
        _dialogueCamera.Follow = npcCam.transform;

    }

    public void ResetCamera()
    {
        if(npcState==null) return;
        npcState.SetState(NPCState.Idle);
        // _currentNPC.GetComponent<NpcState>().SetState(NPCState.Idle);
        // _dialogueCamera.Follow = null;
        _dialogueCamera.transform.gameObject.SetActive(false);
        // _dialogueCamera.Follow = null;
    } 
    private void HandleInput()
    {
        if (!canMove)
        {
            _animator.SetFloat("MoveSpeed", 0f);
            return;
        }
        // Vector3 rayStart = transform.position + Vector3.up * _rayHeight; // Ray 시작점을 위로 올림
        // Ray ray = new Ray(rayStart, transform.forward);
        // RaycastHit hit;
        // if(Physics.Raycast(ray, out hit, _rayDistance))
        // {
        //     Debug.DrawRay(rayStart, transform.forward * _rayDistance, Color.green);
        //     if(hit.collider.CompareTag("NPC"))
        //     {
        //         // isPlayerNearNPC = true;
        //         isNpcRayOn = true;
        //         _currentNPC = hit.collider.gameObject;
        //         Debug.Log("NPC를 바라보고 있습니다.");
        //     }
        // }
        // else
        // {
        //     // isPlayerNearNPC = false;
        //     isNpcRayOn = false;
        //     Debug.DrawRay(rayStart, transform.forward * _rayDistance, Color.red);
        // }
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
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);

            transform.Translate(Vector3.forward * Time.deltaTime * _moveSpeed, Space.Self);
            bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            float currentInterval = isRunning ? runInterval : stepInterval;
            List<AudioClip> currentSounds = isRunning ? runSounds : _walkSounds;

            if (Time.time >= lastStepTime + currentInterval)
            {
                int randomIndex = UnityEngine.Random.Range(0, currentSounds.Count);
                footstepSource.clip = currentSounds[randomIndex];
                footstepSource.Play();
                lastStepTime = Time.time;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if (other.CompareTag("NPC"))
        // {
        //     isPlayerNearNPC = true;
        //     _currentNPC = other.gameObject;
        //     _swapNpc = other.gameObject;
        //     // UIManager.Instance.PopUp(true, "E를e 눌러 대화시작");
        // }
        if (other.CompareTag("EventInteractionTrigger"))
        {
            EventTrigger eventTrigger = other.GetComponent<EventTrigger>();
            if (eventTrigger.isNpc)
            {
                npcCam = eventTrigger.npcCameraPoint;
                npcState = other.GetComponent<NpcState>();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
    
        // if (other.CompareTag("NPC"))
        // {
        //     isPlayerNearNPC = false;
        //     _currentNPC = null;
        //     // UIManager.Instance.PopUp(false);
        // }
        if (other.CompareTag("EventInteractionTrigger"))
        {
            npcCam = null;
            npcState = null;
        }
    }
    public void ResetAndSetupTrigger()
    {
        isPlayerNearNPC = false;
        _currentNPC = null;
    
        if (_swapNpc != null)
        {
            isPlayerNearNPC = true;
            _currentNPC = _swapNpc;
        }
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.moveAction -= HandleInput;
            // InputManager.Instance.selectBtnAction -= OnEKey;
        }
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignMainCamera();
    }

    public void AssignMainCamera()
    {
        _mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        if (_mainCamera != null)
        {
            Debug.Log($"Main camera assigned: {_mainCamera.name}");
        }
        else
        {
            Debug.LogWarning("No main camera found in the current scene.");
        }

        if(_uiCanvas.renderMode == RenderMode.ScreenSpaceCamera) _uiCanvas.worldCamera = _mainCamera;
        
        GameObject parent = GameObject.Find("-----[Cameras]");
        //dialogue camera 찾기
        foreach (Transform child in parent.transform)
        {
            if (child.name == "Dialogue Camera") 
            {
                _dialogueCamera = child.GetComponent<CinemachineVirtualCamera>();
            }
            else if (child.name == "Virtual Camera")
            {
                CinemachineVirtualCamera abcCamera = child.GetComponent<CinemachineVirtualCamera>();
                if (abcCamera != null)
                {
                    abcCamera.LookAt = transform;
                    Debug.Log("Set ABCCamera LookAt to player");
                }
            }
        }
    }
}