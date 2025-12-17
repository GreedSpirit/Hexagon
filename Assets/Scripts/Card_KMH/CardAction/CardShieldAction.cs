using UnityEngine;

public class CardShieldAction : ICardAction
{
    // 카드 사용
    public void Use(CardData data, int value, IBattleUnit target)
    {
        Debug.Log($"방어 카드 사용 : 보호막 {value} 생성.");
    }
}
