using UnityEngine;

public class CardLogic : MonoBehaviour
{
    public CardData Data { get; private set; } // CSV 데이터

    private HandManager _handManager;

    private IBattleUnit target;             // 타겟

    public void Init(CardData data, HandManager manager, IBattleUnit newTarget)
    {
        // 카드 데이터
        Data = data;

        _handManager = manager;

        // 타겟
        target = newTarget;
    }

    // 카드 사용 시도
    public void TryUse()
    {
        // 계산된 카드 수치
        int value = Data.GetCardValue();

        // 사용 (CardData, CardValue, IBattleUnit)
        Data.CardAction.Use(Data, value, target);

        // 핸드 제거
        _handManager.UseCard(gameObject);
    }
}
