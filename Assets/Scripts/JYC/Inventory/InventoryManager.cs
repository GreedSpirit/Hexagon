using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public event Action OnDeckChanged;
    public event Action OnRequestDeselect; // 선택 해제 요청 이벤트 추가
    public bool IsDropProcessing { get; set; } = false;

    // 정렬 방식
    public enum SortType { GradeAsc, LevelDesc, TypeOrder, Name, Recent }
    public SortType CurrentSortType = SortType.GradeAsc;

    [Header("UI Resources (등급별 배경 이미지)")]
    public Sprite bgCommon;     // Common 뒷면
    public Sprite bgRare;       // Rare 뒷면
    public Sprite bgEpic;       // Epic 뒷면
    public Sprite bgLegendary;  // Legendary 뒷면

    [Header("UI Resources (테두리 이미지)")]
    public Sprite borderCommon;
    public Sprite borderRare;
    public Sprite borderEpic;
    public Sprite borderLegendary;

    public Sprite GetGradeBackground(CardGrade grade)
    {
        switch (grade)
        {
            case CardGrade.Common: return bgCommon;
            case CardGrade.Rare: return bgRare;
            case CardGrade.Epic: return bgEpic;
            case CardGrade.Legendary: return bgLegendary;
            default: return bgCommon;
        }
    }
    // 가상 슬롯 (동적으로 크기가 변함)
    private int[] _virtualDeckSlots;

    // 현재 적용 중인 덱 규칙 (던전마다 다름)
    private DeckData _currentDeckData;

    private int _selectedInventoryCardId = -1; // 선택된 카드 ID

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // DeckUI에서 던전에 입장할 때 이 함수를 호출해서 슬롯을 세팅해줍니다.
    public void ConfigureDeckSlots(DeckData deckData)
    {
        _currentDeckData = deckData;

        // 전체 슬롯 개수 계산
        int totalCount = deckData.NormalCount + deckData.RareCount + deckData.EpicCount;

        // 가상 슬롯 배열 재할당
        _virtualDeckSlots = new int[totalCount];
        for (int i = 0; i < totalCount; i++) _virtualDeckSlots[i] = -1; // 빈칸 초기화

        LoadVirtualDeckFromCardManager();
    }

    // CardManager의 리스트 -> 가상 슬롯으로 배치
    private void LoadVirtualDeckFromCardManager()
    {
        if (CardManager.Instance == null || _currentDeckData == null) return;

        // 리스트에 있는 카드들을 등급 규칙에 맞춰 가상 슬롯에 채워넣기
        foreach (int cardId in CardManager.Instance.CurrentDeck)
        {
            CardData data = DataManager.Instance.GetCard(cardId);
            if (data == null) continue;

            // 들어갈 수 있는 구역 확인
            int startIdx, endIdx;
            if (GetSlotRangeByGrade(data.CardGrade, out startIdx, out endIdx))
            {
                // 해당 구역 빈자리 찾아서 넣기
                for (int i = startIdx; i <= endIdx; i++)
                {
                    if (_virtualDeckSlots[i] == -1)
                    {
                        _virtualDeckSlots[i] = cardId;
                        break;
                    }
                }
            }
        }
    }

    // 가상 슬롯 상태 -> CardManager 리스트로 저장 (동기화)
    private void SaveVirtualDeckToCardManager()
    {
        if (CardManager.Instance == null) return;

        CardManager.Instance.CurrentDeck.Clear();

        foreach (int cardId in _virtualDeckSlots)
        {
            if (cardId != -1)
            {
                CardManager.Instance.CurrentDeck.Add(cardId);
            }
        }
        GameSaveManager.Instance.SaveGame();
    }

    // 등급별 슬롯 범위를 동적으로 계산
    private bool GetSlotRangeByGrade(CardGrade grade, out int start, out int end)
    {
        start = 0; end = 0;
        if (_currentDeckData == null) return false;

        switch (grade)
        {
            case CardGrade.Common:
                start = 0;
                end = _currentDeckData.NormalCount - 1;
                break;

            case CardGrade.Rare:
                start = _currentDeckData.NormalCount;
                end = start + _currentDeckData.RareCount - 1;
                break;

            case CardGrade.Epic:
            case CardGrade.Legendary:
                start = _currentDeckData.NormalCount + _currentDeckData.RareCount;
                end = start + _currentDeckData.EpicCount - 1;
                break;

            default: return false;
        }

        // 유효하지 않은 범위(개수가 0개인 경우 등) 체크
        if (start > end) return false;

        return true;
    }

    // 덱 조작 로직


    public bool ToggleDeckEquip(int cardId)
    {
        if (IsCardInDeck(cardId))
        {
            UnequipCard(cardId);
            return true;
        }
        else
        {
            return EquipCard(cardId);
        }
    }

    // 카드 장착 (클릭)
    public bool EquipCard(int cardId)
    {
        var userCard = CardManager.Instance.GetCard(cardId);
        if (userCard == null) return false;
        CardData data = userCard.GetData();
        if (data == null) return false;

        // 들어갈 구역 확인
        int startIdx, endIdx;
        if (!GetSlotRangeByGrade(data.CardGrade, out startIdx, out endIdx))
        {
            Debug.Log($"[{data.CardGrade}] 등급을 장착할 슬롯이 없습니다.");
            return false;
        }

        // 구역 내 빈자리 찾기
        int targetSlot = -1;
        for (int i = startIdx; i <= endIdx; i++)
        {
            if (_virtualDeckSlots[i] == -1)
            {
                targetSlot = i;
                break;
            }
        }

        if (targetSlot == -1)
        {
            Debug.Log($"[{data.CardGrade}] 등급 슬롯이 꽉 찼습니다.");
            return false;
        }

        // 장착 및 저장
        _virtualDeckSlots[targetSlot] = cardId;
        SaveVirtualDeckToCardManager();
        InvokeDeckChanged();
        return true;
    }

    // 카드 해제
    public void UnequipCard(int cardId)
    {
        if (_virtualDeckSlots == null) return;

        for (int i = 0; i < _virtualDeckSlots.Length; i++)
        {
            if (_virtualDeckSlots[i] == cardId)
            {
                _virtualDeckSlots[i] = -1;
                break;
            }
        }
        SaveVirtualDeckToCardManager();
        InvokeDeckChanged();
    }

    // 드래그 앤 드롭 (위치 지정 교체)
    public void ReplaceDeckCardAt(int slotIndex, int newCardId)
    {
        if (_virtualDeckSlots == null || slotIndex < 0 || slotIndex >= _virtualDeckSlots.Length) return;

        // 이미 덱에 있으면 기존 자리 비움
        for (int i = 0; i < _virtualDeckSlots.Length; i++)
        {
            if (_virtualDeckSlots[i] == newCardId)
            {
                if (i == slotIndex) return; // 제자리 드롭
                _virtualDeckSlots[i] = -1;
                break;
            }
        }

        _virtualDeckSlots[slotIndex] = newCardId;
        SaveVirtualDeckToCardManager();
        RefreshInventory();
        InvokeDeckChanged();
        DeselectAll();
    }
    public Sprite GetGradeBorderSprite(CardGrade grade)
    {
        switch (grade)
        {
            case CardGrade.Common: return borderCommon;
            case CardGrade.Rare: return borderRare;
            case CardGrade.Epic: return borderEpic;
            case CardGrade.Legendary: return borderLegendary;
            default: return borderCommon;
        }
    }

    // DeckUI가 가져갈 때 사용 (-1 포함된 리스트 반환)
    public List<int> CurrentDeck
    {
        get
        {
            if (_virtualDeckSlots == null) return new List<int>();
            return _virtualDeckSlots.ToList();
        }
    }

    public bool IsCardInDeck(int cardId)
    {
        if (_virtualDeckSlots == null) return false;
        return _virtualDeckSlots.Contains(cardId);
    }

    public void SetSelectedCardId(int id) => _selectedInventoryCardId = id;
    public int GetSelectedCardId() => _selectedInventoryCardId;

    public void DeselectAll()
    {
        _selectedInventoryCardId = -1;
        OnRequestDeselect?.Invoke();
    }

    public void RefreshInventory() => OnDeckChanged?.Invoke();
    public void InvokeDeckChanged() => OnDeckChanged?.Invoke();

    public List<UserCard> GetSortedList(bool isDeckBuildingMode)
    {
        if (CardManager.Instance == null) return new List<UserCard>();
        var list = new List<UserCard>(CardManager.Instance.UserCardList);

        switch (CurrentSortType)
        {
            case SortType.GradeAsc:
                return list.OrderBy(x => { var d = x.GetData(); return d != null ? (int)d.CardGrade : 999; }).ToList();
            case SortType.LevelDesc:
                return list.OrderByDescending(x => x.Level).ToList();
            case SortType.TypeOrder:
                return list.OrderBy(x => { var d = x.GetData(); return d != null ? (int)d.CardType : 999; }).ToList();
            case SortType.Name:
                return list.OrderBy(x => { var d = x.GetData(); return d != null ? d.Name : ""; }).ToList();
            case SortType.Recent:
                return list.OrderByDescending(x => x.AcquiredTime).ToList();
            default:
                return list;
        }
    }
}