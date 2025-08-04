using System.Collections;
using System.Collections.Generic;
using NooSphere;
using UnityEngine;

public class ArtResource0080 : MonoBehaviour
{
    void Start()
    {
        if (NooSphere.SaveManager.Instance.CurrentLoadType == GameLoadType.ContinueGame)
        {
            Destroy(this.gameObject);
        }
        
        // 이거를 그 프롤로그에 검정 화면 밝아지고 플레이어 애니메이션 되는 스크립트에 부착했거든?? 이어하기면 그거 파괴하는 걸로
    }
}
