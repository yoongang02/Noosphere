using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TitleManager : MonoBehaviour
{
    [SerializeField] private List<DefaultUIBase> _UIs;

    private void Start()
    {
        PlayerPrefs.SetInt("FadeImage", 0);
        ActiveRandomMenu();
        Init();
        // Screen.SetResolution(1920,1080,true);
    }

    private void Init()
    {
        int width, height;
        bool isFullScreen = true;

        if (ES3.KeyExists("ScreenMode", "Setting.es3"))
        {
            isFullScreen = ES3.Load<bool>("ScreenMode", "Setting.es3");
        }
        if (isFullScreen)
        {
            width = 1920;
            height = 1080;
        }
        else
        {
            width = 1280;
            height = 720;
        }

        Screen.SetResolution(width, height, isFullScreen);
    }

    private void ActiveRandomMenu()
    {
        int randomValue = Random.Range(0, 2);
        // 랜덤 값에 따라 하나만 활성화
        if (randomValue == 0)
        {
            DefaultUIController.Instance.OpenUI(_UIs[0]);
        }
        else
        {
            DefaultUIController.Instance.OpenUI(_UIs[1]);
        }
    }
}
