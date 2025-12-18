using UnityEngine;

public interface ICardStatusEffect
{
    public void Use(StatusEffectData data, int value, IBattleUnit target);         // 상태이상 사용
}
