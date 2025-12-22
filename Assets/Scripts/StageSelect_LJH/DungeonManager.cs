using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [Header("Dungeon Data")]
    private DungeonData _currentDungeonData;
    private int _currentStageIndex = 0;

    [Header("Spawn Settings")]
    [SerializeField] private Transform _monsterSpawnPoint;
    [SerializeField] private GameObject _monsterPrefab; // 몬스터 베이스 프리팹

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
            //보스를 처치한 순간 보상을 결정(이걸 DungeonSessionData로 보내서 다른 씬에서 보상 처리를 해도 좋고 보상 Panel이나 UI로 해결해도 될 듯)
            _determinedRewards = RewardDataManager.Instance.GenerateDungeonRewards(_currentDungeonData);
            //보상 방으로 가는 로직
            StartCoroutine(TransitionToRewardRoom());
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
        yield return new WaitForSeconds(2.0f); //몬스터가 죽고 나서 대기 시간이 필요할 것 같아서 넣은 대기 시간

        //몬스터 죽었으니까 삭제
        if(_currentActiveMonster != null) Destroy(_currentActiveMonster.gameObject);

        StartStage(_currentStageIndex);
    }

    private IEnumerator TransitionToRewardRoom()
    {
        yield return new WaitForSeconds(2.0f); //몬스터가 죽고 나서 대기 시간 or 스테이지 클리어 후 보상 방 이동 전 연출 대기
        Debug.Log("Reward Room Go Go");
        //보상 씬을 따로 만들어서 해당 던전 데이터를 불러와서 보상을 보여주거나 숨겨놨던 보상 UI를 활성화 해도 될 듯

    }


}
