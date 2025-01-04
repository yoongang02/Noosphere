using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookShelfManager : UIBase
{
    [SerializeField] private string _curQuizID;
    private QuizStructure _curQuiz;
    public bool isBookClear=false;
    [SerializeField] private List<int> _answer; // 정답 순서
    [SerializeField] private Transform _bookParent;

    public override void OnOpen(string quizID)
    {
        base.OnOpen(quizID);
        _curQuizID = quizID;
        _curQuiz = DataManager.Instance._quiz[_curQuizID];
        Debug.Log($"# quiz id : {quizID}, _curQuiz : {_curQuiz}");
        
        //만약 거울이 깨졌는데 기믹을 미리 성공했다면
        if (_curQuiz.isSolved)
        {
            //퀴즈 실행되지 않음
            UIManager.Instance.CloseTopUI();
            QuizManager.Instance.OnQuizEnd?.Invoke();
            return;
        }
        
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        transform.GetChild(0).gameObject.SetActive(false);
        
        _curQuizID = "";
        _curQuiz = null;
    }
    
    public void CheckBookOrder()
    {
        List<int> currentOrder = new List<int>();
        
        foreach(Transform child in _bookParent)
        {
            // 활성화된 오브젝트이고 BookDrag 컴포넌트가 있는 경우만 체크
            if(child.gameObject.activeSelf && child.TryGetComponent<BookDrag>(out BookDrag book))
            {
                currentOrder.Add(book.bookIdx);
            }
        }

        
        // 정답 체크
        if(IsCorrectOrder(currentOrder))
        {
            //퀴즈 해결되었다고 표시
            _curQuiz.isSolved = true;
            UIManager.Instance.CloseTopUI();
            QuizManager.Instance.OnQuizEnd?.Invoke();
        }
    }

    private bool IsCorrectOrder(List<int> currentOrder)
    {
        if(currentOrder.Count != _answer.Count) 
            return false;

        for(int i = 0; i < currentOrder.Count; i++)
        {
            if(currentOrder[i] != _answer[i])
                return false;
        }
        return true;
    }
}
