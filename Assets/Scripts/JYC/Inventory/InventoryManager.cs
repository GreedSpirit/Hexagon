using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public event Action OnDeckChanged; // UI 갱신 알림
    public bool IsDropProcessing { get; set; } = false;
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

    public bool IsCardInDeck(int cardId)
    {
        if (CardManager.Instance == null) return false;
        return CardManager.Instance.CurrentDeck.Contains(cardId);
    }

    private int _selectedInventoryCardId = -1;
    public event Action OnRequestDeselect; // 선택 해제 요청
    public void SetSelectedCardId(int id)
    {
        _selectedInventoryCardId = id;
    }

    public int GetSelectedCardId()
    {
        return _selectedInventoryCardId;
    }

    public void DeselectAll()
    {
        _selectedInventoryCardId = -1;
        OnRequestDeselect?.Invoke();
    }

    // 덱 카드 교체
    public void ReplaceDeckCardAt(int slotIndex, int newCardId)
    {
        if (CardManager.Instance == null) return;
        List<int> deck = CardManager.Instance.CurrentDeck;

        if (slotIndex < 0 || slotIndex >= deck.Count) return;

        if (deck.Contains(newCardId))
        {
            Debug.Log("이미 덱에 존재하는 카드입니다.");
            return;
        }

        deck[slotIndex] = newCardId;
        Debug.Log($"덱 교체 완료 [{slotIndex}번 슬롯] -> {newCardId}");

        OnDeckChanged?.Invoke();
        CardManager.Instance.SaveGame(); // 저장
    }

    public void RefreshInventory()
    {
        OnDeckChanged?.Invoke();
    }
    public void InvokeDeckChanged()
    {
        OnDeckChanged?.Invoke();
    }
    public List<UserCard> GetSortedList(bool isDeckBuildingMode)
    {
        if (CardManager.Instance == null) return new List<UserCard>();

        //  원본 리스트 가져오기
        var list = new List<UserCard>(CardManager.Instance.UserCardList);

        //  정렬 실행 (데이터가 null일 경우를 대비하여 안전하게 처리)
        switch (CurrentSortType)
        {
            case SortType.GradeAsc:
                return list.OrderBy(x =>
                {
                    var data = x.GetData();
                    // 데이터가 없으면 가장 낮은 우선순위로 취급
                    return data != null ? (int)data.CardGrade : 999;
                }).ToList();

            case SortType.LevelDesc:
                return list.OrderByDescending(x => x.Level).ToList();

            case SortType.TypeOrder:
                return list.OrderBy(x =>
                {
                    var data = x.GetData();
                    return data != null ? (int)data.CardType : 999;
                }).ToList();

            case SortType.Name:
                return list.OrderBy(x =>
                {
                    var data = x.GetData();
                    return data != null ? data.Name : "";
                }).ToList();

            case SortType.Recent:
                return list.OrderByDescending(x => x.AcquiredTime).ToList();

            default:
                return list;
        }
    }
}