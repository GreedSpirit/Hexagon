using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SpriteDatabase", menuName = "Data/SpriteDatabase")]
public class SpriteDatabase : ScriptableObject
{
    public List<Sprite> allSprites = new List<Sprite>();

    // 이름을 넣으면 해당 스프라이트를 찾아서 주는 함수
    public Sprite GetSprite(string spriteName)
    {
        // 리스트를 뒤져서 이름이 같은걸 찾음
        var foundSprite = allSprites.Find(s => s.name == spriteName);

        if (foundSprite != null) return foundSprite;

        Debug.LogWarning($"[SpriteDB] '{spriteName}' 이미지를 찾을 수 없습니다.");
        return null;
    }
}