using UnityEngine;

public class CardSpellAction : ICardAction
{
    public void Use(StatusEffectData statusData, int value, int statusValue, int turn, IBattleUnit target)
    {
        Debug.Log($"주문 카드 사용");
    }
}
