using UnityEngine;

[CreateAssetMenu(fileName = "newShieldAction", menuName = "Card Actions/Shield Action")]
public class CardShieldAction : CardAction
{
    // 카드 사용
    public override void Use(int value, IBattleUnit target)
    {
        base.Use(value, target);

        Debug.Log($"방어 카드 사용 : 보호막 {value} 생성.");
        target.GetShield(value);
    }
}
