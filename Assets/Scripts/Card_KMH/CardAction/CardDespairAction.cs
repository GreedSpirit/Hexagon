using UnityEngine;

[CreateAssetMenu(fileName = "newDespairAction", menuName = "Card Actions/Despair Action")]
public class CardDespairAction : CardActionStatus
{
    public override void Use(string statusEffectKey, int statusValue, int turn, IBattleUnit target)
    {
        base.Use(statusEffectKey, statusValue, turn, target);

        // 스택, 지속시간 둘 다 0 이면 무시
        if (turn == 0 && statusValue == 0) return;

        Debug.Log($"절망 사용 : {turn} 턴 지속");
        target.AddStatusEffect(statusEffectKey, turn, statusValue);
    }
}
