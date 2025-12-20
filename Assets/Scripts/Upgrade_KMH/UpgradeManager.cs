using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [SerializeField] UpgradeCardUI _cardUIPrefab;     // 카드 UI 프리팹
    [SerializeField] Transform _cardScrollView;       // 카드 목록 스크롤뷰

    private List<UpgradeCardUI> _playerCards = new List<UpgradeCardUI>();

    private UpgradeCardUI _upgradeCardUI;       // 선택 카드

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private IEnumerator Start()
    {
        yield return null;
        // 유저 카드 순회
        foreach(var card in CardManager.Instance.UserCardList)
        {
            // 카드 UI 하나 생성
            UpgradeCardUI cardUI = Instantiate(_cardUIPrefab, _cardScrollView);

            // 카드 데이터 가져오기
            CardData cardData = DataManager.Instance.GetCard(card.CardId);

            // UI 초기화
            cardUI.Init(cardData, this);

            // 리스트에 추가
            _playerCards.Add(cardUI);
        }
    }


    // 카드 강화 시도
    public void TryUpgradeCard(int cardId, Button button)
    {
        bool isUpgrade = CardManager.Instance.TryUpgradeCard(cardId);
    }
}
