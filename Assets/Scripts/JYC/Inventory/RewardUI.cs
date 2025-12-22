using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RewardUI : MonoBehaviour
{
    [SerializeField] GameObject _rewardPanel; // 보상 팝업창
    [SerializeField] Transform _slotParent;   // 보상 아이콘들이 생성될 위치
    [SerializeField] GameObject _cardSlotPrefab; // 보여줄 카드 아이콘 프리팹
    [SerializeField] Button _confirmButton;   // '확인(수령)' 버튼

    void Start()
    {
        _confirmButton.onClick.AddListener(CollectRewards);
        _rewardPanel.SetActive(false);
    }

    // 전투 승리 시 호출
    public void ShowRewards(int dungeonRewardGroupId)
    {
        _rewardPanel.SetActive(true);
        // 여기서 DataManager.Instance.GetReward... 로 그룹 ID에 맞는 보상을 불러와서 슬롯 생성
        // (현재 RewardData 파싱 로직을 활용해야 함)
    }

    void CollectRewards()
    {
        // 실제 인벤토리에 아이템 넣기
        // 예: CardManager.Instance.AddCard(보상카드ID, 갯수);

        _rewardPanel.SetActive(false);
        // 마을로 돌아가기 or 다음 스테이지
    }
}