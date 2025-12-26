using UnityEngine;

[CreateAssetMenu(fileName = "newAttackAction", menuName = "Card Actions/Attack Action")]
public class CardAttackAction : CardAction
{
    // 카드 사용
    public override void Use(int value, IBattleUnit target)
    {
        base.Use(value, target);
        Debug.Log($"공격 카드 사용 : {value} 피해.");
        target.TakeDamage(value);
    }
}
