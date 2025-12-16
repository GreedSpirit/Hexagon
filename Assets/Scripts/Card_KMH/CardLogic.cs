using UnityEngine;

public class CardLogic : MonoBehaviour
{
    public CardData Data { get; private set; } // CSV 데이터

    private HandManager _handManager;

    public void Init(CardData data, HandManager manager)
    {
        // 카드 데이터
        Data = data;

        _handManager = manager;
    }

    // 카드 사용 시도
    public void TryUse()
    {
        // 사용 횟수 조건 검사
        if (Data.NumberOfAvailable <= 0)
        {
            Debug.Log($"카드 사용 가능 횟수가 0 입니다");
            return;
        }

        // 계산된 카드 수치
        int value = Data.GetCardValue();

        // 사용
        Data.CardAction.Use(Data, value);

        // 핸드 제거
        _handManager.UseCard(gameObject);
    }
}
