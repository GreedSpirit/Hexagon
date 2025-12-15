using UnityEngine;

public class Card : MonoBehaviour
{
    
    public CardData Data { get; private set; } // CSV 데이터

    private HandManager handManager;

    public void Init(CardData data, HandManager manager)
    {
        Data = data;
        handManager = manager;
    }

    // 카드가 '사용' 시도됨 (드래그가 끝났을 때 호출)
    public void TryUse()
    {
        // 조건 검사
        // (사용 횟수)

        // 통과 시

        // 카드 사용 로직
        Debug.Log("카드 사용");
        // 카드 별로 효과가 많이 있을 수 있어서 (실드 사용해서 공격, 핸드 리롤 같은)
        // 테이블에 Attack, Shield, 같은 기능 정리해서 SO로 모듈 추가하는 형식이면 어떤가

        // 핸드 제거
        handManager.UseCard(gameObject);
    }
}
