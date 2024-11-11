using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

class DialogueStructure
{
    public string dialogue_id;
    public string character_id;
    private string dialogue_Text;
    public string next_dialouge_id;

    // 리스트로 분리된 대사
    public List<string> Dialogue_Text_List
    {
        get
        {
            return new List<string>(dialogue_Text.Split('/'));
        }
    }
}

public class DialougueManager : MonoBehaviour
{
    private Dictionary<string, DialogueStructure> _dialogue = new Dictionary<string, DialogueStructure>();
    void Start()
    {
        LoadDialogue("Dialogue").Forget();
    }

    //이벤트 로드
    private async UniTask LoadDialogue(string fileName)
    {
        CSVParserYKM parser = new CSVParserYKM();
        _dialogue = await parser.Parse<DialogueStructure>(fileName);
    }

}
