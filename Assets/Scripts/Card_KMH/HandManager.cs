using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandManager : MonoBehaviour
{
    [Header("카드 프리팹")]
    [SerializeField] GameObject cardPrefab;     // 카드 프리팹

    [Header("카드 설정")]
    [SerializeField] float _moveSpeed = 15f;        // 이동/회전 속도
    [SerializeField] float _scaleSpeed = 15f;       // 확대/축소 속도
    [SerializeField] float _hoverScale = 1.5f;      // 마우스 올렸을 때 커지는 배율
    [SerializeField] float _useScreenRatio = 0.3f;  // 카드 사용할 화면 높이 비율
    [SerializeField] float _hoverVisualScaleOffset = 1.2f;// 호버 떨림 방지용 비주얼 크기 옵셋

    [Header("핸드 설정")]
    [SerializeField] Transform _handTransform;  // 핸드 위치
    [SerializeField] float _maxAngle = 30f;     // 부채꼴 최대 각도
    [SerializeField] float _maxSpacing = 7f;    // 카드 간 최대 간격
    [SerializeField] float _radius = 2000f;     // 부채꼴 반지름 (클수록 완만)
    [SerializeField] int _startHandCount;       // 시작 시 뽑을 카드 수
    [SerializeField] int _handLimit;            // 핸드 소지 한계

    [Header("소멸 위치 설정")]
    [SerializeField] Transform _disappearPoint;    // 소멸 고정 위치

    [Space]
    [SerializeField] TextMeshProUGUI _deckCount;


    public float MoveSpeed => _moveSpeed;
    public float ScaleSpeed => _scaleSpeed;
    public float HoverScale => _hoverScale;
    public float UseScreenRatio => _useScreenRatio;
    public float HoverVisualScaleOffset => _hoverVisualScaleOffset;
    public float CardHalfHeight => _cardHalfHeight;
    public int HandCount => _handCards.Count;               // 스테이지 종료 시 체크
    public int DeckCount => _deck.Count;                    // 스테이지 종료 시 체크
    public CardUI SelectedCard => _selectedCardUI;
    public IBattleUnit TargetPlayer => _targetPlayer;
    public IBattleUnit TargetMonster => _targetMonster;

    // 카드 매니저
    private CardManager _cardManager;

    // 덱의 카드 id 리스트
    private Queue<int> _deck;

    // 현재 들고 있는 카드 리스트 (로직)
    private List<CardLogic> _handCards = new List<CardLogic>();

    // 드로우 끝나면 호출
    public event Action OnDrawEnd;

    // 카드 절반 높이
    private float _cardHalfHeight;

    // 덱 시작 카드 수
    private int _deckCardCount;

    private IBattleUnit _targetPlayer;     // 타겟 플레이어 
    private IBattleUnit _targetMonster;    // 타겟 몬스터

    private float _playerBuff;             // 플레이어 강화 수치

    // 선택된 카드 UI (1회 클릭)
    private CardUI _selectedCardUI;


    private IEnumerator Start()
    {
        _cardManager = CardManager.Instance;

        // 카드 높이
        float cardHeight = cardPrefab.GetComponent<RectTransform>().rect.height;

        // 높이 절반
        _cardHalfHeight = cardHeight / 2f;

        // 타겟 플레이어
        SetPlayerTarget();

        // 테스트할 때는 동시에 Start가 실행되어서 꼬일 가능성 있기 때문에
        yield return null;

        // 덱 구성
        SetupDeck();
    }

    private void Update()
    {
        // 우클릭 릴리즈
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            // 선택된 카드가 있을 때 && 선택 카드의 드래그 상태
            if (SelectedCard != null && SelectedCard.IsDragging == false)
            {
                // 카드 선택 해지
                DeselectCard();
            }
        }
    }


    // 덱 구성
    private void SetupDeck()
    {
        // 덱 복사
        List<int> newDeck = new List<int>();

        foreach (int cardId in _cardManager.CurrentDeck)
        {
            // 소지중인 카드에서 id 카드 레벨 가져오기
            int cardLevel = _cardManager.GetCardLevel(cardId);

            // id 카드 데이터 불러오기
            CardData cardData = DataManager.Instance.GetCard(cardId);

            // 카드 사용 가능 횟수
            int cardNumberOfAvailable = _cardManager.GetCardNumberOfAvailable(cardLevel, cardData.CardGrade);

            // 사용 횟수 만큼
            for (int i = 0; i < cardNumberOfAvailable; i++)
            {
                // 덱에 id 추가
                newDeck.Add(cardId);
            }
        }

        // 덱 카드 수 설정
        _deckCardCount = newDeck.Count;

        _deckCount.text = $"Deck : {_deckCardCount} / {_deckCardCount}";

        // 덱 섞기
        ShuffleDeck(newDeck);

        // 덱 설정 완료
        _deck = new Queue<int>(newDeck);
    }

    // 셔플 (Fisher Yates)
    private void ShuffleDeck(List<int> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = UnityEngine.Random.Range(0, deck.Count);
            int temp = deck[i];
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
        int cardID = _deck.Dequeue();

        // ID 카드 데이터 가져오기
        CardData cardData = DataManager.Instance.GetCard(cardID);

        // ID 카드 레벨 가져오기
        int level = _cardManager.GetCardLevel(cardID);

        // 카드 UI 생성
        GameObject newCard = Instantiate(cardPrefab, _handTransform.position, Quaternion.identity, _handTransform);

        // 설정
        CardLogic cardLogic = newCard.GetComponent<CardLogic>();
        CardUI cardUI = newCard.GetComponent<CardUI>();

        // 매니저 연결
        cardLogic.Init(cardData, this, level);
        cardUI.Init(cardData, this);

        // 덱 카드 수 갱신
        _deckCount.text = $"Deck : {_deck.Count} / {_deckCardCount}";

        // 오버 드로우 체크
        if (_handCards.Count >= _handLimit)
        {
            // 오버드로우 발생
            cardUI.OnDisappear(_disappearPoint);

            // 핸드에 추가 안하고 바로 끝
            return;
        }

        // 리스트 추가
        _handCards.Add(cardLogic);

        // 카드 내용 대상 상태이상 따라 한 번 체크
        TargetStatusValueChanged();

        // 정렬
        AlignCards();
    }


    // 카드 사용
    public void UseCard(GameObject cardObject)
    {
        // 사용된 카드가 선택된 UI와 같아야
        if (cardObject == _selectedCardUI.gameObject)
        {
            // 소멸
            _selectedCardUI.OnDisappear(_disappearPoint);
            _selectedCardUI = null;
        }

        // 핸드 리스트에서 제외
        foreach (var card in _handCards)
        {
            if(cardObject == card.gameObject)
            {
                _handCards.Remove(card);
                break;
            }
        }

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
        Vector3 center = _handTransform.position - (Vector3.up * _radius);

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
            _handCards[i].gameObject.GetComponent<CardUI>().SetBase(position, rotation, i);

            // 맨 위로 설정
            _handCards[i].gameObject.transform.SetSiblingIndex(i);
        }
    }




    // 카드 선택
    public void SetSelectedCard(CardUI cardUI)
    {
        // 이미 선택된 카드면 무시
        if (_selectedCardUI == cardUI) return;

        // 선택된 카드 있으면 하이라이트 꺼지게
        if (_selectedCardUI != null)
            _selectedCardUI.Deselect();

        // 선택 카드 등록
        _selectedCardUI = cardUI;
    }
    
    // 카드 선택 해제 (다른 카드 선택, 빈 공간, 턴 종료, 별도 키 입력)
    public void DeselectCard()
    {
        if (_selectedCardUI != null)
        {
            _selectedCardUI.Deselect();
            _selectedCardUI = null;
        }
    }




    // 타겟 플레이어 설정
    private void SetPlayerTarget()
    {
        _targetPlayer = Player.Instance;
    }

    // 타겟 몬스터 설정
    public void SetMonsterTarget(IBattleUnit newTarget)
    {
        _targetMonster = newTarget;
    }

    // 플레이어 강화 수치 설정
    public void SetPlayerBuff(float playerBuff)
    {
        _playerBuff = playerBuff;
    }


    // 대상의 상태이상 수치 변하면
    // 아니면 상태이상 감소 사이클 다 돌면
    public void TargetStatusValueChanged()
    {
        // 플레이어 강화 수치
        float playerBuff = _playerBuff;
        // 몬스터 약화 수치
        float monsterDeBuff = 0;
        // 몬스터 방어력
        int monsterDef = 0;

        // 타겟 몬스터 수치
        if(_targetMonster is MonsterStatus monster)
        {
            // 약화
            // 걸린 효과 전부 체크
            foreach (var monsterStatusEffect in monster.StatusEffects)
            {
                // 로직이 받는 피해량 타입이면
                if (monsterStatusEffect.EffectLogic == EffectLogic.DmgTaken)
                    monsterDeBuff += monsterStatusEffect.Value;
            }
            //monsterDeBuff = monster.Get
            // 방어력
            monsterDef = monster.GetMonsterDefense();
        }

        // 모든 카드 최종 데미지, 설명 변경
        foreach (var card in _handCards)
        {
            card.UpdateDeal(playerBuff, monsterDeBuff, monsterDef);
            card.UpdateDesc();
            card.gameObject.GetComponent<CardUI>().SetDescText();
        }
    }


    // 페이즈 변경 시 호출
    public void OnPhaseChanged(PhaseType phaseType)
    {
        // 드로우 페이즈일 때
        if(phaseType == PhaseType.Draw)
        {
            // 카드 뽑기
            DrawCard();

            // 카드 뽑기 종료 이벤트
            OnDrawEnd?.Invoke();
        }
        else if (phaseType == PhaseType.Start)
        {
            // 초기 핸드 채우기
            for (int i = 0; i < _startHandCount; i++)
            {
                DrawCard();
            }
        }
        // 플레이어턴 끝나면 선택 카드 해제
        else if (phaseType == PhaseType.EnemyAct)
        {
            // 선택된 카드가 있을 때 && 선택 카드의 드래그 상태
            if (SelectedCard != null && SelectedCard.IsDragging == false)
            {
                // 카드 선택 해지
                DeselectCard();
            }
        }
    }
}
