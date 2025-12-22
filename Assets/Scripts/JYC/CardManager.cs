using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    // 플레이어가 소지 중인 카드 정보 (Key: 카드 ID, Value: 보유 개수/레벨 등 정보 객체)
    public List<UserCard> UserCardList { get; private set; } = new List<UserCard>();

    // 현재 편성된 덱 (카드 ID 리스트)
    public List<int> CurrentDeck { get; private set; } = new List<int>();

    // 최대 덱 용량 (임시)
    public const int MAX_DECK_COUNT = 30;

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
        // 없으면 추가 (최대 개수 확인)
        else
        {
            if (CurrentDeck.Count >= MAX_DECK_COUNT) return false;

            // 보유한 카든지 확인
            var userCard = UserCardList.Find(x => x.CardId == cardId);
            if (userCard != null)
            {
                CurrentDeck.Add(cardId);
                return true;
            }
        }
        return false;
    }

    // 카드 레벨업 (UpgradeManager에서 호출)
    public bool TryUpgradeCard(int cardId)
    {
        var userCard = UserCardList.Find(x => x.CardId == cardId);
        if (userCard == null) return false;

        // 레벨업 로직
        userCard.Level++;
        Debug.Log($"카드 {cardId} 레벨업! Lv.{userCard.Level}");
        return true;
    }

    // 특정 카드의 레벨 가져오기
    public int GetCardLevel(int cardId)
    {
        var userCard = UserCardList.Find(x => x.CardId == cardId);
        return userCard != null ? userCard.Level : 1;
    }
}
