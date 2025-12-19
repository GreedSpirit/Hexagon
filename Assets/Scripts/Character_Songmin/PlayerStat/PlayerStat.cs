using System.Collections.Generic;
using UnityEngine;

public class PlayerStat
{
    //Initalizer에서 초기에 한 번만 받아오는 값
    public string Name { get; set; }
    public float MoveSpeed { get; set; }

    //Initializer에서 미리 받아올 레벨 별 스탯 일람.
    public List<StatsByLevel> LevelList { get; set; }

    //레벨 업 할때마다 StatByLevels에서 꺼내어 갱신할 값
    public int Hp { get; private set; } //최대 체력
    public int Defense { get; private set; } //방어력
    public int NeedExp { get; private set; } //레벨업까지 필요한 경험치

    //전투 중 갱신할 값.
    public int Level { get; private set; } = 1;
    public int Shield { get; private set; } //보호막
    public int CurrentExp { get; private set; } //현재 보유 중인 경험치
    public int CurrentHp { get; private set; } //현재 체력

    public float Buff { get; private set; } //강화(공격 데미지 상승률)
    public float DeBuff { get; private set; } //약화(피격 데미지 상승률)

    public int Poison { get; private set; } //독 중첩 스택
    public int Burn { get; private set; } //화상 중첩 스택

    public Dictionary<StatusEffectData, int> StatusEffects { get; private set; } //현재 걸려 있는 상태이상 목록



    public void SetStats()//맨 처음 캐릭터 생성할 때 및 레벨업 이후 호출.
    {
        StatusEffects = new Dictionary<StatusEffectData, int>();
        StatsByLevel stats = LevelList[Level - 1];
        Hp = stats.Hp;
        Defense = stats.Defense;
        NeedExp = stats.NeedExp;
    }

    private void LevelUp()
    {
        if (Level >= LevelList.Count)
        {
            Debug.Log("이미 최대 레벨입니다.");
            return;
        }
        Level++;
        SetStats();
        GetHp(Hp);
    }
    public void GetDamage(int damage) //데미지를 입을 때마다 호출.
    {
        int reinforcedDamage = Mathf.FloorToInt(damage * (1 + DeBuff));
        int blockedDamage = reinforcedDamage - Defense; //방어력만큼 일단 감소
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
        Die();
    }
    

    public void GetTrueDamage(int damage) //상태이상 대미지를 받을 때마다 호출
    {
        CurrentHp -= damage;
        Die();
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

    public void ResetStatusEffect() //전투상황 종료 시 호출
    {
        Poison = 0;
        Burn = 0;
        Buff = 0;
        DeBuff = 0;        
        StatusEffects.Clear();
    }

    public void GetExp(int exp) //경험치를 얻을 때마다 호출
    {
        if (Level >= LevelList.Count)
        {
            CurrentExp = NeedExp;
            return;
        }
        CurrentExp += exp;
        if (CurrentExp >= NeedExp && Level < LevelList.Count)
        {
            CurrentExp -= NeedExp;
            LevelUp();
        }
        if (Level >= LevelList.Count)
        {
            CurrentExp = NeedExp;
        }
    }

    private void Die()
    {
        if (CurrentHp <= 0)
        {
            CurrentHp = 0;
            //죽는 매서드 호출
            Debug.Log("플레이어 사망!");
        }
    }
    public void AddStatusEffect(string effectKey, int duration, int stack)
    {
        StatusEffectData data = DataManager.Instance.GetStatusEffectData(effectKey);
                
        if (StatusEffects.ContainsKey(data))
        {
            StatusEffects[data] += duration + stack;

            if (data.Key == "KeyStatusBurn")
            {
                Burn += duration + stack;
                if (StatusEffects[data] > data.MaxChar)
                {
                    StatusEffects[data] = data.MaxChar;
                    Burn = data.MaxChar;
                }
            }
            if (data.Key == "KeyStatusPoison")
            {
                Poison += duration + stack;
                if (StatusEffects[data] > data.MaxChar)
                {   
                    StatusEffects[data] = data.MaxChar;
                    Poison = data.MaxChar;
                }
            }
            if (StatusEffects[data] > data.MaxChar)
            {
                StatusEffects[data] = data.MaxChar;             
            }


        }
        else
        {
            StatusEffects.Add(data, stack + duration);
            if (data.BuffType == BuffType.Buff)
            {
                Buff += data.ValueFormula;
            }
            else if (data.BuffType == BuffType.DeBuff)
            {
                DeBuff += data.ValueFormula;
            }

            if (StatusEffects[data] > data.MaxChar)
            {
                StatusEffects[data] = data.MaxChar;
            }
        }
    }

    public void ApplyStatusEffect()
    {            
        List<StatusEffectData> removeList = new List<StatusEffectData>();

        foreach (var pair in StatusEffects)
        {
            StatusEffectData data = pair.Key;
            int stackOrDuration = pair.Value;
            

            // 지속 효과 적용
            if (data.BuffType == BuffType.DoT)
            {
                GetTrueDamage(stackOrDuration);                
            }

            // 스택 또는 턴 감소
            stackOrDuration--;

            if (stackOrDuration <= 0)
            {
                removeList.Add(data);
                if (data.Key == "KeyStatusBurn") 
                    Burn = 0;
                if (data.Key == "KeyStatusPoison") 
                    Poison = 0;
            }
            else
            {
                StatusEffects[data] = stackOrDuration;
                if (data.Key == "KeyStatusBurn") 
                    Burn = stackOrDuration;
                if (data.Key == "KeyStatusPoison")
                    Poison = stackOrDuration;
            }
        }

        // 만료된 상태이상 제거
        foreach (var data in removeList)
        {
            StatusEffects.Remove(data);
            if (data.BuffType == BuffType.Buff)
            {
                Buff -= data.ValueFormula;
            }
            else if (data.BuffType == BuffType.DeBuff)
            {
                DeBuff -= data.ValueFormula;
            }
        }
    }
}
