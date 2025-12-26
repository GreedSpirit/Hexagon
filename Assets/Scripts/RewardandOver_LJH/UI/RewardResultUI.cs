using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewardResultUI : MonoBehaviour
{
    [Header("Left Section (Boss Card)")]
    [SerializeField] private GameObject _bossSection;
    [SerializeField] private Image _bossCardImage;
    [SerializeField] private TextMeshProUGUI _bossAmountText;

    [Header("Right Section (General Card)")]
    [SerializeField] private Transform _generalCardParent; // Horizontal Layout Group 위치
    [SerializeField] private GameObject _cardSlotPrefab;   // RewardSlotUI가 붙은 프리팹

    [Header("Right Section (Currency)")]
    [SerializeField] private Transform _currencyParent;    // Vertical Layout Group 위치
    [SerializeField] private GameObject _currencyTextPrefab; // 텍스트만 있는 프리팹
    [SerializeField] private GameObject _currencyIsOverText;

    [Header("Buttons")]
    [SerializeField] private Button _returnButton;

    private DungeonManager _dungeonManager; // GetRewards 호출용 참조

    public void Init(List<DeterminedReward> rewards, DungeonManager manager)
    {
        _dungeonManager = manager;
        _returnButton.onClick.RemoveAllListeners();
        _returnButton.onClick.AddListener(OnReturnButtonClicked);

        // UI 초기화 (기존 데이터 삭제)
        ClearUI();

        bool hasBossCard = false;

        foreach (var reward in rewards)
        {
            switch (reward.RewardType)
            {
                case "BossCard":
                    SetBossCard(reward);
                    hasBossCard = true;
                    break;

                case "Card":
                    CreateGeneralCardSlot(reward);
                    break;

                case "Currency":
                    CreateCurrencyText(reward);
                    break;
            }
        }

        // 보스 카드가 없으면 좌측 영역을 비우거나 숨김 처리 (기획에 따라 조정)
        if (!hasBossCard)
        {
             _bossSection.SetActive(false); // 혹은 빈 이미지로 두기
            _bossCardImage.color = Color.clear;
            _bossAmountText.text = "";
        }
    }

    private void SetBossCard(DeterminedReward reward)
    {
        // 보스 카드 이미지 설정 로직
        // _bossCardImage.sprite = ...
        _bossCardImage.color = Color.white;
        _bossAmountText.text = $"x {reward.Amount}";
    }

    private void CreateGeneralCardSlot(DeterminedReward reward)
    {
        GameObject obj = Instantiate(_cardSlotPrefab, _generalCardParent);
        RewardSlotUI slot = obj.GetComponent<RewardSlotUI>();
        
        // 아이템 ID(혹은 Key)를 이용해 슬롯 초기화
        slot.InitSlot(reward.ItemId, reward.Amount);
    }

    private void CreateCurrencyText(DeterminedReward reward)
    {
        GameObject obj = Instantiate(_currencyTextPrefab, _currencyParent);
        TextMeshProUGUI textComp = obj.GetComponent<TextMeshProUGUI>();

        // 텍스트 포맷: "아이템 이름 : 수량"
        // reward.ItemKey를 이용해 실제 한글 이름을 가져오는 로직 필요
        string displayName = reward.ItemKey; 
        
        textComp.text = $"{displayName} : {reward.Amount}";
    }

    public void OnMoneyOverText()
    {
        _currencyIsOverText.SetActive(true);
    }

    private void ClearUI()
    {
        foreach (Transform child in _generalCardParent) Destroy(child.gameObject);
        foreach (Transform child in _currencyParent) Destroy(child.gameObject);
        _currencyIsOverText.SetActive(false);
    }

    private void OnReturnButtonClicked()
    {
        //실제 보상 획득 로직 실행
        _dungeonManager.GetRewards();

        // [추가] 보상을 다 받았으니 저장
        GameSaveManager.Instance.SaveGame();
        Debug.Log("보상 획득 및 저장 완료");

        SceneManager.LoadScene(0);
    }
}