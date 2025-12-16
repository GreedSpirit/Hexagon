using System.Collections.Generic;
using UnityEngine;

public class TestGameManager_KMH : MonoBehaviour
{
    public static TestGameManager_KMH Instance;
    
    // 카드 타입 별 동작
    private Dictionary<CardType, ICardAction> _cardActions = new Dictionary<CardType, ICardAction>();

    // 덱 구성 (id, level)
    public List<Card> Deck { get; private set; }

    private void Awake()
    {
        Instance = this;
        InitCardActions();
        InitDeck();
    }

    private void InitCardActions()
    {
        _cardActions.Add(CardType.Attack, new CardAttackAction());
        //cardActions.Add(CardType.Healing, new CardHealingAction());
        //cardActions.Add(CardType.Shield, new CardShieldAction());
        //cardActions.Add(CardType.Spell, new CardSpellAction());

        Debug.Log("카드 각 타입 동작 초기화");
    }

    private void InitDeck()
    {
        // 테스트용 카드 10장
        int count = 10;

        // 덱 생성
        Deck = new List<Card>();

        // 덱 구성 없으니 일단 랜덤 Id 카드 생성
        for (int i = 0; i < count; i++)
        {
            Card card = new Card();
            card.Id = Random.Range(1, 4);
            Deck.Add(card);
        }
    }

    public ICardAction GetAction(CardType type)
    {
        if (_cardActions.TryGetValue(type, out ICardAction action))
            return action;

        Debug.LogWarning($"{type}에 해당하는 행동이 없습니다.");
        return null;
    }
}
