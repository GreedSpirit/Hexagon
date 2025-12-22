using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

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
    [Header("강화 버튼")]
    [SerializeField] Button _upgradeButton;           // 강화 버튼
    [Header("강화 연출 테스트")]
    [SerializeField] Image _upgradeCover;             // 연출 이미지

    // 임시 골드 재화
    public int Gold { get; private set; }

    private CardManager _cardManager;

    // 강화 UI 플레이어 보유 카드 리스트
    private List<UpgradeCardUI> _playerCards = new List<UpgradeCardUI>();
    private UpgradeCardUI _selectedCard;       // 선택중인 카드

    private Coroutine upgradeCoroutine;        // 강화 코루틴

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private IEnumerator Start()
    {
        _cardManager = CardManager.Instance;

        yield return null;
        // 유저 보유 카드 순회
        foreach(UserCard card in _cardManager.UserCardList)
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

    // 카드 리스트 새로고침 (UI 온, 강화 연출 완료)
    // 기본 - 우선순위 내 높은 등급 순
    // 1순위 - 강화에 필요한 모든 재화 충족
    // 2순위 - 카드 충족, 골드 부족
    // 3순위 - 골드 충족, 카드 부족
    // 최하위 - 최대 강화
    // 표시 안함 - 골드와 카드 모두 부족
    public void RefreshList()
    {
        // 선택 카드 있으면
        if (_selectedCard != null)
        {
            // 재화 UI 갱신
            UserCard userCard = _selectedCard.UserCard;
            UpdateCurrencyUI(userCard);

            // 선택 카드 텍스트 갱신
            _selectedCard.UpdateUpgradeText();
            _upgradeSlotCard.UpdateUpgradeText();
        }

        foreach(var card in _playerCards)
        {
            //card.UserCard
        }
    }


    // 카드 선택
    public void SelectCard(UpgradeCardUI card)
    {
        // 연출 진행중이면 선택 불가
        if (upgradeCoroutine != null) return;

        // 강화 슬롯 카드 꺼져있으면 켜기
        if (_upgradeSlotCard.gameObject.activeSelf == false) _upgradeSlotCard.gameObject.SetActive(true);

        // 기존 카드 선택 해지
        _selectedCard?.DeSelect();

        // 선택 카드 교체
        _selectedCard = card;

        // UserCard 가져오기
        UserCard userCard = _selectedCard.UserCard;

        UpdateCurrencyUI(userCard);

        // 카드 선택 실행
        _upgradeSlotCard.Select(userCard);
        _selectedCard.Select();
    }


    // 카드 강화 시도
    public void TryUpgradeCard()
    {
        if (_selectedCard == null) return;

        UserCard userCard = _selectedCard.UserCard;

        // 필요 재화량
        int reqCard = GetReqCardAmount(userCard.CardId, userCard.Level);
        int reqGold = GetReqGoldAmount(userCard.CardId, userCard.Level);

        // 재화 체크
        bool isCardEnough = userCard.Count - 1 >= reqCard;
        bool isGoldEnough = Gold >= reqGold;

        // 애초에 버튼 비활성화겠지만 혹시 몰라서 방어
        if (isCardEnough == false || isGoldEnough == false) return;

        // 재화 소모
        userCard.Count -= reqCard;
        Gold -= reqGold;

        // 카드 레벨 증가
        userCard.Level++;

        // 연출 중이라면 멈춤 (방어)
        if (upgradeCoroutine != null)
            StopCoroutine(upgradeCoroutine);

        // 강화 연출 시작
        upgradeCoroutine = StartCoroutine(UpgradeCard());
    }

    private IEnumerator UpgradeCard()
    {
        // 연출 끝날 때 까지 버튼 잠금
        _upgradeButton.interactable = false;

        float fadeTime = 1f; // 연출 시간
        float timer = 0f;

        // 시작 전 일단 투명하게
        Color tempColor = _upgradeCover.color;
        tempColor.a = 0f;
        _upgradeCover.color = tempColor;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;

            // 현재 색상 임시로 담음
            Color currentColor = _upgradeCover.color;

            currentColor.a = Mathf.Lerp(0f, 1f, timer / fadeTime);

            // 변경된 색 다시 이미지에 적용
            _upgradeCover.color = currentColor;

            yield return null;
        }

        // 마무리 불투명
        Color finalColor = _upgradeCover.color;
        finalColor.a = 1f;
        _upgradeCover.color = finalColor;
        
        // UI 리셋
        ResetUI();

        // 코루틴 비우기
        upgradeCoroutine = null;

        // 강화 리스트 새로고침
        RefreshList();
    }

    // 리셋 - 강화 연출 완료, UI 강제 종료
    private void ResetUI()
    {
        // 투명하게
        Color tempColor = _upgradeCover.color;
        tempColor.a = 0f;
        _upgradeCover.color = tempColor;

        // 버튼 비활성화
        _upgradeButton.interactable = false;

        // 재화 텍스트
        _cardCountText.text = "- / -";
        _goldText.text = "- / -";
    }

    // 강화 UI Off (닫기 버튼)
    public void CloseUpgradeUI()
    {
        // 리스트 새로고침
        RefreshList();

        // 기존 카드 선택 해지
        _selectedCard?.DeSelect();
        _selectedCard = null;

        // 강화 슬롯 카드 비활성화
        _upgradeSlotCard.gameObject.SetActive(false);

        // 연출 도중에 꺼져서 코루틴 남아있을 수 있음
        if (upgradeCoroutine != null)
        {
            StopCoroutine(upgradeCoroutine);
            upgradeCoroutine = null;
        }

        // UI 리셋
        ResetUI();
    }



    // 재화 UI 갱신
    private void UpdateCurrencyUI(UserCard userCard)
    {
        // 필요 재화량
        int reqCard = GetReqCardAmount(userCard.CardId, userCard.Level);
        int reqGold = GetReqGoldAmount(userCard.CardId, userCard.Level);

        // 재화 상태 체크
        bool isCardEnough = userCard.Count - 1 >= reqCard;
        bool isGoldEnough = Gold >= reqGold;

        // 텍스트 적용
        _cardCountText.text = $"{userCard.Count - 1} / {reqCard}";
        _goldText.text = Gold.ToString("N0") + " / " + reqGold.ToString("N0");

        // 색상 적용
        _cardCountText.color = isCardEnough ? Color.white : Color.red;
        _goldText.color = isGoldEnough ? Color.white : Color.red;

        // 강화 버튼 상태 변경
        _upgradeButton.interactable = isCardEnough && isGoldEnough;
    }



    // 필요 카드량 반환
    private int GetReqCardAmount(int cardId, int level)
    {
        // 강화 데이터 가져오기
        UpgradeData upgradeData = GetUpgradeData(cardId, level);

        return upgradeData.ReqCardAmount;
    }
    // 필요 골드량 반환
    private int GetReqGoldAmount(int cardId, int level)
    {
        // 강화 데이터 가져오기
        UpgradeData upgradeData = GetUpgradeData(cardId, level);

        return upgradeData.ReqCurrencyAmount;
    }


    // 강화 데이터 반환 (id, level)
    private UpgradeData GetUpgradeData(int cardId, int level)
    {
        // ID 카드 등급 가져오기
        CardGrade grade = DataManager.Instance.GetCard(cardId).CardGrade;

        // 등급에 맞는 강화 데이터 가져오기
        switch (grade)
        {
            case CardGrade.Common:
                return DataManager.Instance.GetCommonCardUpgradeData(level);
            case CardGrade.Rare:
                return DataManager.Instance.GetRareCardUpgradeData(level);
            case CardGrade.Epic:
                return DataManager.Instance.GetEpicCardUpgradeData(level);
            case CardGrade.Legendary:
                return DataManager.Instance.GetLegendaryCardUpgradeData(level);
            default:
                return null;
        }
    }


    
    // 테스트용 카드 추가
    public void AddCard(int amount)
    {
        if (_selectedCard == null) return;

        UserCard userCard = _selectedCard?.UserCard;

        // 카드 추가
        CardManager.Instance.AddCard(userCard.CardId, amount);

        // 필요 재화량
        int reqCard = GetReqCardAmount(userCard.CardId, userCard.Level);
        int reqGold = GetReqGoldAmount(userCard.CardId, userCard.Level);

        // 재화 체크
        bool isCardEnough = userCard.Count - 1 >= reqCard;
        bool isGoldEnough = Gold >= reqGold;

        // 텍스트 적용
        _cardCountText.text = $"{userCard.Count - 1} / {reqCard}";

        // 색상 적용
        _cardCountText.color = isCardEnough ? Color.white : Color.red;

        // 강화 버튼 상태 변경
        _upgradeButton.interactable = isCardEnough && isGoldEnough;
    }

    // 테스트용 골드 추가
    public void AddGold(int amount)
    {
        Gold += amount;

        if (_selectedCard == null) return;

        UserCard userCard = _selectedCard?.UserCard;

        // 필요 재화량
        int reqCard = GetReqCardAmount(userCard.CardId, userCard.Level);
        int reqGold = GetReqGoldAmount(userCard.CardId, userCard.Level);

        // 재화 체크
        bool isCardEnough = userCard.Count >= reqCard;
        bool isGoldEnough = Gold >= reqGold;

        // 텍스트 적용
        _goldText.text = Gold.ToString("N0") + " / " + reqGold.ToString("N0");

        // 색상 적용
        _goldText.color = Gold >= reqGold ? Color.white : Color.red;

        // 강화 버튼 상태 변경
        _upgradeButton.interactable = isCardEnough && isGoldEnough;
    }
}
