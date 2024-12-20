using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlyForTest : MonoBehaviour
{
    //스테이지1 빠른 테스트를 위한 임시 마스터 코드

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("종료");
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("스테이지1로 바로 이동");
            GoToStage1();
        }
    }
    
    void GoToStage1()
    {
        //이벤트 모두 실행
        foreach (var _event in DataManager.Instance._events)
        {
            string chapterIndex = _event.Key;
            if (chapterIndex[6] == 'A')
            {
                _event.Value.isExecuted = true;
                //증거물 모두 수집
                if (!string.IsNullOrEmpty(_event.Value.evidenceId))
                {
                    EvidenceStructure evidence = DataManager.Instance._evidences[_event.Value.evidenceId];

                    if (evidence.acquisitionType == 'Y')
                    {
                        InventoryManager.Instance.AddEvidence(evidence);
                    }
                    else if (evidence.acquisitionType == 'N')
                    {
                        evidence.accessCnt = 3;
                    }
                }
            }
        }
        
        //퀴즈 모두 정답
        DataManager.Instance._quiz["Quiz_001"].isSolved = true;
        //서브 증거물
        DataManager.Instance._evidences["Evidence_008"].accessCnt = 3;
        
        UIManager.Instance.CloseAllUI();
        
        //씬 스테이지1로 이동
        SceneManager.LoadScene("Stage1Map_real");
        //현재 스테이지 변경
        EventManagerYKM.Instance.curStageInfo = EventManagerYKM.ChapterInfo.Stage1;
        EventManagerYKM.Instance.currentEventID = "Event_A031";
        EventManagerYKM.Instance.nextEventID = "";
    }
}
