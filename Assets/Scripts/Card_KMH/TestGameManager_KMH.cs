using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class TestGameManager_KMH : MonoBehaviour
{
    public static TestGameManager_KMH Instance;
    
    // 카드 타입 별 동작
    private Dictionary<CardType, ICardAction> _cardActions = new Dictionary<CardType, ICardAction>();

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
        _cardActions.Add(CardType.Attack, new CardAttackAction());
        _cardActions.Add(CardType.Healing, new CardHealingAction());
        _cardActions.Add(CardType.Shield, new CardShieldAction());
        _cardActions.Add(CardType.Spell, new CardSpellAction());
    }

    // 덱 구성
    private void InitDeck()
    {
        // 테스트용 카드 5장
        int count = 5;

        // 덱 생성
        Deck = new Dictionary<int, int>();

        // 덱 구성 없으니 일단 랜덤 Id 카드 생성
        for (int i = 1; i <= count; i++)
        {
            int randId = Random.Range(1, DataManager.Instance.CardDict.Count + 1);
            //int id = DataManager.Instance.CardDict[i].Id;
            // 기본 레벨 1
            Deck[randId] = 1;
        }
    }


    // 동작 반환
    public ICardAction GetAction(CardType type)
    {
        if (_cardActions.TryGetValue(type, out ICardAction action))
            return action;

        Debug.LogWarning($"{type}에 해당하는 행동이 없습니다.");
        return null;
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
