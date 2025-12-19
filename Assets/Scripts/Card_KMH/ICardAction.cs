public interface ICardAction
{
    // 카드 사용 (상태이상 데이터, 카드 계산 값, 상태이상 부여 값, 약화강화 지속 턴, 적용 대상)
    public void Use(string statusEffectKey, int value, int statusValue, int turn, IBattleUnit target);
}
