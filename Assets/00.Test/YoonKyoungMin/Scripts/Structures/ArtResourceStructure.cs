using UnityEngine;

public class ArtResourceStructure
{
    public string artresource_id;
    public string artresource_Name;
    public string artresource_Type;
    public string FilePath;

    public Sprite GetSpriteFromFilePath()
    {
        Sprite resultSprite = Resources.Load<Sprite>(FilePath);

        if (resultSprite == null)
        {
            Debug.Log(FilePath + "에 이미지 리소스가 존재하지 않습니다.");
            return null;
        }

        return resultSprite;
    }
}