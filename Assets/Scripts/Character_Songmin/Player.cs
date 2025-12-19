using System;
using UnityEngine;

/// <summary>
/// 외부와 상호작용할 '플레이어 캐릭터'는 이거 하나. 
/// </summary>
public class Player : MonoBehaviour, IBattleUnit //나중에 싱글톤도 해주기
{   
    PlayerStat _stat;
    public static Player Instance;

    
    public Action<int, int, int, int> OnHpChanged; //체력 수치 변화할 때마다 호출.    
    public Action<int> OnShieldChanged; //보호막 수치 변화할 때마다 호출.
    public Action<int, int> OnExpChanged; //경험치 획득할 때마다 호출.
    public Action<int> OnLevelChanged; //레벨업할 때마다 호출.
    public Action<int> OnDefenseChanged; //방어력 변화할 때마다 호출.
    public Action OnStatusEffectChanged; //상태이상 변화할 때마다 호출

    private void Awake() //임시 유사 싱글톤 처리
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start() //파싱 되었는지 테스트용 코드
    {
        Debug.Log($"{_stat.Name}");
        Debug.Log($"남은 경험치 : {_stat.NeedExp}");
        Debug.Log($"체력 : {_stat.Hp}");
        Debug.Log($"현재 체력 : {_stat.CurrentHp}");
        Debug.Log($"방어력 : {_stat.Defense}");
        Debug.Log($"이동속도 : {_stat.MoveSpeed}");
        
    }

    

    ///게임 매니저에서 호출할 함수
    public void Init(PlayerStat stat)
    {
        _stat = stat;
        GetHp(_stat.Hp);
        PushHp();
        PushExp();
        PushLevel();
        PushDefense();
        PushShield();      
    }

    
    /// UI 활성화 시 호출할 함수
    public void PushHp()
    {
        if (_stat != null)
        {
            OnHpChanged?.Invoke(_stat.CurrentHp, _stat.Hp, _stat.Poison, _stat.Burn);            
        }        
    }
    public void PushExp()
    {
        if (_stat != null)
        {
            OnExpChanged?.Invoke(_stat.CurrentExp, _stat.NeedExp);
        }        
    }
    public void PushDefense()
    {
        if (_stat != null)
        {
            OnDefenseChanged?.Invoke(_stat.Defense);
        }            
    }
    public void PushShield()
    {
        if (_stat != null)
        {
            OnShieldChanged?.Invoke(_stat.Shield);
        }            
    }

    public int PushTotalConditionDamage()
    {
        int total = _stat.Poison + _stat.Burn;
        return total;
    }

    public void PushLevel()
    {
        if (_stat != null)
        {
            OnLevelChanged?.Invoke(_stat.Level);
        }        
    }

    public int GetCurrentHp()
    {
        return _stat.CurrentHp;
    }
    public int GetFullHp()
    {
        return _stat.Hp;
    }


    /// 이하 함수들은 전투 중 외부에서 호출.      
    public void TakeDamage(int damage) //공격 데미지를 입을 때마다 호출.
    {
        _stat.GetDamage(damage);
        OnShieldChanged?.Invoke(_stat.Shield);
        OnHpChanged?.Invoke(_stat.CurrentHp, _stat.Hp, _stat.Poison, _stat.Burn);
    }

      
        

    public void GetHp(int hp) //체력을 회복할 때마다 호출
    {
        _stat.GetHp(hp);
        OnHpChanged?.Invoke(_stat.CurrentHp, _stat.Hp, _stat.Poison, _stat.Burn);
    }

    public void GetShield(int shield) //보호막을 얻을 때마다 호출
    {
        _stat.GetShield(shield);
        OnShieldChanged?.Invoke(_stat.Shield);
    }

    public void ResetShield() //보호막이 사라질 때(매 턴 시작 전 등)마다 호출
    {
        _stat.ResetShield();
        OnShieldChanged?.Invoke(_stat.Shield);
    }

    public void ResetCondition()
    {
        _stat.ResetStatusEffect();
        OnHpChanged?.Invoke(_stat.CurrentHp, _stat.Hp, _stat.Poison, _stat.Burn);
    }


    public void GetExp(int exp)
    {
        _stat.GetExp(exp);
        OnHpChanged?.Invoke(_stat.CurrentHp, _stat.Hp, _stat.Poison, _stat.Burn);
        OnExpChanged?.Invoke(_stat.CurrentExp, _stat.NeedExp);
        OnLevelChanged?.Invoke(_stat.Level);
    }

    public void AddStatusEffect(string effectKey, int duration, int stack)
    {        
        _stat.AddStatusEffect(effectKey, duration, stack);
    }

    public void ApplyStatusEffect()
    {
        Debug.Log("플레이어 상태이상 대미지 적용");
        _stat.ApplyStatusEffect();
        Debug.Log($"현재 독 : {_stat.Poison}, 화상 : {_stat.Burn}");
        OnHpChanged?.Invoke(_stat.CurrentHp, _stat.Hp, _stat.Poison, _stat.Burn);
    }  
}
