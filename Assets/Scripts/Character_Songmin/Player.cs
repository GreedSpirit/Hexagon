using System;
using UnityEngine;

/// <summary>
/// 외부와 상호작용할 '플레이어 캐릭터'는 이거 하나. 
/// </summary>
public class Player : MonoBehaviour //나중에 싱글톤도 해주기
{   
    PlayerStat _stat;

    

    public Action OnHpChanged; //체력 수치 변화할 때마다 호출.
    public Action OnShieldChanged; //보호막 수치 변화할 때마다 호출.

    private void Start()
    {        
        //GameManager.'PlayerInitializer를 생성해서 PlayerInitializer.InitPlayerStat을 호출하는 함수'를 호출
    }

    
    /// 추후 GameManager 구현 시 Start에 다음 함수 추가할 것
    // FindFirstObjectByType<Player>().Init(new PlayerStatInitializer().InitPlayerStat());
    /// 이거 좀 다듬어서... 안전 예외 처리도 좀 하고.. 가독성도 키우고...




    ///게임 매니저에서 호출할 함수
    public void Init(PlayerStat stat)
    {
        _stat = stat;
    }


    
    /// 이하 함수들은 전투 중 외부에서 호출.      
    public void GetDamage(int damage) //데미지를 입을 때마다 호출.
    {
        _stat.GetDamage(damage);
        OnShieldChanged();
        OnHpChanged?.Invoke();
    }


    public void GetTrueDamage(int damage) //상태이상 대미지를 받을 때마다 호출
    {
        _stat.GetTrueDamage(damage);
    }


    public void GetHp(int hp) //체력을 회복할 때마다 호출
    {
        _stat.GetHp(hp);
        OnHpChanged?.Invoke();
    }

    public void GetShield(int shield) //보호막을 얻을 때마다 호출
    {
        _stat.GetShield(shield);
        OnShieldChanged?.Invoke();
    }

    public void ResetShield() //보호막이 사라질 때(매 턴 시작 전 등)마다 호출
    {
        _stat.ResetShield();
        OnShieldChanged?.Invoke();
    }


    public void GetExp(int exp)
    {
        _stat.GetExp(exp);
    }



}
