using UnityEngine;

public class CardSpellAction : ICardAction
{
    public void Use(StatusEffectData statusData, int value, IBattleUnit target)
    {
        Debug.Log($"주문 카드 사용");
    }
}
