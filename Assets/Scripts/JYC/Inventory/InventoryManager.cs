using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public event Action OnDeckChanged; // UI 갱신 알림

    // 정렬 방식 Enum
    public enum SortType
    {
        GradeAsc,   // 등급 오름차순
        LevelDesc,  // 레벨 내림차순
        TypeOrder,  // 타입순 (공격 -> 치유 -> 방어 -> 주문)
        Name,       // 이름순
        Recent      // 최신순
    }

    public SortType CurrentSortType = SortType.GradeAsc;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 덱 변경 요청 (UI -> CardManager 전달)
    public bool ToggleDeckEquip(int cardId)
    {
        if (CardManager.Instance == null) return false;

        bool changed = CardManager.Instance.ToggleDeckEquip(cardId);

        if (changed)
        {
            OnDeckChanged?.Invoke(); // UI 갱신 방송
        }
        return changed;
    }

    // 현재 덱 리스트 가져오기 (UI 표시용)
    public List<int> CurrentDeck
    {
        get
        {
            if (CardManager.Instance == null) return new List<int>();
            return CardManager.Instance.CurrentDeck;
        }
    }

    // 보유 카드 리스트 가져오기 (정렬 적용)
    public List<UserCard> GetSortedList(bool isDeckBuildingMode)
    {
        if (CardManager.Instance == null) return new List<UserCard>();

        // CardManager의 원본 리스트를 가져와서 정렬
        var filteredList = CardManager.Instance.UserCardList.ToList();

        // 정렬 로직
        switch (CurrentSortType)
        {
            case SortType.GradeAsc: // 등급 오름차순 -> 이름
                return filteredList.OrderBy(x => x.GetData().CardGrade).ThenBy(x => x.GetData().Name).ToList();

            case SortType.LevelDesc: // 레벨 내림차순 -> 이름
                return filteredList.OrderByDescending(x => x.Level).ThenBy(x => x.GetData().Name).ToList();

            case SortType.TypeOrder: // 타입 (공격 -> 치유 -> 방어 -> 주문)
                return filteredList.OrderBy(x => GetTypeOrderWeight(x.GetData().CardType)).ThenBy(x => x.GetData().Name).ToList();

            case SortType.Name: // 이름 가나다
                return filteredList.OrderBy(x => x.GetData().Name).ToList();

            case SortType.Recent: // 획득 시간 -> 이름
                return filteredList.OrderByDescending(x => x.AcquiredTime).ToList();

            default:
                return filteredList;
        }
    }

    // 타입 정렬 가중치 함수
    private int GetTypeOrderWeight(CardType type)
    {
        switch (type)
        {
            case CardType.Attack: return 1;
            case CardType.Healing: return 2;
            case CardType.Shield: return 3;
            case CardType.Spell: return 4;
            default: return 99;
        }
    }

    public bool IsCardInDeck(int cardId)
    {
        if (CardManager.Instance == null) return false;
        return CardManager.Instance.CurrentDeck.Contains(cardId);
    }
    public void RefreshInventory()
    {
        OnDeckChanged?.Invoke();
    }
}