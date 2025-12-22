using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DeterminedReward
{
    public string ItemKey;
    public string RewardType;
    public int Amount;
    public int ItemId; //AddCard 등에 사용할 ID
}

public class RewardDataManager : MonoBehaviour
{
    public static RewardDataManager Instance;
    public List<RewardData> AllRewardDataList = new List<RewardData>();
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        AllRewardDataList = CSVReader.Read<RewardData>("Reward");
    }

    //던전 테이블의 리워드 그룹 인자값
    public List<RewardData> GetRewardGroupByDungeonRewardGroup(int rewardgroup)
    {
        return AllRewardDataList.Where(x => x.RewardGroup == rewardgroup).ToList();
    }

    // 보스 처치 후 혹은 던전 입장 시에 미리 보상을 결정하는 함수(DungeonManager에서 사용 예정)
    public List<DeterminedReward> GenerateDungeonRewards(DungeonData dungeonData)
    {
        List<DeterminedReward> selectedRewards = new List<DeterminedReward>();

        int openSlotCount = CalculateOpenSlots(dungeonData); //슬롯 갯수 결정

        // 리워드 그룹에 따른 보상 데이터 필터링
        var groupRewards = GetRewardGroupByDungeonRewardGroup(dungeonData.RewardGroup);
        // 일반 카드 보상을 선택하기 위한 데이터 필터링
        var cardPool = groupRewards.FindAll(r => r.RewardType == "Card");
        // 일반 카드 보상 선택
        for(int i = 0; i < openSlotCount; i++)
        {
            if(cardPool.Count == 0)
            {
                Debug.LogWarning("RewardCardPool is empty");
                break;
            }

            var picked = cardPool[Random.Range(0, cardPool.Count)];
            selectedRewards.Add(CreateRewardInstance(picked));
        }

        // 골드 or 퀘스트 보상 체크 (Currency 타입)
        var currencyPool = groupRewards.FindAll(r => r.RewardType == "Currency");
        foreach(var reward in currencyPool)
        {
            if (!string.IsNullOrEmpty(reward.Quest))
            {
                // 이곳에서 플레이어 혹은 게임 매니저에게 현재 보유한 퀘스트 키를 받아와서 체크
                // if(!QuestManager.Instance.HasQuest(reward.Quest)) continue; 요런 느낌
            }

            //currency의 확률이 1이라는 보장이 없으므로 체크
            if(Random.value <= reward.Probability)
            {
                selectedRewards.Add(CreateRewardInstance(reward));
            }
        }

        // 보스 카드 보상 체크
        var bossCardPool = groupRewards.FindAll(r => r.RewardType == "BossCard");
        if(bossCardPool.Count == 0)
        {
            Debug.LogWarning("BossCardPool is empty");
        }

        float total = 0;
        foreach(var b in bossCardPool) total += b.Probability;
        if(total != 1)
        {
            Debug.LogWarning("Boss Card Probability total is not 1");
        }
        float randomPro = Random.value * total; // 이렇게 하면 총 확률이 1이 아니어도 1인 것처럼 가능할듯
        float curPro = 0;

        foreach(var b in bossCardPool)
        {
            curPro += b.Probability;
            if(randomPro <= curPro)
            {
                selectedRewards.Add(CreateRewardInstance(b));
                break;
            }
        }

        return selectedRewards;
    }

    private int CalculateOpenSlots(DungeonData data)
    {
        //독립적 확률 계산이 아닌 순차적 계산
        float[] probs = {data.Slot1Probability, data.Slot2Probability, data.Slot3Probability, data.Slot4Probability};
        int count = 0;

        foreach(float p in probs)
        {
            if(p <= 0) break;
            if(Random.value <= p) count++;
            else break;
        }
        return count;
    }

    private DeterminedReward CreateRewardInstance(RewardData data)
    {
        return new DeterminedReward
        {
            ItemId = data.Id,
            ItemKey = data.RewardItem,
            RewardType = data.RewardType,
            Amount = CalculateAmount(data.MinAmount, data.MaxAmount, data.Measure)
        };
    }

    private int CalculateAmount(int min, int max, int measure)
    {
        if(measure <= 0) measure = 1;
        int minStep = min / measure;
        int maxStep = max / measure;
        return Random.Range(minStep, maxStep + 1) * measure;
    }
}
