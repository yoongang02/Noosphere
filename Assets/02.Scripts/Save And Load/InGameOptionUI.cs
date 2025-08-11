using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class InGameOptionUI : DefaultUIBase
{
    [SerializeField] private GameObject firstSelectable;
    public override void OnOpen()
    {
        base.OnOpen();

        // 게임 일시 정지
        Time.timeScale = 0f;

        EventSystem.current.firstSelectedGameObject = firstSelectable;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();

        // 게임 일시 정지 해제
        Time.timeScale = 1f;

        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void HandleKeyboardInput()
    {
        base.HandleKeyboardInput();
    }
    public void OnClickResume()
    {
        DefaultUIController.Instance.CloseAllUI();
    }

    public void OnClickSave()
    {
        DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.saveUI);
    }

    public void OnClickOption()
    {
        DefaultUIController.Instance.OpenUI(DefaultUIController.Instance.settingUI);
    }

    public void OnClickStartScene()
    {
        DefaultUIController.Instance.CloseTopUI();

        //dontdestroyonload에서 사운드매니저하고 dotween만 남겨놓기
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.name == "DontDestroyOnLoad" &&
                obj.GetComponent<SoundManager>() == null &&
                !obj.name.Contains("[DOTween]") && !obj.name.Contains("SceneChanger"))
            {
                Destroy(obj);
            }
        }

        SceneChanger.Instance.ChangeScene("StartScene").Forget();
    }
}
