public interface ICardAction
{
    // 일반 카드 사용 (카드 계산 값, 적용 대상)
    public void Use(int value, IBattleUnit target);


    // 주문 카드 사용 (상태이상 데이터, 상태이상 부여 값, 약화강화 지속 턴, 적용 대상)
    public void Use(string statusEffectKey, int statusValue, int turn, IBattleUnit target);
}
