using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EvidenceDetailOnePageNavigator : MonoBehaviour
{
    public int curPage = 0;
    public List<Sprite> detailImgs = new List<Sprite>();
    public int pageCnt = 0;
    public Image page;
    public string evidence_id;
    public EvidenceStructure evidenceStructure;
    void Start()
    {
        curPage = 0;
        pageCnt = detailImgs.Count;
        page.sprite = detailImgs[0];
        evidenceStructure = DataManager.Instance._evidences[evidence_id];
    }

    private void OnEnable()
    {
        curPage = 0;
        pageCnt = detailImgs.Count;
        page.sprite = detailImgs[0];
        evidenceStructure = DataManager.Instance._evidences[evidence_id];
    }

    void Update()
    {
        //증거물 상세내용 UI가 열려있고, 텍스트 상세내용이라면 페이지 버튼 활성화
        if (UIManager.Instance._isDetailOpen)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) && curPage != 0)
            {
                curPage -= 1;
                UpdateUI();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) && (curPage != pageCnt-1))
            {
                curPage += 1;
                UpdateUI();
                
                
                //현재 상세 내용을 보고 있는 증거물에 서브 증거물이 존재한다면, 그리고 그 서브 증거물의 타입이 page라면
                if (evidenceStructure != null)
                {
                    if (!String.IsNullOrEmpty(evidenceStructure.sub_evidence_id) &&
                        evidenceStructure.sub_Evidence_Acquisition_Type == "page")
                    {
                        //page가 제한조건 이상이 되었다면
                        int limitPage = evidenceStructure.acquisition_Page_Num;
                        if (limitPage >= curPage * 2 && limitPage <= curPage * 2 + 1)
                        {
                            if (DataManager.Instance._evidences.ContainsKey(evidenceStructure.sub_evidence_id))
                            {
                                Debug.Log(evidenceStructure.sub_evidence_id + " 숨겨져 있던 증거물 발견!!");
                                //해당 증거물 접근 횟수 증가
                                DataManager.Instance._evidences[evidenceStructure.sub_evidence_id].accessCnt++;
                                //강제 종료 코루틴 호출
                                CoroutineManager.Instance.StartManagedCoroutine(UIManager.Instance.ForceQuitInteraction(evidenceStructure));
                            }
                        }
                    }
                }
            }
            

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("상세 내용 창만 닫기");
                UIManager.Instance.CloseDetailEvidence();
            }
        }
    }

    void UpdateUI()
    {
        page.sprite = detailImgs[curPage];
    }
}
