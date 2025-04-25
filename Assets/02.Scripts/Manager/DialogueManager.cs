using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class DialogueManager : UIBase
{
    //싱글톤
    private static DialogueManager _instance;
    
    public static DialogueManager Instance 
    { 
        get 
        { 
            if (_instance == null) 
            {
                _instance = FindObjectOfType<DialogueManager>();
                if (_instance == null) 
                {
                    GameObject singletonObject = new GameObject(nameof(DialogueManager));
                    _instance = singletonObject.AddComponent<DialogueManager>();
                }
            }
            return _instance;
        } 
    }

    void Awake(){
        
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); 
        contentSizeFitter = dialogueText.gameObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        textRectTransform = dialogueText.GetComponent<RectTransform>();
        textRectTransform.pivot = new Vector2(0, 0.5f); //
    }

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject _dialogueGroup;
    [SerializeField] private TextMeshProUGUI _speakerText;

    private DialogueStructure _curDialogue;
    
    private string _currentDialogueId = "";
    private int _currentLineIndex = 0;
    private string _initialDialogueId = "";

    [SerializeField] float _letterDelay = 0.05f;
    private float _currentTextElapsedTime = 0f;
    private int _currentLetterIndex = 0;
    public bool isTyping = false;
    public Action OnDialogueEnd;
    private ContentSizeFitter contentSizeFitter;
    private RectTransform textRectTransform;


    public override void OnOpen()
    {
        base.OnOpen();
        _dialogueGroup.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        _dialogueGroup.SetActive(false);
        if (_curDialogue.triggerType == "interact" && _curDialogue.interactionType == "npc")
        {
            PlayerController.Instance.ResetCamera();
        }
    }

    public void SetDialogue(string id)
    {
        InitDialogue();
        if (!string.IsNullOrEmpty(id) && DataManager.Instance._dialogue.ContainsKey(id))
        {
            _curDialogue = DataManager.Instance._dialogue[id];
            _currentDialogueId = id;
            EscapeUI.Instance.DisActive();
            UIManager.Instance.OpenUI(UIManager.Instance.dialogueUI);
            
            if (_curDialogue.triggerType == "interact" && _curDialogue.interactionType == "npc")
            {
                PlayerController.Instance.NpcCameraOn();
            }
            ShowNextLine().Forget();
        }
        else
        {
            Debug.LogWarning($" {_currentDialogueId} 못찾음");
        }
    }
    public override void HandleKeyboardInput()
    {
        if ((InputRouter.Instance.ConsumeSpace() || Input.GetMouseButtonDown(0)) && !string.IsNullOrEmpty(_currentDialogueId))
        {
            if (isTyping)
            {
                isTyping = false;
            }
            else
            {
                ShowNextLine().Forget();
            }
        }
    }
    public async UniTask ShowNextLine()
    {
        // _speakerText.text = _curDialogue.characterId;
       if (_currentLineIndex < _curDialogue.Dialogue_Text_List.Count)
        {
            //튜토리얼 있으면 실행
            TutorialManager.Instance.ShowTutorial(_curDialogue.Dialogue_Text_List[_currentLineIndex].tutorialID);
            // 타이핑 시작
            _speakerText.text = _curDialogue.Dialogue_Text_List[_currentLineIndex].characterId;
            await TypeText(_curDialogue.Dialogue_Text_List[_currentLineIndex].text);
            // 타이핑이 완료되었거나 스킵되었을 때만 다음 라인으로 진행
            _currentLineIndex++;
        }
        else
        { 
            if (!string.IsNullOrEmpty(_curDialogue.nextDialougeId))
            {
                if (DataManager.Instance._dialogue.ContainsKey(_curDialogue.nextDialougeId))
                {
                    _currentDialogueId = _curDialogue.nextDialougeId;
                    _curDialogue = DataManager.Instance._dialogue[_currentDialogueId];
                    _currentLineIndex = 0;
                    ShowNextLine().Forget();
                }
                else
                {
                    Debug.LogWarning($"Next Dialogue ID {_curDialogue.nextDialougeId} not found.");
                }
            }
            else
            {
                Debug.LogWarning("대화가 종료되었습니다.");
                UIManager.Instance.CloseTopUI();
                
                //예외 이벤트 처리 코드
                if (EventManagerYKM.Instance.currentEventID == "Event_A004")
                {
                    GameObject.Find("Artresource_0002").transform.GetChild(0).gameObject.SetActive(false);
                }
                OnDialogueEnd?.Invoke();
                
                //현재 위치한 곳에 트리거가 있다면 해당 트리거 실행 가능한지 다시 체크
                if (PlayerInteract.Instance.isInsideTrigger && PlayerInteract.Instance.curTrigger != null)
                {
                    await Task.Delay(100);
                    if (PlayerInteract.Instance.isInsideTrigger && PlayerInteract.Instance.curTrigger != null)
                    {
                        if(_curDialogue.dialogueId == "Dialogue_0065") return;
                        PlayerInteract.Instance.curTrigger.OnTriggerEnter(PlayerInteract.Instance.GetComponent<CapsuleCollider>());
                    }
                }
            }
        }
    }
    
    private async UniTask TypeText(string text)
    {
        isTyping = true;
    
        // 임시로 텍스트 알파값을 0으로 설정
        dialogueText.alpha = 0;
    
        // 전체 텍스트 길이 계산을 위해 텍스트 설정
        text = text.Replace("\\n", "\n");
        dialogueText.text = text;
        await UniTask.NextFrame();
    
        float totalWidth = dialogueText.preferredWidth;
    
        Canvas canvas = dialogueText.transform.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float canvasCenter = canvasRect.rect.width / 2f;
        float startX = canvasCenter - (totalWidth / 2f);
        textRectTransform.anchoredPosition = new Vector2(startX, textRectTransform.anchoredPosition.y);
    
        // 텍스트 다시 비우고 알파값 복구
        dialogueText.text = "";
        dialogueText.alpha = 1;
        StartDialogueSound(_curDialogue.Dialogue_Text_List[_currentLineIndex].characterId);
        if (!isTyping)
        {
            dialogueText.text = text;
            return;
        }

        for (int i = 0; i < text.Length && isTyping; i++)
        {
            dialogueText.text += text[i];
            await UniTask.Delay((int)(_letterDelay * 1000));
        }

        dialogueText.text = text;
        isTyping = false;
        SoundManager.Instance.StopAllSFX();
    }

    void InitDialogue()
    {
        _currentDialogueId = "";
        _currentLineIndex = 0;
        _curDialogue = null;
        PlayerController.Instance.ResetCamera();
    }

    private void StartDialogueSound(string speaker)
    {
        switch (speaker)
        {
            case "(나)":
                SoundManager.Instance.PlayLoopingSound("Soundresource_026"); 
                break;
            case "(닥터 로만)" :
                 SoundManager.Instance.PlayLoopingSound("Soundresource_027");
                 break;
            case "(레이)" :
                SoundManager.Instance.PlayLoopingSound("Soundresource_077");
                break;
            case "(다프네)" :
                SoundManager.Instance.PlayLoopingSound("Soundresource_076");
                break;
            case "" :
                SoundManager.Instance.PlayLoopingSound("Soundresource_075");
                break;
        }
    }
    public string GetCurDialogueId()
    {
        return _currentDialogueId;
    }
}

