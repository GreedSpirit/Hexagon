using System.Collections.Generic;
using UnityEngine;

public enum MonsterGrade
{
    Common,
    Boss
}

public class MonsterStatus : MonoBehaviour
{
    [SerializeField] private string _monsterName;
    [SerializeField] private string _monsterLevel;
    [SerializeField] private MonsterGrade _monsterGrade;
    [SerializeField] private int _monsterMaxHP;
    [SerializeField] private int _monsterCurHP;
    [SerializeField] private int _monsterDefense;
    [SerializeField] private int _monsterShield = 0;

    private List<IMonsterHpObserver> _hpObservers = new List<IMonsterHpObserver>(); //옵저버 목록을 관리할 List

    public MonsterStatus(string name, string level, MonsterGrade grade, int maxHp, int defense)
    {
        //! 추후 데이터 테이블에서 읽어와서 초기화 할 때 할당
        _monsterName = name;
        _monsterLevel = level;
        _monsterGrade = grade;
        _monsterMaxHP = maxHp;
        _monsterCurHP = maxHp;
        _monsterDefense = defense;
    }

    private void Start()
    {
        _monsterCurHP = _monsterMaxHP;
        NotifyHpObservers(); //초기 HP 상태를 알림
    }

    public void TakeDamage(int damage)
    {
        //!추후 자세한 damage 계산 로직 추가(방어력)
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


    //! 유니티 에디터를 사용할 때 체력 소모 이벤트에 대한 테스트를 위한 코드 추후 삭제 필요
    #if UNITY_EDITOR
    public void TestTakeDamage()
    {
        TakeDamage(10);
    }
    #endif

}
