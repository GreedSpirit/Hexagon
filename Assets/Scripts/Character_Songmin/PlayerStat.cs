using System.Collections.Generic;
using UnityEngine;

public class PlayerStat
{
    //Initalizer에서 초기에 한 번만 받아오는 값
    public string Name { get;  set; }
    public float MoveSpeed { get; set; }

    //Initializer에서 미리 받아올 레벨 별 스탯 일람.
    public List<StatsByLevel> LevelList { get; set; }

    //레벨 업 할때마다 StatByLevels에서 꺼내어 갱신할 값
    public int Hp { get; private set; }
    public int Defense { get; private set; }
    public int NeedExp { get; private set; }
    public int TotalExp { get; private set; }

    //전투 중 갱신할 값.
    public int Level { get; private set; } = 1;
    public int Shield { get; private set; }
    public int CurrentExp { get; private set; }
    public int CurrentHp {  get; private set; }    
    



    
    public void SetStats()//맨 처음 캐릭터 생성할 때 및 레벨업 이후 호출.
    {
        StatsByLevel stats = LevelList[Level -1];
        Hp = stats.Hp;
        Defense = stats.Defense;
        NeedExp = stats.NeedExp;
        TotalExp = stats.TotalExp;        
    }

    private void LevelUp()
    {
        if (Level < LevelList.Count)
        {
            Level++;
            SetStats();
        }
        else
        {
            Debug.Log("이미 최대 레벨입니다.");
        }        
    }
    public void GetDamage(int damage) //데미지를 입을 때마다 호출.
    {
        int blockedDamage = damage - Defense; //방어력만큼 일단 감소
        if (Shield > 0) //보호막이 있는 경우
        {
            Shield -= blockedDamage; //보호막이 대미지 흡수
            if (Shield < 0) //데미지가 남았으면 체력도 감소
            {
                CurrentHp += Shield;
                Shield = 0;
            }            
        }
        else //보호막이 없는 경우
        {
            CurrentHp -= blockedDamage;
        }
    }


    public void GetTrueDamage(int damage) //상태이상 대미지를 받을 때마다 호출
    {
        CurrentHp -= damage;
    }


    public void GetHp(int hp) //체력을 회복할 때마다 호출
    {
        CurrentHp += hp;
        if (CurrentHp > Hp)
        {
            CurrentHp = Hp;
        }        
    }

    public void GetShield(int barrier) //보호막을 얻을 때마다 호출
    {
        Shield += barrier;
    }

    public void ResetShield() //보호막이 사라질 때(매 턴 시작 전 등)마다 호출
    {
        Shield = 0;
    }


    public void GetExp(int exp) //경험치를 얻을 때마다 호출
    {
        CurrentExp += exp;
        if (CurrentExp > NeedExp)
        {
            CurrentExp -= NeedExp;
            LevelUp();
        }
    }
}
