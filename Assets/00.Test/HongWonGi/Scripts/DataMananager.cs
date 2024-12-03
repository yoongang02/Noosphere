using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataManager : Singleton<DataManager>
{
    public Dictionary<string, EventStructure> _events = new Dictionary<string, EventStructure>();
    public Dictionary<string, LockConditionStructure> _lockConditions = new Dictionary<string, LockConditionStructure>();
    public Dictionary<string, EvidenceStructure> _evidences = new Dictionary<string, EvidenceStructure>();
    public Dictionary<string, ArtResourceStructure> _artResources = new Dictionary<string, ArtResourceStructure>();
    public Dictionary<string, DialogueStructure> _dialogue = new Dictionary<string, DialogueStructure>();
    public Dictionary<string, InputFieldStructure> _input = new Dictionary<string, InputFieldStructure>();
    public Dictionary<string, EffectStructure> _effect = new Dictionary<string, EffectStructure>();
    public Dictionary<string, SoundResourceStructure> _sound = new Dictionary<string, SoundResourceStructure>();

    public async UniTask InitializeData(Action<float> onProgressUpdated = null)
    {
        List<Func<UniTask>> loadSteps = new List<Func<UniTask>>()
        {
            async () => { _events = await LoadData<EventStructure>("Event"); },
            async () => { _lockConditions = await LoadData<LockConditionStructure>("Lock_condition"); },
            async () => { _evidences = await LoadData<EvidenceStructure>("Evidence"); },
            async () => { _artResources = await LoadData<ArtResourceStructure>("ArtResource"); },
            async () => { _dialogue = await LoadDialogueData(); },
            async () => { _input = await LoadData<InputFieldStructure>("Input"); },
            async () => { _effect = await LoadData<EffectStructure>("Effect"); },
            async () => { _sound = await LoadData<SoundResourceStructure>("SoundResource"); }
        };

        int totalSteps = loadSteps.Count;
        int currentStep = 0;

        foreach (var loadStep in loadSteps)
        {
            await loadStep.Invoke();
            currentStep++;
            onProgressUpdated?.Invoke((float)currentStep / totalSteps);
        }
        Debug.Log("끝?");
    }
    public async UniTask<Dictionary<string, T>> LoadData<T>(string SheetName) where T : new()
    {
        CSVParserYKM parser = new CSVParserYKM();
        return await parser.Parse<T>(SheetName);
    }
    
    private async UniTask<Dictionary<string, DialogueStructure>> LoadDialogueData()
    {
        DialogueCSVParser parser = new DialogueCSVParser();
        return await parser.Parse("DialogueTest");
       
    }

}
