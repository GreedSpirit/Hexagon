using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [Header("Dungeon Data")]
    private DungeonData _currentDungeonData;
    private int _currentStageIndex = 0;

    [Header("Spawn Settings")]
    [SerializeField] private Transform _monsterSpawnPoint;
    [SerializeField] private GameObject _monsterPrefab; // 몬스터 베이스 프리팹
    [SerializeField] private BattleManager _battleManager; // 현재 소환된 몬스터를 할당해주기 위해 참조
    [SerializeField] private RewardInteraction _rewardInteraction;
    [SerializeField] private Animator _rewardObjAnimator; // 금서의 애니메이션을 넣을 위치
    [SerializeField] private RewardResultUI _rewardResultUI;
    [SerializeField] private GameObject _rewardCanvas;

    private MonsterStatus _currentActiveMonster;
    private StageData _tempStageData;
    private List<DeterminedReward> _determinedRewards;

    void Start()
    {
        // DungeonPresenter에서 던전 슬롯 클릭 시 설정된 Id를 통해 던전 데이터를 불러온다.
        int dungeonId = DungeonSessionData.SelectedDungeonId;
        if (dungeonId == -1)
        {
            Debug.LogWarning("던전ID가 -1입니다. 뭔가 문제가 있을 수 있습니다.");
        }
        _currentDungeonData = DataManager.Instance.GetDungeon(dungeonId);
        //_tempStageData = StageDataManager.Instance.GetStageByDungeonKey(_currentDungeonData.DungeonKey)[_currentStageIndex];

        StartStage(_currentStageIndex);
    }

    private void StartStage(int index)
    {
        //스테이지 리스트를 CSV에서 List 형태로 갖고 있다고 가정
        if(index >= _currentDungeonData.NumberOfStages)
        {
            //모든 스테이지 클리어 로직
            Debug.Log("All Stage Clear");
            return;
        }
        _tempStageData = StageDataManager.Instance.GetStageByDungeonKey(_currentDungeonData.DungeonKey)[_currentStageIndex];
        int targetMonsterId = DataManager.Instance.GetMonsterStatData(_tempStageData.SpawnMonster).Id;
        SpawnMonster(targetMonsterId);
    }

    private void SpawnMonster(int monsterId)
    {
        //몬스터 생성(현재 monsterStatus에서 정보를 갱신하고 초기화 해주니까 괜찮을 것 같은데 테스트 필요)
        GameObject obj = Instantiate(_monsterPrefab, _monsterSpawnPoint.position, Quaternion.identity);
        _currentActiveMonster = obj.GetComponent<MonsterStatus>();

        _currentActiveMonster.InitMonsterStatus(monsterId);
        _battleManager.SetMonster(_currentActiveMonster);
        //여기서 해당 몬스터가 죽거나 했을 때 이벤트를 등록시켜주면 될 듯?
        _currentActiveMonster.OnMonsterDeath += HandleMonsterDeath;
    }

    private void HandleMonsterDeath(MonsterStatus monster)
    {
        monster.OnMonsterDeath -= HandleMonsterDeath; // 죽으면 파괴 되거나 비활성화 될테니까 구독 해제

        // 보스였는지 확인
        if(monster.MonsterData.MonGrade == MonsterGrade.Boss)
        {
            Debug.Log("Boss Clear! Next Logic is RewardRoom");

            // [추가] 1회차 클리어 플래그 ON
            if (Player.Instance != null)
            {
                Player.Instance.IsFirstDungeonCleared = true;
            }

            //보상 방으로 가는 로직
            StartCoroutine(BossClearSequence());
        }
        else
        {
            Debug.Log("Stage " + (_currentStageIndex + 1) + "Clear!");
            _currentStageIndex++;
            //다음 스테이지 불러오기
            StartCoroutine(NextStageRoutine());
        }
    }

    private IEnumerator NextStageRoutine()
    {
        yield return new WaitForSeconds(3.0f); //몬스터가 죽고 나서 대기 시간이 필요할 것 같아서 넣은 대기 시간

        //몬스터 죽었으니까 삭제
        if(_currentActiveMonster != null)
        {
            Destroy(_currentActiveMonster.gameObject);
        }

        StartStage(_currentStageIndex);
    }

    private IEnumerator BossClearSequence()
    {
        yield return new WaitForSeconds(3.0f); //몬스터가 죽고 나서 대기 시간이 필요할 것 같아서 넣은 대기 시간

        // 1. 보스를 잡고 보상을 확정
        _determinedRewards = RewardDataManager.Instance.GenerateDungeonRewards(_currentDungeonData);

        // 2. 보상 인터랙션을 진행
        if(_rewardObjAnimator != null) _rewardObjAnimator.gameObject.SetActive(true);
        bool isInteractionDone = false;
        _rewardInteraction.OnInteractionComplete = () => {isInteractionDone = true;};
        _rewardInteraction.StartInteraction();

        // 인터랙션 끝날 때까지 대기
        yield return new WaitUntil(() => isInteractionDone);

        // 3. 금서 애니메이션 진행
        if(_rewardObjAnimator != null)
        {
            _rewardObjAnimator.SetTrigger("Done");

            yield return null;


            float animTime = _rewardObjAnimator.GetCurrentAnimatorStateInfo(0).length; // base layer에서 현재 실행되고 있는 애니메이션의 시간 반환

            if (_rewardObjAnimator.IsInTransition(0))
            {
                animTime = _rewardObjAnimator.GetNextAnimatorStateInfo(0).length;
            }

            yield return new WaitForSeconds(animTime);
        }

        // 4. 최종 보상 결과창 띄우기
        _rewardCanvas.SetActive(true);
        _rewardResultUI.Init(_determinedRewards, this);
    }

    public void GetRewards()
    {
        foreach (var reward in _determinedRewards)
        {
            if(reward.RewardType == "Card" || reward.RewardType == "BossCard")
            {
                CardManager.Instance.AddCard(reward.ItemId, reward.Amount);
            }
            else if(reward.RewardType == "Currency")
            {
                Player.Instance.GetExp(_currentDungeonData.Exp);
                if(Player.Instance.GetMoney() + reward.Amount > 999999)
                {
                    _rewardResultUI.OnMoneyOverText();
                }
                Player.Instance.PlusMoney(reward.Amount);
            }
        }
    }


}
