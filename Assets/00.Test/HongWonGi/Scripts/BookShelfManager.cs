using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookShelfManager : Singleton<BookShelfManager>
{
    public bool isBookClear=false;
    [SerializeField] private List<int> _answer; // 정답 순서
    [SerializeField] private Transform _bookParent;
    [SerializeField] private GameObject _bookShelfPanel;
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
            _bookShelfPanel.SetActive(false);
            MirrorPuzzleManager.Instance.GetMirrorPiece(1);
            //거울조각 얻기
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
