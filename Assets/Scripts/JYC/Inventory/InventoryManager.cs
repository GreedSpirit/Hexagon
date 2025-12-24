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

        // 이미 덱에 있는가? -> 해제
        if (IsCardInDeck(cardId))
        {
            UnequipCard(cardId);
            return true;
        }
        else
        {
            // 덱에 없는가? -> 등급 제한 체크 후 장착
            return EquipCard(cardId);
        }
    }
    // 카드 장착 (등급 제한 체크 포함)
    public bool EquipCard(int cardId)
    {
        var userCard = CardManager.Instance.GetCard(cardId);
        if (userCard == null) return false;

        CardData data = userCard.GetData();
        if (data == null) return false;


        //  등급별 개수 제한 체크

        int currentCount = GetDeckCountByGrade(data.CardGrade);
        int maxCount = GetMaxCountByGrade(data.CardGrade);

        // 이미 꽉 찼다면?
        if (currentCount >= maxCount)
        {
            Debug.Log($" {data.CardGrade} 등급 슬롯이 꽉 찼습니다! ({currentCount}/{maxCount})");
            // ( 여기에 UI 안내 메시지 띄우는 코드 추가 가능)
            return false; // 장착 실패
        }

        // 자리 있으면 장착 진행
        bool success = CardManager.Instance.ToggleDeckEquip(cardId);
        if (success)
        {
            OnDeckChanged?.Invoke();
        }
        return success;
    }
    // 카드 해제
    public void UnequipCard(int cardId)
    {
        CardManager.Instance.ToggleDeckEquip(cardId);
        OnDeckChanged?.Invoke();
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

        // 인덱스 안전장치
        if (slotIndex < 0 || slotIndex >= deck.Count) return;

        // 넣으려는 카드의 데이터 확인
        var newCard = CardManager.Instance.GetCard(newCardId);
        if (newCard == null) return;
        CardGrade newCardGrade = newCard.GetData().CardGrade;

        // 이미 덱에 있는 카드인가? (위치 이동 로직)
        int oldSlotIndex = deck.IndexOf(newCardId);

        if (oldSlotIndex != -1)
        {
            // 같은 자리면 무시
            if (oldSlotIndex == slotIndex) return;

            // 다른 자리에서 이사 오는 경우 -> 원래 자리를 비워줌 (-1)
            Debug.Log($" 카드 이동: {oldSlotIndex}번 슬롯 -> {slotIndex}번 슬롯");
            deck[oldSlotIndex] = -1;

            // (주의) 원래 자리를 비웠으니, 총 개수는 변하지 않음 -> 등급 제한 체크 불필요
        }
        else
        {
            // 덱에 없던 카드를 새로 끼우는 경우

            if (deck[slotIndex] == -1)
            {
                int currentCount = GetDeckCountByGrade(newCardGrade);
                int maxCount = GetMaxCountByGrade(newCardGrade);

                if (currentCount >= maxCount)
                {
                    Debug.Log($" {newCardGrade} 등급 슬롯이 꽉 찼습니다! ({currentCount}/{maxCount})");
                    // 실패했으니 아무것도 안 하고 종료
                    return;
                }
            }
        }

        // 최종 장착 (덮어쓰기)
        deck[slotIndex] = newCardId;

        Debug.Log($" 덱 슬롯[{slotIndex}] 설정 완료: {newCardId}");

        // 저장 및 UI 갱신
        CardManager.Instance.SaveGame();
        RefreshInventory();     // 인벤토리 E 마크 갱신
        InvokeDeckChanged();    // 덱 UI 갱신

        // 선택 상태 초기화
        DeselectAll();
    }
    // 현재 덱에 특정 등급 카드가 몇 장 있는지 세는 함수
    private int GetDeckCountByGrade(CardGrade grade)
    {
        int count = 0;
        // 들어온 등급이 '영웅'이나 '전설'이면 -> "고급 등급"으로 취급
        bool isHighTier = (grade == CardGrade.Epic || grade == CardGrade.Legendary);

        foreach (int id in CardManager.Instance.CurrentDeck)
        {
            if (id == -1) continue; // 빈 슬롯 패스

            var card = CardManager.Instance.GetCard(id);
            if (card != null)
            {
                CardGrade targetGrade = card.GetData().CardGrade;

                if (isHighTier)
                {
                    // 덱에 있는 카드가 '영웅'이든 '전설'이든 다 카운트
                    if (targetGrade == CardGrade.Epic || targetGrade == CardGrade.Legendary)
                    {
                        count++;
                    }
                }
                else
                {
                    // 일반, 희귀는 자기 것만 셈
                    if (targetGrade == grade)
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }

    // 등급별 최대 개수 (임시 하드코딩 - 나중에 던전 정보에서 가져오기)
    private int GetMaxCountByGrade(CardGrade grade)
    {
        switch (grade)
        {
            case CardGrade.Common:
                return 3; // 일반 3장
            case CardGrade.Rare:
                return 2; // 희귀 2장

            case CardGrade.Epic:
            case CardGrade.Legendary:
                return 1; // "영웅+전설 합쳐서" 1장만 가능 

            default: return 0;
        }
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