using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public int snapOffsst = 30;
    public GameObject piecePos;
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(Vector3.Distance(piecePos.transform.position, transform.position) < snapOffsst)
        {
            transform.SetParent(piecePos.transform);
            transform.localPosition = Vector3.zero;
        }
    }
}