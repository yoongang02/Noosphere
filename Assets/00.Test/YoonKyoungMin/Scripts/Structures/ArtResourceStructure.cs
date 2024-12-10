using UnityEngine;

public class ArtResourceStructure
{
    public string artresourceId;
    public string artresourceName;
    public string artresourceType;
    public string filePath;
    public string inventoryFilePath;
    public string evidenceDetailPrefab;
    public string mapBackgroundImg;
    public string textDetailStartImg;
    public int textDetailImgCnt;

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
        GameObject resultGameObject = Resources.Load<GameObject>(evidenceDetailPrefab);

        if (resultGameObject == null)
        {
            Debug.Log(evidenceDetailPrefab + "에 프리팹이 존재하지 않습니다.");
            return null;
        }

        return resultGameObject;
    }
}