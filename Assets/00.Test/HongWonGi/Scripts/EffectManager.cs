using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;


public class EffectManager : Singleton<EffectManager>
{
    public Dictionary<string, EffectStructure> _effect = new Dictionary<string, EffectStructure>();
    private string _currentEffectID;
    private void Start()
    {
        InitializeDialogue().Forget();
    }
    private async UniTaskVoid InitializeDialogue()
    {
        await LoadDialogue("Effect");
    }

    private async UniTask LoadDialogue(string sheetName)
    {
        CSVParserYKM parser = new CSVParserYKM();
        _effect = await parser.Parse<EffectStructure>(sheetName);
        Debug.Log("이펙트 로드 완료");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SceneManager.LoadScene("map1");
        }
    }

    public void SetEffect(string id)
    {
        if (_effect.TryGetValue(_currentEffectID, out EffectStructure effect))
        {
            // if (effect.trigger_type == "auto") //대화창 바로 뜨기
            // {
            //     PlayerController.Instance.isDialogueOn = true;
            //     UIManager.Instance.dialogueUI.gameObject.SetActive(true);
            //     ShowNextLine().Forget();
            // }
            //
            // if (dialogue.trigger_type == "interact")
            // {
            //     SetInteractDialogue(dialogue.interaction_type);
            // }
            
        }
    }
}
