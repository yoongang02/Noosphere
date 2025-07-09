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
            string location = ES3.Load<string>("Location",filePath);
            _locationTexts[slotIndex - 1].text = "저장 위치 : " + location;
        }

        if (ES3.KeyExists("DateTime", filePath))
        {
            string dateTime = ES3.Load<string>("DateTime", filePath);
            _dateTimeTexts[slotIndex - 1].text = "저장 일시 : " + dateTime;
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
