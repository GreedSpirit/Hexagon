using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

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
            LoadGame();
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
            foreach (var cardData in DataManager.Instance.CardDict)
            {
                cardData.Value.SetString();
                cardData.Value.SetStatusValue();
            }
        }
    }
    private void OnApplicationQuit()
    {
        SaveGame();
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


        // 덱 구성 없으니 일단 카드 데이터 전부
        //foreach (var cardData in DataManager.Instance.CardDict)
        //{
        //    // IsCard가 false(스킬)인 것은 건너뜀 (9~16번만 통과)
        //    if (cardData.Value == null || cardData.Value.IsCard == false) continue;
        //
        //    // 인벤토리에 넉넉하게 5장씩 (테스트용)
        //    AddCard(cardData.Value.Id, 5);
        //
        //    // CurrentDeck.Add(cardData.Value.Id);  주석 처리
        //}
        // 자동으로 덱 채우기 (테스트 편의용)
        // 가지고 있는 카드를 앞에서부터 순서대로 덱에 장착 시도 (최대 30장까지)
        // Common, Rare, Epic 카드가 덱에 들어가서 입장 조건을 맞춥니다.
        foreach (var userCard in UserCardList)
        {
            ToggleDeckEquip(userCard.CardId);
        }

        // 저장
        SaveGame();
    }

    // 동작 구성
    private void InitCardActions()
    {
        // 카드 타입
        _cardTypeActions.Add(CardType.Attack, new CardAttackAction());
        _cardTypeActions.Add(CardType.Healing, new CardHealingAction());
        _cardTypeActions.Add(CardType.Shield, new CardShieldAction());
        _cardTypeActions.Add(CardType.Spell, new CardSpellAction());

        // 카드 상태이상
        _cardStatusActions.Add("KeyStatusPoison", new CardPoisonAction());
        _cardStatusActions.Add("KeyStatusBurn", new CardBurnAction());
        _cardStatusActions.Add("KeyStatusPride", new CardPrideAction());
        _cardStatusActions.Add("KeyStatusVulnerable", new CardVulnerableAction());

        _cardStatusActions.Add("KeyStatusStigma", new CardVulnerableAction());
        _cardStatusActions.Add("KeyStatusKnowledge", new CardVulnerableAction());
        _cardStatusActions.Add("KeyStatusDespair", new CardVulnerableAction());
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
        // 지금은 단순히 개수만 체크하지만, 나중에 코스트 제한 등을 추가할 수 있음
        if (CurrentDeck.Count != requiredCount)
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

    // 덱 장착/해제 (InventoryManager에서 호출)
    public bool ToggleDeckEquip(int cardId)
    {
        // 이미 있으면 제거
        if (CurrentDeck.Contains(cardId))
        {
            CurrentDeck.Remove(cardId);
            return true;
        }
        // 중복 장착 방지 추가
        if (CurrentDeck.Contains(cardId))
        {
            Debug.Log("같은 카드는 한 번만 장착할 수 있습니다!");
            return false;
        }
        // 최대 개수 확인
        if (CurrentDeck.Count >= MAX_DECK_COUNT) return false;

        var userCard = UserCardList.Find(x => x.CardId == cardId);
        if (userCard != null)
        {
            CurrentDeck.Add(cardId);
            return true;
        }

        return false;
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

    // 저장 데이터 포맷 클래스
    [System.Serializable]
    public class SaveData
    {
        public int myGold;
        public List<UserCard> myCards; // 내 보유 카드
        public List<int> myDeck; // 내 덱 구성
    }

    //  게임 저장하기 (JSON 방식)
    [ContextMenu("Save Game")] // 유니티 에디터 인스펙터에서 우클릭으로 실행 가능
    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.myGold = this.Gold;
        data.myCards = this.UserCardList;
        data.myDeck = this.CurrentDeck;

        // JSON 변환
        string json = JsonUtility.ToJson(data, true); // true는 보기 좋게 줄바꿈 함
        // 파일 저장 경로 
        string path = Path.Combine(Application.persistentDataPath, "savegame.json");
        File.WriteAllText(path, json);

        Debug.Log($"[Save] 저장 완료: {path}");
    }

    // 게임 불러오기
    [ContextMenu("Load Game")]
    public void LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "savegame.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            if (data != null)
            {
                this.Gold = data.myGold;
                this.UserCardList = data.myCards ?? new List<UserCard>();
                this.CurrentDeck = data.myDeck ?? new List<int>();
                Debug.Log($"[Load] 불러오기 완료. 카드 {UserCardList.Count}장, 덱 {CurrentDeck.Count}장");
            }
        }
        else
        {
            Debug.Log("[Load] 저장된 파일이 없습니다. 새로 시작합니다.");
        }
    }
}
