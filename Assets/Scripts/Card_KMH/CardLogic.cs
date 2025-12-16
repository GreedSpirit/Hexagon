using UnityEngine;

public class CardLogic : MonoBehaviour
{
    public CardData Data { get; private set; } // CSV 데이터

    private ICardAction _cardAction;

    private HandManager _handManager;

    public void Init(CardData data, HandManager manager)
    {
        // 카드 데이터
        Data = data;
        // 카드 타입에 맞는 동작 가져오기
        _cardAction = TestGameManager_KMH.Instance.GetAction(Data.CardType);
        _handManager = manager;
    }

    // 카드가 '사용' 시도됨 (드래그가 끝났을 때 호출)
    public void TryUse()
    {
        // 조건 검사
        // (사용 횟수)

        // 통과 시

        // 카드 사용 로직

        // 카드 기본 수치
        int value = _cardAction.GetValue(Data);

        _cardAction.Use(Data, value);

        // 핸드 제거
        _handManager.UseCard(gameObject);
    }
}
