using UnityEngine;

[CreateAssetMenu(fileName = "newSpellAction", menuName = "Card Actions/Spell Action")]
public class CardSpellAction : CardAction
{
    public override void Use(int value, IBattleUnit target)
    {
        Debug.Log($"주문 카드 사용");
    }
}
