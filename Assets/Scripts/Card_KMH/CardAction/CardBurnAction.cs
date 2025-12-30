using UnityEngine;

[CreateAssetMenu(fileName = "newBurnAction", menuName = "Card Actions/Burn Action")]
public class CardBurnAction : CardActionStatus
{
    public override void Use(string statusEffectKey, int statusValue, int turn, IBattleUnit target)
    {
        base.Use(statusEffectKey, statusValue, turn, target);

        // 스택, 지속시간 둘 다 0 이면 무시
        if (turn == 0 && statusValue == 0) return;

        Debug.Log($"화상 사용 : {statusValue} 부여");
        target.AddStatusEffect(statusEffectKey, turn, statusValue);
    }
}
