using UnityEngine;

public class CardAttackAction : ICardAction
{
    // 카드 사용
    public void Use(CardData data, int value)
    {
        Debug.Log($"공격 카드 사용 : {value} 피해.");
    }
}
