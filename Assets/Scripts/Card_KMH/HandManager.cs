using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] float _hoverVisualScaleOffset = 1.2f;// 호버 떨림 방지용 비주얼 크기 옵셋

    [Header("핸드 설정")]
    [SerializeField] float _maxAngle = 30f;     // 부채꼴 최대 각도
    [SerializeField] float _maxSpacing = 7f;    // 카드 간 최대 간격
    [SerializeField] float _radius = 2000f;     // 부채꼴 반지름 (클수록 완만)
    [SerializeField] int _startHandCount;       // 시작 시 뽑을 카드 수
    [SerializeField] int _handLimit;            // 핸드 소지 한계

    [Space]
    [SerializeField] TextMeshProUGUI _deckCount;


    public float MoveSpeed => _moveSpeed;
    public float ScaleSpeed => _scaleSpeed;
    public float HoverScale => _hoverScale;
    public float UseScreenRatio => _useScreenRatio;
    public float HoverVisualScaleOffset => _hoverVisualScaleOffset;
    public float CardHalfHeight => _cardHalfHeight;

    // 덱의 카드 id 리스트
    private Queue<CardData> _deck;

    // 현재 들고 있는 카드 리스트
    private List<GameObject> _handCards = new List<GameObject>();

    // 카드 절반 높이
    private float _cardHalfHeight;

    // 덱 시작 카드 수
    private int _deckCardCount;

    private IBattleUnit targetPlayer;     // 타겟 플레이어 
    private IBattleUnit targetMonster;    // 타겟 몬스터

    // 선택된 카드
    private GameObject _selectedCard;



    private void Start()
    {
        // 카드 높이
        float cardHeight = cardPrefab.GetComponent<RectTransform>().rect.height;

        // 높이 절반
        _cardHalfHeight = cardHeight / 2f;

        // 타겟 플레이어
        SetPlayerTarget();

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
        // 덱 복사
        List<CardData> newDeck = new List<CardData>();

        foreach (var card in TestGameManager_KMH.Instance.Deck)
        {
            int cardKey = card.Key;
            int cardLevel = card.Value;

            // cardKey 사용해서 테이블 정보 불러오기
            CardData tempCardData = DataManager.Instance.GetCard(cardKey);

            // 레벨, 사용 가능 횟수
            tempCardData.SetCardLevel(cardLevel);

            // 카드 사용 가능 횟수
            int cardNumberOfAvailable = TestGameManager_KMH.Instance.GetCardNumberOfAvailable(cardLevel, tempCardData.CardGrade);


            // 사용 횟수 만큼 덱에 추가
            for (int i = 0; i < cardNumberOfAvailable; i++)
            {
                // 덱에 추가할 진짜 카드 데이터
                CardData newCardData = DataManager.Instance.GetCard(cardKey);

                // 카드 동작, 레벨, 사용 횟수 설정
                newCardData.SetCardAction();
                newCardData.SetCardLevel(cardLevel);

                // 덱에 추가
                newDeck.Add(newCardData);
            }
        }

        // 덱 카드 수 설정
        _deckCardCount = newDeck.Count;

        _deckCount.text = $"Deck : {_deckCardCount} / {_deckCardCount}";

        // 덱 섞기
        ShuffleDeck(newDeck);

        // 덱 설정 완료
        _deck = new Queue<CardData>(newDeck);
    }

    // 셔플 (Fisher Yates)
    private void ShuffleDeck(List<CardData> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(0, deck.Count);
            CardData temp = deck[i];
            deck[i] = deck[rand];
            deck[rand] = temp;
        }
    }



    // 테스트용 드로우 버튼에 연결
    public void DrawCard()
    {
        if (_deck.Count <= 0)
        {
            Debug.Log("덱이 비었습니다");
            return;
        }

        // 덱큐에서 카드 한장 뽑기
        CardData cardData = _deck.Dequeue();

        // 덱 카드 수 갱신
        _deckCount.text = $"Deck : {_deck.Count} / {_deckCardCount}";

        // 오버 드로우 체크
        if (_handCards.Count >= _handLimit)
        {
            Debug.Log("오버 드로우 발생");
            // 나중에 카드 생성 이후로 위치 변경
            // 카드 소멸 보여주기
            return;
        }

        // 카드 UI 생성
        GameObject newCard = Instantiate(cardPrefab, transform.position, Quaternion.identity, transform);

        // 설정
        CardLogic cardLogic = newCard.GetComponent<CardLogic>();
        CardUI cardUI = newCard.GetComponent<CardUI>();

        // 타겟
        IBattleUnit target = cardData.Target == Target.Self ? targetPlayer : targetMonster;

        // 매니저 연결
        cardLogic.Init(cardData, this, target);
        cardUI.Init(cardData, this);
        
        // 리스트 추가
        _handCards.Add(newCard);

        // 정렬
        AlignCards();
    }

    // 카드 사용
    public void UseCard(GameObject card)
    {
        // 핸드 리스트에서 제외
        _handCards.Remove(card);
        // 삭제 (나중에 풀링)
        Destroy(card.gameObject);
        // 남은 카드 재정렬
        AlignCards();           
    }

    // 카드 부채꼴 정렬
    private void AlignCards()
    {
        // 카드 수
        int count = _handCards.Count;

        // 부채꼴 범위 정하기 (카드가 적으면 좁게, 많으면 maxAngle까지)
        float currentAngle = Mathf.Min((count - 1) * _maxSpacing, _maxAngle);

        // 카드 사이의 간격
        float currentAngleStep = count > 1 ? currentAngle / (count - 1) : 0;

        // 시작 각도 설정
        // 전체 각도의 절반만큼 오른쪽으로
        float startAngle = -currentAngle / 2f;

        // 중심 포인트 지정 (반지름 만큼 아래로)
        Vector3 center = transform.position - (Vector3.up * _radius);

        // 카드 순회
        for (int i = 0; i < count; i++)
        {
            // 현재 카드 각도
            float angleDeg = startAngle + (currentAngleStep * i);

            // 회전
            Quaternion rotation = Quaternion.Euler(0, 0, angleDeg);

            // 위치
            Vector3 direction = rotation * Vector3.up;
            Vector3 position = center + (direction * (_radius + _cardHalfHeight));

            // 카드 위치 설정
            _handCards[i].GetComponent<CardUI>().SetBase(position, rotation);

            // 맨 위로 설정
            _handCards[i].transform.SetSiblingIndex(i);
        }
    }

    // 플레이어 타겟 설정
    private void SetPlayerTarget()
    {
        targetPlayer = Player.Instance;
    }

    // 몬스터 타겟 설정
    public void SetMonsterTarget(IBattleUnit newTarget)
    {
        targetMonster = newTarget;
    }
}
