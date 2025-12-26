using UnityEngine;

[CreateAssetMenu(fileName = "newHealingAction", menuName = "Card Actions/Healing Action")]
public class CardHealingAction : CardAction
{
    // 카드 사용
    public override void Use(string statusEffectKey, int value, IBattleUnit target)
    {
        base.Use(statusEffectKey, value, target);
        Debug.Log($"치유 카드 사용 : {value} 회복.");
        target.GetHp(value);
    }
}
