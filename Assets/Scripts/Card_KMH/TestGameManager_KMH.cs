using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class TestGameManager_KMH : MonoBehaviour
{
    public static TestGameManager_KMH Instance;
    
    // 카드 타입 별 동작
    private Dictionary<CardType, ICardAction> _cardTypeActions = new Dictionary<CardType, ICardAction>();
    // 카드 상태이상 별 동작
    private Dictionary<string, ICardAction> _cardStatusActions = new Dictionary<string, ICardAction>();

    // 덱 구성 (id, level)
    public Dictionary<int, int> Deck { get; private set; }

    private void Awake()
    {
        Instance = this;
        InitCardActions();      // 카드 동작 초기화
        InitDeck();             // 덱 초기화

    }
    // 동작 구성
    private void InitCardActions()
    {
        // 카드 타입
        _cardTypeActions.Add(CardType.Attack, new CardAttackAction());
        _cardTypeActions.Add(CardType.Healing, new CardHealingAction());
        _cardTypeActions.Add(CardType.Shield, new CardShieldAction());
        _cardTypeActions.Add(CardType.Spell, new CardSpellAction());

        // 카드 상태이상 임시
        _cardStatusActions.Add("KeyStatusPoison", new CardPoisonAction());
        _cardStatusActions.Add("KeyStatusBurn", new CardBurnAction());
        _cardStatusActions.Add("KeyStatusPride", new CardPrideAction());
        _cardStatusActions.Add("KeyStatusVulnerable", new CardVulnerableAction());
    }

    // 덱 구성
    private void InitDeck()
    {
        // 덱 생성
        Deck = new Dictionary<int, int>();

        // 모든 카드 데이터 한글 설정, 예외 처리
        foreach(var cardData in DataManager.Instance.CardDict)
        {
            cardData.Value.SetString();
            cardData.Value.SetStatusValue();
        }

        // 덱 구성 없으니 일단 카드 데이터 전부
        for (int i = 1; i <= DataManager.Instance.CardDict.Count; i++)
        {
            //int id = DataManager.Instance.CardDict[i].Id;
            // 기본 레벨 1
            Deck[i] = 1;
        }
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


    // id 카드의 레벨 반환
    public int GetCardLevel(int id)
    {
        return Deck[id];
    }


    // 카드 등급과 레벨에 따른 사용 가능 횟수 반환
    public int GetCardNumberOfAvailable(int level, CardGrade grade)
    {
        int numberOfAvailable;

        switch (grade)
        {
            case CardGrade.Common:
                numberOfAvailable = DataManager.Instance.GetCommonCardData(level).NumberOfAvailable;
                break;
            case CardGrade.Rare:
                numberOfAvailable = DataManager.Instance.GetRareCardData(level).NumberOfAvailable;
                break;
            case CardGrade.Epic:
                numberOfAvailable = DataManager.Instance.GetEpicCardData(level).NumberOfAvailable;
                break;
            case CardGrade.Legendary:
                numberOfAvailable = DataManager.Instance.GetLegendaryCardData(level).NumberOfAvailable;
                break;
            default:
                numberOfAvailable = 0;
                break;
        }

        return numberOfAvailable;
    }


    // 카드 강화 인벤토리용?
    public void UpgradePlayerCard(int id)
    {
        if (Deck.ContainsKey(id) == false)
        {
            Debug.LogError($"덱에 존재하지 않는 카드 입니다");
            return;
        }

        if(Deck[id] == 5)
        {
            Debug.Log("최대 강화 입니다.");
            return;
        }

        Deck[id]++;
    }
    public void UpgradeMonsterCard(int id)
    {

        if (Deck[id] == 999)
        {
            Debug.Log("최대 강화 입니다.");
            return;
        }

        Deck[id]++;
    }
}
