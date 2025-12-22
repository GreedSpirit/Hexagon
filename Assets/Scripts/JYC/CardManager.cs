using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnApplicationQuit()
    {
        SaveGame();
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
    public bool IsDeckValid(int requiredCount)
    {
        // 카드 개수 체크
        if (CurrentDeck.Count != requiredCount)
            return false;

        // (추후 추가) 중복 카드 제한이나 코스트 제한 등이 있다면 여기서 체크

        return true;
    }

    // 저장 데이터 포맷 클래스
    [System.Serializable]
    public class SaveData
    {
        public List<UserCard> myCards; // 내 보유 카드
        public List<int> myDeck;       // 내 덱 구성
    }

    // 게임 저장하기 (JSON 방식)
    [ContextMenu("Save Game")] // 유니티 에디터 인스펙터에서 우클릭으로 실행 가능
    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.myCards = this.UserCardList;
        data.myDeck = this.CurrentDeck;

        // JSON 변환
        string json = JsonUtility.ToJson(data, true); // true는 보기 좋게 줄바꿈 함

        // 파일 저장 경로 (PC: AppData, Mobile: 앱 내부 저장소)
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
