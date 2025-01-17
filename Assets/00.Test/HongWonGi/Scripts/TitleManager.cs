using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TitleManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _objects;
    [SerializeField] private AudioSource _bgmSource;
    private void Start()
    {
        ActiveRandomMenu();
        _bgmSource.Play();
    }

    private void ActiveRandomMenu()
    {
        int randomValue = Random.Range(0, 2);
        // 랜덤 값에 따라 하나만 활성화
        if (randomValue == 0)
        {
            _objects[0].SetActive(true);
        }
        else
        {
            _objects[1].SetActive(true);
        }
    }
}
