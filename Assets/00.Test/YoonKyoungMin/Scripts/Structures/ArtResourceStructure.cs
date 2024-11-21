using UnityEngine;

public class ArtResourceStructure
{
    public string artresource_id;
    public string artresource_Name;
    public string artresource_Type;
    public string filePath;
    public string inventoryFilePath;
    public string evidence_detail_prefab;
    public string map_background_img;
    public string text_detail_start_img;
    public int text_detail_img_cnt;
    public string prefab_grab_for_use;

    public Sprite GetSpriteFromFilePath(string path)
    {
        Sprite resultSprite = Resources.Load<Sprite>(path);

        if (resultSprite == null)
        {
            Debug.Log(path + "에 이미지 리소스가 존재하지 않습니다.");
            return null;
        }

        return resultSprite;
    }

    public GameObject GetPrefabFromFilePath()
    {
        GameObject resultGameObject = Resources.Load<GameObject>(evidence_detail_prefab);

        if (resultGameObject == null)
        {
            Debug.Log(evidence_detail_prefab + "에 프리팹이 존재하지 않습니다.");
            return null;
        }

        return resultGameObject;
    }

    public GameObject GetPrefabForGrab()
    {
        GameObject resultGameObject = Resources.Load<GameObject>(prefab_grab_for_use);

        if (resultGameObject == null)
        {
            Debug.Log(evidence_detail_prefab + "에 프리팹이 존재하지 않습니다.");
            return null;
        }

        return resultGameObject;
    }
    
    
}