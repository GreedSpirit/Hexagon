using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance { get; private set; }

    public event System.Action OnEnterBossStage;
    public bool IsRewardSequenceActive { get; private set; } = false;

    [Header("Dungeon Data")]
    private DungeonData _currentDungeonData;
    private int _currentStageIndex = 0;

    [Header("Spawn Settings")]
    [SerializeField] private Transform _monsterSpawnPoint;
    [SerializeField] private GameObject _monsterPrefab; // 몬스터 베이스 프리팹
    [SerializeField] private BattleManager _battleManager; // 현재 소환된 몬스터를 할당해주기 위해 참조


    [Header("Reward Settings")]
    [SerializeField] private RewardInteraction _rewardInteraction;
    [SerializeField] private Animator _rewardObjAnimator; // 금서의 애니메이션을 넣을 위치
    [SerializeField] private RewardResultUI _rewardResultUI;
    [SerializeField] private GameObject _rewardCanvas;

    [Header("Stage UI")]
    [SerializeField] private Sprite[] _stageIndexSprites;
    [SerializeField] private Image _stageIndexUI;

    private MonsterStatus _currentActiveMonster;
    private StageData _tempStageData;
    private List<DeterminedReward> _determinedRewards;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // DungeonPresenter에서 던전 슬롯 클릭 시 설정된 Id를 통해 던전 데이터를 불러온다.
        int dungeonId = DungeonSessionData.SelectedDungeonId;
        if (dungeonId == -1)
        {
            Debug.LogWarning("던전ID가 -1입니다.");
        }

        _currentDungeonData = DataManager.Instance.GetDungeon(dungeonId);

        // 변경: 바로 스테이지 시작 → 시나리오 후 시작
        Player.Instance.PlayScenario(Trigger_Type.dungeonenter,() =>{StartStage(_currentStageIndex);} );

    }

    public int GetDungeonId()
    {
        return DungeonSessionData.SelectedDungeonId;
    }

    public int GetCurrentStageIndex()
    {
        return _currentStageIndex;
    }

    public string GetCurrentDungeonName()
    {
        if(_currentDungeonData.Name != null)
        {
            return DataManager.Instance.GetString(_currentDungeonData.Name).Korean;
        }
        return "Unknown Dungeon";
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
        _stageIndexUI.sprite = _stageIndexSprites[index];
        _tempStageData = StageDataManager.Instance.GetStageByDungeonKey(_currentDungeonData.DungeonKey)[_currentStageIndex];
        int targetMonsterId = DataManager.Instance.GetMonsterStatData(_tempStageData.SpawnMonster).Id;
        SpawnMonster(targetMonsterId);
        
        //_battleManager.StartBattle();
    }

    private void SpawnMonster(int monsterId)
    {
        GameObject obj = Instantiate(_monsterPrefab, _monsterSpawnPoint.position, Quaternion.identity);
        _currentActiveMonster = obj.GetComponent<MonsterStatus>();

        _currentActiveMonster.InitMonsterStatus(monsterId);
        _battleManager.SetMonster(_currentActiveMonster);

        if (_currentActiveMonster.MonsterData.MonGrade == MonsterGrade.Boss)
        {
            SoundManager.Instance.PlayBGM(BGMType.Boss);

            
            Player.Instance.PlayScenario(Trigger_Type.prebattle,() =>{ Player.Instance.EnterBattleMod(); _battleManager.StartBattle(); });
        }
        else
        {
            Player.Instance.EnterBattleMod();
            _battleManager.StartBattle(); // 일반 몬스터는 그대로
        }

        _currentActiveMonster.OnMonsterDeath += HandleMonsterDeath;
    }


    private void HandleMonsterDeath(MonsterStatus monster)
    {
        monster.OnMonsterDeath -= HandleMonsterDeath; // 죽으면 파괴 되거나 비활성화 될테니까 구독 해제

        // 보스였는지 확인
        if(monster.MonsterData.MonGrade == MonsterGrade.Boss)
        {
            Debug.Log("Boss Clear! Next Logic is RewardRoom");

            //보상 방으로 가는 로직
            StartCoroutine(BossClearSequence());
        }
        else
        {
            Debug.Log("Stage " + (_currentStageIndex + 1) + " Clear!");

            // 변경: 바로 다음 스테이지 가지 않고 시나리오 시작
            Player.Instance.PlayScenario(
                Trigger_Type.stageenter,
                () =>
                {
                    _currentStageIndex++;                 
                    StartCoroutine(NextStageRoutine());
                }
            );
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
        yield return new WaitForSeconds(3.0f);

        
        Player.Instance.PlayScenario(
            Trigger_Type.preseal,
            () =>
            {
                StartCoroutine(BossClearSequence_Internal());
            }
        );
    }

    private IEnumerator BossClearSequence_Internal()
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
        IsRewardSequenceActive = true;
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

        IsRewardSequenceActive = false;

        // [추가] 1회차 클리어 플래그 ON
        if (Player.Instance != null)
        {
            Player.Instance.DungeonClearedIndex = DungeonSessionData.SelectedDungeonId;
        }
        
        Player.Instance.PlayScenario(Trigger_Type.clear,() =>{Player.Instance.EnterMoveMod();});
    }


}
