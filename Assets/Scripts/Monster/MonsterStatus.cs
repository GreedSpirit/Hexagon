using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 소환된 몬스터의 상태(체력, 방어력 등)를 관리하는 클래스
/// </summary>

public class MonsterStatus : MonoBehaviour, IBattleUnit
{
    public MonsterData MonsterData => _monsterData; //외부에서 몬스터 데이터에 접근할 수 있는 프로퍼티
    [SerializeField] private int _monsterId; //몬스터 데이터 ID
    [SerializeField] private int _monsterLevel; //추후 스테이지 테이블에서 불러올 내용
    [SerializeField] private MonsterGrade _monsterGrade;
    [SerializeField] private int _monsterMaxHP;
    [SerializeField] private int _monsterCurHP;
    public int MonsterCurHP => _monsterCurHP; // 외부에서 몬스터의 현재 체력에 접근할 수 있는 프로퍼티
    [SerializeField] private int _monsterDefense;
    [SerializeField] private int _monsterShield = 0;
    [SerializeField] private float _monsterBuff = 0;
    [SerializeField] private float _monsterDebuff = 0;
    [SerializeField] private MonsterData _monsterData;
    [SerializeField] private MonsterSkillSetData _monsterSkillSet;
    [SerializeField] private MonsterStatData _monsterStatData;
    [SerializeField] private CardData _currentSkillData; //현재 사용될 예정인 스킬 카드 데이터

    private List<IMonsterHpObserver> _hpObservers = new List<IMonsterHpObserver>(); //옵저버 목록을 관리할 List

    private string _selectedSkillKey = null; //선택된 스킬 키 임시 변수
    private int _selectedSkillSlot = -1; //선택된 스킬 슬롯 인덱스 임시 변수
    private int _selectedSkillValue = 0;

    public event System.Action OnEnemyActTurnEnd; // 몬스터 턴 종료 시점에 발행할 이벤트

    public void ChangePhase(PhaseType newPhase) //OnPhaseChanged 이벤트 구독용 함수
    {
        //유저 드로우 턴 페이즈일 때 행동
        if(newPhase == PhaseType.Draw || newPhase == PhaseType.Start)
        {
            GetRandomSkillFromSet();
        }
        else if(newPhase == PhaseType.EnemyAct)
        {
            UseSkill();
            OnEnemyActTurnEnd?.Invoke(); //몬스터 턴 종료 이벤트 발행
        }
    }

    public void InitMonsterStatus()
    {
        //추후 어떻게 사용할지에 따라서 중복 변수이기 때문에 DataManager에서 불러오는 방식으로 변경할 수도 있음
        _monsterGrade = _monsterData.MonGrade;

        if(_monsterGrade == MonsterGrade.Normal)
        {
            _monsterLevel = 3; //추후 스테이지 관련 테이블에서 갖고와서 레벨 설정하기
            _monsterStatData = DataManager.Instance.GetCommonMonsterStatData(_monsterLevel);

            _monsterMaxHP = Mathf.FloorToInt(_monsterStatData.Hp * _monsterData.HpRate); 
            _monsterDefense = Mathf.FloorToInt(_monsterStatData.Defense * _monsterData.DefRate);
        }
        else if(_monsterGrade == MonsterGrade.Boss)
        {
            _monsterLevel = 1; //추후 스테이지 관련 테이블에서 갖고와서 레벨 설정하기
            // 500은 임시 기본 체력 수치, 추후 _monsterLevel 값을 이용해서 BossMonsterStat 테이블에서 수치 갖고오기
            _monsterMaxHP = Mathf.FloorToInt(500 * _monsterData.HpRate);
            _monsterDefense = Mathf.FloorToInt(10 * _monsterData.DefRate);
        }
        _monsterSkillSet = DataManager.Instance.GetMonsterSkillSetData(_monsterData.SkillSet);
        _monsterCurHP = _monsterMaxHP;        
    }

    private void Awake()
    {
        // ID 방식 Key 방식 구별
        _monsterData = DataManager.Instance.GetMonsterStatData(1);
        //_monsterStatData = DataManager.Instance.GetMonsterStatData("KeyMonsterGhost0001");
        if(_monsterData == null)
        {
            Debug.LogError("몬스터 스탯 데이터를 불러오지 못했습니다. Id: 1");
            return;
        }
        else
        {
            Debug.Log("데이터 로드 성공");
            InitMonsterStatus();
            NotifyHpObservers(); //초기 HP 상태 갱신
        }
    }

    public void TakeDamage(int damage) // 데미지를 입을 때 호출할 함수
    {
        damage = (int)Mathf.Max((damage -  Mathf.Min(_monsterDefense, 7)) * (1 + _monsterDebuff), 0); //방어력 적용(방어력 최대가 7이라서 7까지만 적용)
        if(_monsterShield > 0)
        {
            int shieldDamage = Mathf.Min(_monsterShield, damage);
            _monsterShield -= shieldDamage;
            damage -= shieldDamage;
        }
        _monsterCurHP -= damage;
        if (_monsterCurHP < 0)
            _monsterCurHP = 0;
        NotifyHpObservers(); //HP 변경 알림

        if (_monsterCurHP == 0)
        {
            Death();
        }
    }

    public void TakeTrueDamage(int damage) // 방어력 무시 데미지(상태 이상 데미지)를 입을 때 호출할 함수
    {
        _monsterCurHP -= damage;
        if (_monsterCurHP < 0)
            _monsterCurHP = 0;
        NotifyHpObservers(); //HP 변경 알림

        if (_monsterCurHP == 0)
        {
            Death();
        }
    }

    public void GetHp(int hp) // 체력을 회복할 때 호출할 함수
    {
        _monsterCurHP += hp;
        if (_monsterCurHP > _monsterMaxHP)
            _monsterCurHP = _monsterMaxHP;
        NotifyHpObservers(); //HP 변경 알림
    }

    public void GetShield(int shield) // 방어막을 얻을 때 호출할 함수
    {
        _monsterShield += shield;
    }

    public void AddHpObserver(IMonsterHpObserver observer)
    {
        if (!_hpObservers.Contains(observer)) //방어 코드
        {
            _hpObservers.Add(observer);
        }
    }

    public void RemoveHpObserver(IMonsterHpObserver observer)
    {
        if (_hpObservers.Contains(observer)) //방어 코드
        {
            _hpObservers.Remove(observer);
        }
    }

    private void NotifyHpObservers() //옵저버들에게 HP 변경 알림
    {
        foreach (var observer in _hpObservers)
        {
            observer.OnMonsterHpChanged(_monsterCurHP, _monsterMaxHP);
        }
    }

    private void Death()
    {
        //몬스터 죽음 처리 로직 추가
        Debug.Log("몬스터가 죽었습니다. ID: " + _monsterId);
        if(_monsterGrade == MonsterGrade.Boss)
        {
            DropLoot();
        }
    }

    private void DropLoot()
    {
        //보스 몬스터 죽음 시 특별 처리 로직 추가
    }

    // 스킬 사용 및 공격 로직 시작 ------------------------------------------------------------

    private void GetRandomSkillFromSet() // 몬스터의 스킬셋에서 가중치에 따라 랜덤으로 스킬을 선택하는 함수
    {
        if(_monsterSkillSet == null || _monsterSkillSet.skillWeights.Count == 0)
        {
            Debug.LogError("몬스터 스킬셋 데이터가 없거나 스킬이 설정되어 있지 않습니다.");
            return;
        }
        if(_selectedSkillKey != null)
        {
            // 선턴을 잡을 경우 or 버그 방지 방어 코드 겸용
            Debug.Log("이미 선택된 스킬이 있습니다. 스킬 선택을 건너뜁니다.");
            return;
        }
        float totalRate = 0f;
        foreach(var skillweight in _monsterSkillSet.skillWeights)
        {
            totalRate += skillweight.Item2;
        }

        int idx = -1;
        float randomValue = Random.Range(0f, totalRate);
        float curSum = 0f;

        foreach(var skillSlot in _monsterSkillSet.skillWeights)
        {
            idx++;
            curSum += skillSlot.Item2;
            if(randomValue <= curSum)
            {
                Debug.Log("선택된 스킬 가중치: " + skillSlot.Item2);
                Debug.Log("선택된 스킬 레벨: " + _monsterSkillSet.skillLevels[idx]);
                _selectedSkillSlot = idx; //선택된 스킬 슬롯 인덱스 저장
                _selectedSkillKey = skillSlot.Item1;
                _currentSkillData = DataManager.Instance.GetCard(_selectedSkillKey); //선택된 스킬 카드 데이터 저장
                _selectedSkillValue = _currentSkillData.BaseValue + (_monsterSkillSet.skillLevels[_selectedSkillSlot] - 1) * _currentSkillData.ValuePerValue;
                //이곳에서 스킬 타입에 따른 아이콘과 수치 UI 갱신 로직 추가
                break;
            }
        }        
    }

    private void UseSkill() //턴마다 스킬을 사용하는 함수
    {
        //_selectedSkillKey를 이용해서 스킬을 가져오고 _selectedSkillSlot 인덱스를 이용해서 스킬 레벨도 가져오기
        if(_selectedSkillKey != null)
        {
            //스킬 사용 로직 추가
            Debug.Log("몬스터가 스킬을 사용했습니다. 스킬 키: " + _selectedSkillKey + ", 스킬 레벨: " + _monsterSkillSet.skillLevels[_selectedSkillSlot]);
            
            if(_currentSkillData.CardType == CardType.Attack)
            {
                //Player.Instance.TakeDamage(_selectedSkillValue);
                Debug.Log("플레이어에게 " + _selectedSkillValue + "의 데미지를 입혔습니다.");
            }
            else if(_currentSkillData.CardType == CardType.Healing)
            {
                GetHp(_selectedSkillValue);
                Debug.Log("몬스터가 " + _selectedSkillValue + "의 체력을 회복했습니다.");
            }
            else if(_currentSkillData.CardType == CardType.Shield)
            {
                GetShield(_selectedSkillValue);
                Debug.Log("몬스터가 " + _selectedSkillValue + "의 방어막을 얻었습니다.");
            }
            //사용 후 선택된 스킬 초기화 (방어 코드 및 선턴을 잡을 경우 예외 사항)
            _selectedSkillKey = null;
            _selectedSkillValue = 0;
            _selectedSkillSlot = -1;
        }
        else
        {
            Debug.LogError("선택된 스킬이 없습니다.");
        }
    }



    //! 유니티 에디터를 사용할 때 체력 소모 이벤트에 대한 테스트를 위한 코드 추후 삭제 필요
    #if UNITY_EDITOR
    public void TestTakeDamage()
    {
        TakeDamage(10);
    }

    public void TestSelectSkill()
    {
        GetRandomSkillFromSet();
    }

    public void TestUseSkill()
    {
        UseSkill();
    }
    #endif

}
