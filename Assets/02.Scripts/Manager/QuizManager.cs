using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : Singleton<QuizManager>
{
    private QuizStructure _curQuiz;
    public InputFieldManager inputFieldManager;
    //퀴즈 설정
    public void SetQuiz(string quizID)
    {
        //quiz type이 input인 경우랑, ui인 경우 나눠서 생각
        _curQuiz = DataManager.Instance._quiz[quizID];

        if (_curQuiz.quizType == "input")
        {
            inputFieldManager.SetQuestionField(quizID);
        }
        else if (_curQuiz.quizType == "ui")
        {
            //현재 씬에서 quizId랑 동일한 이름을 갖고 있는 오브젝트가 있는지 파악
            //찾으면 해당 오브젝트의 UIBase 컴포넌트 가져와서 열기
            GameObject quizObject = GameObject.Find(_curQuiz.quizId);
            UIBase script = quizObject.GetComponent<UIBase>();
            UIManager.Instance.OpenUI(script);
        }
    }
}
