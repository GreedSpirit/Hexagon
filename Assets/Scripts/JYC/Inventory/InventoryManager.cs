using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    // 보유 중인 카드 리스트
    public List<UserCard> UserCardList { get; private set; } = new List<UserCard>();

    // 정렬 방식 Enum
    public enum SortType
    {
        GradeAsc,   // 등급 오름차순 (기본)
        LevelDesc,  // 레벨 내림차순
        TypeOrder,  // 타입 (공격->방어->주문)
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
        CurrentDeck.Add(1);
        CurrentDeck.Add(3);
        Debug.Log("테스트 카드 추가 완료");

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
        Debug.Log($"[Manager] 필터링 요청 받음! 모드: {isDeckBuildingMode}");
        string deckListStr = string.Join(", ", CurrentDeck);
        Debug.Log($"[Manager] 현재 덱 리스트(ID): [{deckListStr}]");
        // 필터링 (덱 편성 모드일 때 퀘스트 카드 제외)
        // 기획 변경으로 'IsCard'가 true인 것만 덱에 넣을 수 있다고 가정
        var filteredList = UserCardList;

        if (isDeckBuildingMode)
        {
            // 덱 리스트(CurrentDeck)에 포함된 카드 ID만 남깁니다.
            filteredList = UserCardList.Where(x => CurrentDeck.Contains(x.CardId)).ToList();
        }
        Debug.Log($"[Manager] 필터링 결과: {filteredList.Count}개의 카드를 보냄");
        // 정렬 (2차 정렬은 이름순)
        switch (CurrentSortType)
        {
            case SortType.GradeAsc: // 등급 오름차순 -> 이름
                return filteredList.OrderBy(x => x.GetData().CardGrade).ThenBy(x => x.GetData().Name).ToList();

            case SortType.LevelDesc: // 레벨 내림차순 -> 이름
                return filteredList.OrderByDescending(x => x.Level).ThenBy(x => x.GetData().Name).ToList();

            case SortType.TypeOrder: // 타입 (공격->방어->주문) -> 이름
                return filteredList.OrderBy(x => x.GetData().CardType).ThenBy(x => x.GetData().Name).ToList();

            case SortType.Name: // 이름 가나다
                return filteredList.OrderBy(x => x.GetData().Name).ToList();

            case SortType.Recent: // 획득 시간 -> 이름
                return filteredList.OrderByDescending(x => x.AcquiredTime).ThenBy(x => x.GetData().Name).ToList();

            default:
                return filteredList;
        }
    }
    // 현재 덱에 장착된 카드의 ID들을 담을 리스트 (임시로 가정)
    public List<int> CurrentDeck = new List<int>();

    // 해당 카드 ID가 덱에 포함되어 있는지 확인하는 함수
    public bool IsCardInDeck(int cardId)
    {
        return CurrentDeck.Contains(cardId);
    }
}