using UnityEngine;

public class CardHealingAction : ICardAction
{
    // 카드 사용
    public void Use(CardData data, int value, IBattleUnit target)
    {
        Debug.Log($"치유 카드 사용 : {value} 회복.");
    }
}
