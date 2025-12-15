using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("카드 프리팹")]
    [SerializeField] GameObject cardPrefab;     // 카드 프리팹

    [Header("카드 설정값")]
    [SerializeField] float _moveSpeed = 15f;        // 이동/회전 속도
    [SerializeField] float _scaleSpeed = 15f;       // 확대/축소 속도
    [SerializeField] float _hoverScale = 1.5f;      // 마우스 올렸을 때 커지는 배율
    [SerializeField] float _useScreenRatio = 0.3f;  // 카드 사용할 화면 높이 비율
    [SerializeField] float _hoverOffset = 1.2f;     // 호버 떨림 방지용 옵셋

    [Header("핸드 설정")]
    [SerializeField] float _maxAngle = 30f;     // 부채꼴 최대 각도
    [SerializeField] float _maxSpacing = 7f;    // 카드 간 최대 간격
    [SerializeField] float _radius = 2000f;     // 부채꼴 반지름 (클수록 완만)
    [SerializeField] int _startHandCount;       // 시작 시 뽑을 카드 수
    [SerializeField] int _handLimit;            // 핸드 소지 한계


    public float MoveSpeed => _moveSpeed;
    public float ScaleSpeed => _scaleSpeed;
    public float HoverScale => _hoverScale;
    public float UseScreenRatio => _useScreenRatio;
    public float HoverOffset => _hoverOffset;

    // 덱의 카드 id 리스트
    private Queue<int> deck;

    // 현재 들고 있는 카드 리스트
    private List<GameObject> handCards = new List<GameObject>();

    private void Start()
    {
        // 덱 구성
        SetupDeck();

        // 핸드 채우기
        for(int i = 0; i< _startHandCount; i++)
        {
            DrawCard();
        }
    }


    // 덱 구성
    private void SetupDeck()
    {
        // 테스트용 카드
        List<int> allIds = new();
        for(int i = 0; i < 10; i++)
            allIds.Add(i);

        // 덱 섞기
        ShuffleDeck(allIds);

        // 덱 설정 완료
        deck = new Queue<int>(allIds);
    }

    // 셔플 (Fisher Yates)
    private void ShuffleDeck(List<int> allIds)
    {
        for (int i = 0; i < allIds.Count; i++)
        {
            Debug.Log(i);
            int rand = Random.Range(0, allIds.Count);
            int temp = allIds[i];
            allIds[i] = allIds[rand];
            allIds[rand] = temp;
        }
    }



    // 테스트용 드로우 버튼에 연결
    public void DrawCard()
    {
        if (deck.Count <= 0)
        {
            Debug.Log("덱이 비었습니다");
            return;
        }

        // 덱큐에서 카드 한장 뽑기
        //int cardId = deck.Dequeue();
        // cardId 사용해서 테이블 정보 불러오기
        // 예시
        // var cardTable = TableManager.Instance.GetTable<int, CardTableData>();
        // CardTableData data = cardTable[cardId];
        //

        // 카드 생성
        GameObject card = Instantiate(cardPrefab, transform.position, Quaternion.identity, transform);

        // 설정
        Card cardLogic = card.GetComponent<Card>();
        CardUI cardUI = card.GetComponent<CardUI>();

        // 매니저 연결
        cardLogic.Init(this);
        //cardLogic.Init(data, this);
        cardUI.Init(this);
        //cardUI.Init(data, this);

        // 리스트 추가
        handCards.Add(card);

        // 정렬
        AlignCards();
    }

    // 카드 사용
    public void UseCard(GameObject card)
    {
        // 핸드 리스트에서 제외
        handCards.Remove(card);
        // 삭제 (나중에 풀링)
        Destroy(card.gameObject);
        // 남은 카드 재정렬
        AlignCards();           
    }

    // 카드 부채꼴 정렬
    private void AlignCards()
    {
        // 카드 수
        int count = handCards.Count;

        // 부채꼴 범위 정하기 (카드가 적으면 좁게, 많으면 maxAngle까지)
        float currentAngle = Mathf.Min((count - 1) * _maxSpacing, _maxAngle);

        // 카드 사이의 간격
        float currentAngleStep = count > 1 ? currentAngle / (count - 1) : 0;

        // 시작 각도 설정
        // 전체 각도의 절반만큼 왼쪽으로
        float startAngle = currentAngle / 2f;

        // 중심 포인트 지정 (반지름 만큼 아래로)
        Vector3 center = transform.position - (Vector3.up * _radius);

        // 카드 순회
        for (int i = 0; i < count; i++)
        {
            // 현재 카드 각도
            float angleDeg = startAngle - (currentAngleStep * i);

            // 회전
            Quaternion rotation = Quaternion.Euler(0, 0, angleDeg);

            // 위치
            Vector3 direction = rotation * Vector3.up;
            Vector3 position = center + (direction * _radius);

            // 카드 위치 설정
            handCards[i].GetComponent<CardUI>().SetBase(position, rotation);

            // 오른쪽이 맨 위로 올라오게
            handCards[i].transform.SetSiblingIndex(i);
        }
    }
}
