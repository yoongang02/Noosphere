using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class SaveUI : Singleton<SaveUI>
{
    [SerializeField] GameObject _loadingUI;
    [SerializeField] List<TextMeshProUGUI> _locationTexts;
    [SerializeField] List<TextMeshProUGUI> _playTimeTexts;
    [SerializeField] List<TextMeshProUGUI> _dateTimeTexts;
    void OnEnable()
    {
        NooSphere.SaveManager.Instance.OnSaveStart += HandleSaveStart;
        NooSphere.SaveManager.Instance.OnSaveFinish += HandleSaveFinish;
    }

    void OnDisable()
    {
        NooSphere.SaveManager.Instance.OnSaveStart -= HandleSaveStart;
        NooSphere.SaveManager.Instance.OnSaveFinish -= HandleSaveFinish;
    }

    void HandleSaveStart()
    {
        _loadingUI.SetActive(true);
    }

    void HandleSaveFinish(int slotIndex)
    {
        _loadingUI.SetActive(false);
        UpdateSlotUI(slotIndex);
    }

    void UpdateSlotUI(int slotIndex) {

        string folderPath = Path.Combine(Application.persistentDataPath, "Saves");
        string filePath = Path.Combine(folderPath, $"slot{slotIndex}.es3");

        if (ES3.KeyExists("Location", filePath))
        {
            string location = NooSphere.SaveManager.Instance.GetLocationData(slotIndex);
            _locationTexts[slotIndex - 1].text = "저장 위치 : " + location;
        }

        if (ES3.KeyExists("DateTime", filePath))
        {
            string dateTime = NooSphere.SaveManager.Instance.GetDateTimeData(slotIndex);
            _dateTimeTexts[slotIndex - 1].text = "저장 일시 : " + dateTime;
        }
        
        if (ES3.KeyExists("PlayTime", filePath))
        {
            string playTime = NooSphere.SaveManager.Instance.GetPlayTimeData(slotIndex);
            _playTimeTexts[slotIndex - 1].text = "플레이 타임 : " + playTime;
        }
    }

    public void ClickSaveBtn(int slotIndex)
    {
        NooSphere.SaveManager.Instance.SetSlotIndex(slotIndex);
        StartCoroutine(NooSphere.SaveManager.Instance.DoSave());
    }

    public void ClickLoadBtn(int slotIndex)
    {

    }
}
