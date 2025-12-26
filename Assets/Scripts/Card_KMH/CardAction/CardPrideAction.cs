using UnityEngine;

[CreateAssetMenu(fileName = "newPrideAction", menuName = "Card Actions/Pride Action")]
public class CardPrideAction : CardAction
{
    public override void Use(string statusEffectKey, int statusValue, int turn, IBattleUnit target)
    {
        base.Use(statusEffectKey, statusValue, turn, target);

        // 스택, 지속시간 둘 다 0 이면 무시
        if (turn == 0 && statusValue == 0) return;

        Debug.Log($"오만 사용 : {turn} 턴 지속");
        target.AddStatusEffect(statusEffectKey, turn, statusValue);
    }
}
