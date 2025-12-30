using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public struct CardTypeAction
{
    public string Name;             // 인스펙터 편의용
    public CardType type;           // 카드 타입
    public CardAction actionSO;     // 행동 SO
}

[System.Serializable]
public struct CardStatusAction
{
    public string Name;             // 인스펙터 편의용
    public string statusKey;        // 상태이상 키
    public CardAction actionSO;     // 행동 SO
}

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    [Header("카드 행동 리스트")]
    [SerializeField] private List<CardTypeAction> _typeActionList;
    [SerializeField] private List<CardStatusAction> _statusActionList;

    // 플레이어가 소지 중인 카드 정보 (Key: 카드 ID, Value: 보유 개수/레벨 등 정보 객체)
    public List<UserCard> UserCardList { get; private set; } = new List<UserCard>();

    // 현재 편성된 덱 (카드 ID 리스트)
    public List<int> CurrentDeck { get; private set; } = new List<int>();

    // 카드 타입 별 동작
    private Dictionary<CardType, ICardAction> _cardTypeActions = new Dictionary<CardType, ICardAction>();
    // 카드 상태이상 별 동작
    private Dictionary<string, ICardAction> _cardStatusActions = new Dictionary<string, ICardAction>();

    // 최대 덱 용량 (임시)
    public const int MAX_DECK_COUNT = 30;
    public int Gold { get; private set; } = 0;
    public void AddGold(int amount)
    {
        Gold += amount;
        if (Gold < 0) Gold = 0;
        // 필요하다면 UI 갱신 이벤트 호출
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 이동해도 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitCardActions();      // 카드 동작 초기화
        if (UserCardList.Count == 0)
        {
            InitStartingDeck(); // 스타팅 덱 초기화, 카드 데이터 한글/예외 설정
        }
        else
        {
            if (CurrentDeck == null) CurrentDeck = new List<int>();

            foreach (var cardData in DataManager.Instance.CardDict)
            {

                cardData.Value.SetString();
                cardData.Value.SetStatusValue();
            }
        }
    }
    // 통합 저장 매니저가 데이터를 넣어주는 통로
    public void LoadFromSaveData(List<UserCard> loadedCards, List<int> loadedDeck)
    {
        this.UserCardList = loadedCards ?? new List<UserCard>();
        this.CurrentDeck = loadedDeck ?? new List<int>();

        // 불러온 뒤 데이터 세팅이 필요하다면 여기서 수행
        foreach (var userCard in UserCardList)
        {
            var data = userCard.GetData();
            if (data != null)
            {
                data.SetString();
                data.SetStatusValue();
            }
        }

        Debug.Log($"[CardManager] 데이터 로드됨: 카드 {UserCardList.Count}장, 덱 {CurrentDeck.Count}장");
    }
    // 덱 구성 (테스트용 임시)
    public void InitStartingDeck()
    {
        // 모든 카드 데이터 한글 설정, 예외 처리
        foreach (var cardData in DataManager.Instance.CardDict)
        {
            cardData.Value.SetString();
            cardData.Value.SetStatusValue();
        }
        // 덱 및 인벤토리 초기화 (중복 방지용)
        UserCardList.Clear();
        CurrentDeck.Clear();


        AddStartingCard("KeyCardBanBookThrowing");
        AddStartingCard("KeyCardOldBookBreakDown");
        AddStartingCard("KeyCardWeightOfKnowledge");
        AddStartingCard("KeyCardRewindHistory");
        AddStartingCard("KeyCardOldBookShield");
        AddStartingCard("KeyCardAccumulatedKnowledge");

    }

    // 동작 구성
    private void InitCardActions()
    {
        // 카드 타입 액션 등록
        foreach (var map in _typeActionList)
        {
            if (map.actionSO != null && !_cardTypeActions.ContainsKey(map.type))
            {
                _cardTypeActions.Add(map.type, map.actionSO);
            }
        }

        // 상태이상 액션 등록
        foreach (var map in _statusActionList)
        {
            if (map.actionSO != null && !_cardStatusActions.ContainsKey(map.statusKey))
            {
                _cardStatusActions.Add(map.statusKey, map.actionSO);
            }
        }
    }

    // 첫 시작 카드 추가 (Key)
    private void AddStartingCard(string cardKey)
    {
        // Key로 카드데이터
        CardData cardData = DataManager.Instance.GetCard(cardKey);

        if(cardData == null)
        {
            Debug.Log($"키 {cardKey} 의 CardData가 존재하지 않습니다.");
            return;
        }

        // id
        int cardId = cardData.Id;

        // 플레이어 소지 카드 목록에 추가
        UserCardList.Add(new UserCard()
        {
            CardId = cardId,
            Level = 1,
            Count = 1,
            AcquiredTime = System.DateTime.Now
        });
    }


    public bool IsDeckValid(int requiredCount)
    {
        int realCardCount = 0;
        foreach (int id in CurrentDeck)
        {
            if (id != -1) realCardCount++;
        }

        if (realCardCount != requiredCount)
            return false;

        return true;
    }

    public int GetCardNumberOfAvailable(int level, CardGrade grade)
    {
        int numberOfAvailable;

        switch (grade)
        {
            case CardGrade.Common:
                numberOfAvailable = DataManager.Instance.GetCommonCardNoAData(level).NumberOfAvailable;
                break;
            case CardGrade.Rare:
                numberOfAvailable = DataManager.Instance.GetRareCardNoAData(level).NumberOfAvailable;
                break;
            case CardGrade.Epic:
                numberOfAvailable = DataManager.Instance.GetEpicCardNoAData(level).NumberOfAvailable;
                break;
            case CardGrade.Legendary:
                numberOfAvailable = DataManager.Instance.GetLegendaryCardNoAData(level).NumberOfAvailable;
                break;
            default:
                numberOfAvailable = 0;
                break;
        }

        return numberOfAvailable;
    }
    // 카드 추가 (보상 획득 시 호출)
    public void AddCard(int cardId, int amount = 1)
    {
        var existingCard = UserCardList.Find(x => x.CardId == cardId);
        if (existingCard != null)
        {
            existingCard.Count = Mathf.Min(existingCard.Count + amount, 99);
        }
        else
        {
            UserCardList.Add(new UserCard()
            {
                CardId = cardId,
                Level = 1,
                Count = Mathf.Min(amount, 99),
                AcquiredTime = System.DateTime.Now
            });

            // 미보유 카드 획득 시 플레이어 경험치 획득
            GetExp(cardId);
        }
    }
    public void AddKeyCard(string cardKey, int amount = 1)
    {
        // 카드 데이터 호출
        CardData data = DataManager.Instance.GetCard(cardKey);

        // 오타나 데이터 누락으로 null일 경우 에러 처리
        if (data == null)
        {
            Debug.LogError($"[CardManager] '{cardKey}'라는 키를 가진 카드를 찾을 수 없습니다.");
            return;
        }

        // 데이터에서 ID 추출
        int cardId = data.Id;

        // 이후 로직은 기존과 동일하게 ID 사용
        var existingCard = UserCardList.Find(x => x.CardId == cardId);

        if (existingCard != null)
        {
            existingCard.Count = Mathf.Min(existingCard.Count + amount, 99);
        }
        else
        {
            UserCardList.Add(new UserCard()
            {
                CardId = cardId,
                Level = 1,
                Count = Mathf.Min(amount, 99),
                AcquiredTime = System.DateTime.Now
            });

            // 미보유 카드 획득 시 플레이어 경험치 획득
            GetExp(cardId);
        }
    }
    // 특정 카드 가져오기
    public UserCard GetCard(int cardId)
    {
        var userCard = UserCardList.Find(x => x.CardId == cardId);
        return userCard != null ? userCard : null;
    }

    // 특정 카드의 레벨 가져오기
    public int GetCardLevel(int cardId)
    {
        var userCard = UserCardList.Find(x => x.CardId == cardId);
        return userCard != null ? userCard.Level : 1;
    }

    // 동작 반환 (CardType)
    public bool GetAction(CardType type, out ICardAction action)
    {
        if (_cardTypeActions.TryGetValue(type, out action))
            return true;

        Debug.LogWarning($"{type}에 해당하는 행동이 없습니다.");
        return false;
    }

    // 동작 반환 (StatusEffect)
    public bool GetAction(string statusEffect, out ICardAction action)
    {
        if (_cardStatusActions.TryGetValue(statusEffect, out action))
            return true;

        Debug.LogWarning($"{statusEffect}에 해당하는 행동이 없습니다.");
        return false;
    }

    // 미보유 카드 첫 획득 시 플레이어 경험치 획득
    private void GetExp(int cardId)
    {
        int exp;

        CardGrade grade = DataManager.Instance.GetCard(cardId).CardGrade;

        switch (grade)
        {
            case CardGrade.Common:
            case CardGrade.Rare:
                exp = 20;
                break;
            case CardGrade.Epic:
                exp = 25;
                break;
            case CardGrade.Legendary:
                exp = 40;
                break;
            default:
                exp = 0;
                break;
        }

        Player.Instance.GetExp(exp);
    }
}
