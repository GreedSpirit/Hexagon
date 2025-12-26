using UnityEngine;

[CreateAssetMenu(fileName = "newSpellAction", menuName = "Card Actions/Spell Action")]
public class CardSpellAction : CardAction
{
    public override void Use(string statusEffectKey, int statusValue, int turn, IBattleUnit target)
    {
        Debug.Log($"주문 카드 사용");
    }
}
