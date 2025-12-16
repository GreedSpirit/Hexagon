using UnityEngine;

public class CardSpellAction : ICardAction
{
    public void Use(CardData data, int value)
    {
        Debug.Log($"주문 카드 사용");
    }
}
