using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RewardUI : MonoBehaviour
{
    [SerializeField] GameObject _rewardPanel; // 보상 팝업창
    [SerializeField] Transform _slotParent;   // 보상 아이콘들이 생성될 위치
    [SerializeField] GameObject _cardSlotPrefab; // 보여줄 카드 아이콘 프리팹
    [SerializeField] Button _confirmButton;   // '확인(수령)' 버튼
    private List<DeterminedReward> _currentRewards;
    void Start()
    {
        _confirmButton.onClick.AddListener(CollectRewards);
        _rewardPanel.SetActive(false);
    }

    // 전투 승리 시 호출
    public void ShowRewards(List<DeterminedReward> rewards)
    {
        _currentRewards = rewards;
        _rewardPanel.SetActive(true);

        // _slotParent 아래에 슬롯 프리팹 생성해서 아이콘 보여주는 연출 코드 작성 (UI 시각화)
    }

    void CollectRewards()
    {
        if (_currentRewards == null) return;

        foreach (var reward in _currentRewards)
        {
            // 재화(골드)인 경우
            if (reward.RewardType == "Currency")
            {
                // 테이블 Key가 "KeyGold" 인지 확인하는 로직 추가 가능
                CardManager.Instance.AddGold(reward.Amount);
                Debug.Log($"골드 {reward.Amount} 획득");
            }
            // 카드인 경우
            else if (reward.RewardType == "Card" || reward.RewardType == "BossCard")
            {
                // reward.ItemKey (예: KeySkillSwipe01)를 이용해 CardData를 찾고 ID를 얻어야 함
                CardData cardInfo = DataManager.Instance.GetCard(reward.ItemKey);

                if (cardInfo != null)
                {
                    CardManager.Instance.AddCard(cardInfo.Id, reward.Amount);
                    Debug.Log($"{cardInfo.Name} 카드 {reward.Amount}장 획득");
                }
            }
        }

        // 획득 후 즉시 저장
        GameSaveManager.Instance.SaveGame();

        _rewardPanel.SetActive(false);

        // 마을로 돌아가거나 다음 스테이지로 가는 로직 호출 구현 필요
    }
}