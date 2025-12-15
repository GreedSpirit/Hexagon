using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 소환된 몬스터의 상태(체력, 방어력 등)를 관리하는 클래스
/// </summary>

public class MonsterStatus : MonoBehaviour
{
    [SerializeField] private string _monsterName;
    [SerializeField] private int _monsterLevel; //추후 스테이지 테이블에서 불러올 내용
    [SerializeField] private MonsterGrade _monsterGrade;
    [SerializeField] private int _monsterMaxHP;
    [SerializeField] private int _monsterCurHP;
    [SerializeField] private int _monsterDefense;
    [SerializeField] private int _monsterShield = 0;
    [SerializeField] private MonsterStatData _monsterStatData;
    [SerializeField] private MonsterSkillSetData _monsterSkillSet;

    private List<IMonsterHpObserver> _hpObservers = new List<IMonsterHpObserver>(); //옵저버 목록을 관리할 List

    public void InitMonsterStatus()
    {
        //추후 어떻게 사용할지에 따라서 중복 변수이기 때문에 DataManager에서 미리 불러오는 방식으로 변경할 수도 있음
        _monsterName = _monsterStatData.Name;
        _monsterGrade = _monsterStatData.MonGrade;

        if(_monsterGrade == MonsterGrade.Normal)
        {
            _monsterLevel = 1; //추후 스테이지 관련 테이블에서 갖고와서 레벨 설정하기
            // 100은 임시 기본 체력 수치, 추후 _monsterLevel 값을 이용해서 CommonMonsterStat 테이블에서 수치 갖고오기
            _monsterMaxHP = Mathf.FloorToInt(100 * _monsterStatData.HpRate); 
            _monsterDefense = Mathf.FloorToInt(5 * _monsterStatData.DefRate);
        }
        else if(_monsterGrade == MonsterGrade.Boss)
        {
            _monsterLevel = 2; //추후 스테이지 관련 테이블에서 갖고와서 레벨 설정하기
            // 500은 임시 기본 체력 수치, 추후 _monsterLevel 값을 이용해서 BossMonsterStat 테이블에서 수치 갖고오기
            _monsterMaxHP = Mathf.FloorToInt(500 * _monsterStatData.HpRate);
            _monsterDefense = Mathf.FloorToInt(10 * _monsterStatData.DefRate);
        }

        //_monsterSkillSet = DataManager.Instance.GetMonsterSkillSetData(monsterData.SkillSet);
        
    }

    private void Start()
    {
        // ID 방식 Key 방식 구별
        _monsterStatData = DataManager.Instance.GetMonsterStatData(1);
        //_monsterStatData = DataManager.Instance.GetMonsterStatData("KeyMonsterGhost0001");
        if(_monsterStatData == null)
        {
            Debug.LogError("몬스터 스탯 데이터를 불러오지 못했습니다. Id: 1");
            return;
        }
        else
        {
            Debug.Log("데이터 로드 성공");
            InitMonsterStatus();
        }

        _monsterCurHP = _monsterMaxHP;
        NotifyHpObservers(); //초기 HP 상태 갱신
    }

    public void TakeDamage(int damage) // 데미지를 입을 때 호출할 함수
    {
        damage = Mathf.Max(damage -  Mathf.Min(_monsterDefense, 7), 0); //방어력 적용(방어력 최대가 7이라서 7까지만 적용)
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

    private string GetRandomSkillFromSet() // 몬스터의 스킬셋에서 가중치에 따라 랜덤으로 스킬을 선택하는 함수
    {
        float totalRate = 0f;
        foreach(var skillweight in _monsterSkillSet.skillWeights)
        {
            totalRate += skillweight.Item2;
        }

        float randomValue = Random.Range(0f, totalRate);
        float curSum = 0f;

        foreach(var skillSlot in _monsterSkillSet.skillWeights)
        {
            curSum += skillSlot.Item2;
            if(randomValue <= curSum)
            {
                return skillSlot.Item1;
            }
        }
        return null;        
    }

    private void UseSkill() //턴마다 스킬을 사용하는 함수
    {
        string skillKey = GetRandomSkillFromSet();
        if(skillKey != null)
        {
            //스킬 사용 로직 추가
        }
    }



    //! 유니티 에디터를 사용할 때 체력 소모 이벤트에 대한 테스트를 위한 코드 추후 삭제 필요
    #if UNITY_EDITOR
    public void TestTakeDamage()
    {
        TakeDamage(10);
    }
    #endif

}
