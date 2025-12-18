using NUnit.Framework.Internal;
using UnityEngine;


[System.Serializable]
public class MonsterStatusEffectInstance
{
    public int Id;
    public string Key;
    public string Name;
    public BuffType Type;
    public int Duration;
    public int Stack;
    public float AppliedTime; // 적용 시점

    public int GetSortOrder(MonsterStatusEffectInstance other)
    {
        //1순위 정렬 : 지속 턴/ 스택 합산 내림차순
        int thisValue = Mathf.Max(this.Duration, this.Stack);
        int otherValue = Mathf.Max(other.Duration, other.Stack);
        if(thisValue != otherValue) return otherValue.CompareTo(thisValue);

        //2순위 정렬 : 타입 (Buff > Debuff > Dot)
        if(this.Type != other.Type) return this.Type.CompareTo(other.Type);

        //3순위 정렬 : 적용 시점 빠른 순서
        if (this.AppliedTime != other.AppliedTime) return this.AppliedTime.CompareTo(other.AppliedTime);

        return this.Id.CompareTo(other.Id); 
    }
}
