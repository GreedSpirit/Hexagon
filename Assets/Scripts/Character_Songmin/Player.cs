using System;
using UnityEngine;

public class Player : MonoBehaviour //나중에 싱글톤도 해주기
{
    [SerializeField] PlayerStat _stat;
    public Action OnHpChanged; //체력 수치 변화할 때마다 호출.
    public Action OnShieldChanged; //보호막 수치 변화할 때마다 호출.





    public void GetDamage(int damage) //데미지를 입을 때마다 호출.
    {        


        if (_stat.Shield > 0) //보호막이 있는 경우
        {
            _stat.Shield -= damage; //보호막이 대미지 흡수
            if (_stat.Shield < 0) //데미지가 남았으면 체력도 감소
            {
                _stat.CurrentHp += _stat.Shield;
                _stat.Shield = 0;
            }
            OnShieldChanged();
        }
        else //보호막이 없는 경우
        {
            _stat.CurrentHp -= damage;
        }
        OnHpChanged();
    }


    public void GetTrueDamage(int damage) //상태이상 대미지를 받을 때마다 호출
    {
        _stat.CurrentHp -= damage;
        OnHpChanged();
    }


    public void GetHp(int hp) //체력을 회복할 때마다 호출
    {
        _stat.CurrentHp += hp;
        if (_stat.CurrentHp > _stat.Hp)
        {
            _stat.CurrentHp = _stat.Hp;
        }
        OnHpChanged();
    }

    public void GetShield(int barrier) //보호막을 얻을 때마다 호출
    {
        _stat.Shield += barrier;
    }

    public void ResetShield() //보호막이 사라질 때(매 턴 시작 전 등)마다 호출
    {
        _stat.Shield = 0;
    }


    public void GetExp(int exp) //경험치를 얻을 때마다 호출
    {
        _stat.CurrentExp += exp;
        if (_stat.CurrentExp > _stat.NeedExp)
        {
            _stat.CurrentExp -= _stat.NeedExp;
            _stat.LevelUp();
        }
    }









    public void SetState()
    {

    }
}
