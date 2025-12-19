using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    // 보유 중인 카드 리스트

    public List<UserCard> UserCardList { get; private set; } = new List<UserCard>();
    public List<int> CurrentDeck = new List<int>(); // 현재 덱 (ID 리스트)
    public event Action OnDeckChanged; // 덱 상태가 변경될 때 UI에게 알리는 이벤트

    // 덱 최대 개수 제한 (일단 임시로 제한, 기획서에 따라 수정)
    private const int MAX_DECK_COUNT = 30;

    // 정렬 방식 Enum
    public enum SortType
    {
        GradeAsc,   // 등급 오름차순 (기본)
        LevelDesc,  // 레벨 내림차순
        TypeOrder,  // 타입 (공격 -> 치유 -> 방어 -> 주문)
        Name,       // 이름 가나다
        Recent      // 최신순
    }

    public SortType CurrentSortType = SortType.GradeAsc;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        // 테스트용: 게임 시작 시 임의의 카드 추가 (나중에 삭제)
        AddCard(1, 5); // ID 1번 카드 5장
        AddCard(2, 1);
        AddCard(3, 10);
    }

    private void Start()
    {
        // [중요] 게임 시작 시 팀원의 매니저(TestGameManager)와 덱 상태 동기화
        if (TestGameManager_KMH.Instance != null)
        {
            // 매니저에 덱이 비어있다면 테스트용 덱 주입 (개발용)
            if (TestGameManager_KMH.Instance.Deck.Count == 0)
            {
                ToggleDeckEquip(1); // 1번 카드 장착 시도
                ToggleDeckEquip(3); // 3번 카드 장착 시도
            }

            // 현재 덱 상태를 로컬 리스트에 복사 (UI 표시용)
            SyncDeckFromGameManager();
        }
    }
    // 덱에 넣고 빼는 스위치 함수
    public bool ToggleDeckEquip(int cardId)
    {
        
        // 팀원의 매니저가 없으면 중단 (방어 코드)
        if (TestGameManager_KMH.Instance == null) return false;

        var gameDeck = TestGameManager_KMH.Instance.Deck;

        // 이미 덱에 있는가? -> 제거
        if (gameDeck.ContainsKey(cardId))
        {
            gameDeck.Remove(cardId);
            Debug.Log($"[Inventory] 카드 {cardId} 덱에서 해제됨.");
        }
        // 덱에 없는가? -> 추가
        else
        {
            // 덱 용량 체크
            if (gameDeck.Count >= MAX_DECK_COUNT)
            {
                Debug.LogWarning("[Inventory] 덱이 가득 찼습니다!");
                return false;
            }

            // 내 인벤토리에서 해당 카드의 현재 레벨 가져오기
            var userCard = UserCardList.Find(x => x.CardId == cardId);
            if (userCard == null)
            {
                Debug.LogError($"[Inventory] 보유하지 않은 카드({cardId})를 장착하려 합니다.");
                return false;
            }

            // 매니저 덱에 추가 (Key: ID, Value: Level)
            gameDeck.Add(cardId, userCard.Level);
            Debug.Log($"[Inventory] 카드 {cardId} (Lv.{userCard.Level}) 덱에 장착됨.");
        }

        // 변경 사항을 내 로컬 리스트(CurrentDeck)에도 반영
        SyncDeckFromGameManager();

        OnDeckChanged?.Invoke();

        return true; 
    }
    // 매니저의 덱 정보를 내 리스트로 가져오는 함수
    private void SyncDeckFromGameManager()
    {
        if (TestGameManager_KMH.Instance == null) return;

        CurrentDeck.Clear();
        foreach (var key in TestGameManager_KMH.Instance.Deck.Keys)
        {
            CurrentDeck.Add(key);
        }
        OnDeckChanged?.Invoke();
    }

    // 카드 추가 로직 (99개 제한 반영)
    public void AddCard(int cardId, int amount = 1)
    {
        // 이미 있는 카든지 확인
        var existingCard = UserCardList.Find(x => x.CardId == cardId);

        if (existingCard != null)
        {
            existingCard.Count = Mathf.Min(existingCard.Count + amount, 99);
        }
        else
        {
            UserCard newCard = new UserCard()
            {
                CardId = cardId,
                Level = 1,
                Count = Mathf.Min(amount, 99),
                AcquiredTime = DateTime.Now
            };
            UserCardList.Add(newCard);
        }
    }

    // 카드 목록 가져오기 (필터링 및 정렬 적용)
    public List<UserCard> GetSortedList(bool isDeckBuildingMode)
    {
        // 필터링 
        var filteredList = UserCardList.ToList(); // 기본적으로 전체 보유 목록

        // 정렬 (2차 정렬은 이름순)
        switch (CurrentSortType)
        {
            case SortType.GradeAsc: // 등급 오름차순 -> 이름
                return filteredList.OrderBy(x => x.GetData().CardGrade).ThenBy(x => x.GetData().Name).ToList();

            case SortType.LevelDesc: // 레벨 내림차순 -> 이름
                return filteredList.OrderByDescending(x => x.Level).ThenBy(x => x.GetData().Name).ToList();

            case SortType.TypeOrder:
                // 단순 Enum 정렬이 아닌, 기획서 순서(공격->치유->방어->주문)를 강제하는 함수 사용
                return filteredList.OrderBy(x => GetTypeOrderWeight(x.GetData().CardType)).ThenBy(x => x.GetData().Name).ToList();

            case SortType.Name: // 이름 가나다
                return filteredList.OrderBy(x => x.GetData().Name).ToList();

            case SortType.Recent: // 획득 시간 -> 이름
                return filteredList.OrderByDescending(x => x.AcquiredTime).ThenBy(x => x.GetData().Name).ToList();

            default:
                return filteredList;
        }
    }
    private int GetTypeOrderWeight(CardType type)
    {
        switch (type)
        {
            case CardType.Attack: return 1;
            case CardType.Healing: return 2; // [체크] 치유 타입 순서 반영
            case CardType.Shield: return 3;
            case CardType.Spell: return 4;
            default: return 99; // 그 외
        }
    }

    public bool IsCardInDeck(int cardId)
    {
        if (TestGameManager_KMH.Instance == null) return false;
        return TestGameManager_KMH.Instance.Deck.ContainsKey(cardId);
    }
}