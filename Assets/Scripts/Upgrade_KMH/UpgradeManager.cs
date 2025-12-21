using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("강화 카드 UI 프리팹")]
    [SerializeField] UpgradeCardUI _cardUIPrefab;     // 카드 UI 프리팹
    [Header("카드 리스트 스크롤 뷰")]
    [SerializeField] Transform _cardScrollView;       // 카드 목록 스크롤뷰
    [Header("강화 슬롯 카드")]
    [SerializeField] UpgradeCardUI _upgradeSlotCard;  // 강화 슬롯 카드
    [Header("재화 텍스트")]
    [SerializeField] TextMeshProUGUI _cardCountText;  // 카드
    [SerializeField] TextMeshProUGUI _goldText;       // 골드

    // 강화 UI 플레이어 보유 카드 리스트
    private List<UpgradeCardUI> _playerCards = new List<UpgradeCardUI>();
    private UpgradeCardUI _selectedCard;       // 선택중인 카드

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private IEnumerator Start()
    {
        yield return null;
        // 유저 보유 카드 순회
        foreach(UserCard card in TestCardManager.Instance.UserCardList)
        {
            // 카드 UI 하나 생성
            UpgradeCardUI cardUI = Instantiate(_cardUIPrefab, _cardScrollView);

            // UI 초기화
            cardUI.Init(card, this);

            // 리스트에 추가
            _playerCards.Add(cardUI);
        }

        // 강화 슬롯 카드 초기화
        _upgradeSlotCard.Init(null, this);
    }

    // 카드 재정렬 (UI 온, 강화 연출 완료)
    // 기본 - 우선순위 내 높은 등급 순
    // 1순위 - 강화에 필요한 모든 재화 충족
    // 2순위 - 카드 충족, 골드 부족
    // 3순위 - 골드 충족, 카드 부족
    // 최하위 - 최대 강화
    // 표시 안함 - 골드와 카드 모두 부족
    public void SortList()
    {

    }


    // 카드 선택
    public void SelectCard(UpgradeCardUI card, int cardId)
    {
        // 강화 슬롯 카드 꺼져있으면 켜기
        if (_upgradeSlotCard.gameObject.activeSelf == false) _upgradeSlotCard.gameObject.SetActive(true);

        // 기존 카드 선택 해지
        _selectedCard?.DeSelect();

        // 선택 카드 교체
        _selectedCard = card;

        // UserCard 가져오기
        UserCard userCard = TestCardManager.Instance.GetCard(cardId);

        // 카드 선택 실행
        _upgradeSlotCard.Select(userCard);
        _selectedCard.Select();
    }


    // 카드 강화 시도
    public void TryUpgradeCard(int cardId, Button button)
    {
        bool isUpgrade = TestCardManager.Instance.TryUpgradeCard(cardId);
    }


    // 강화 UI On
    public void OpenUpgradeUI()
    {
        // 정렬
        SortList();
    }

    // 강화 UI Off
    public void CloseUpgradeUI()
    {
        // 기존 카드 선택 해지
        _selectedCard?.DeSelect();

        // 강화 슬롯 카드 끔
        _upgradeSlotCard.gameObject.SetActive(false);
    }
}
