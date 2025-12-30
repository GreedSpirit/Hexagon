using System;
using UnityEngine;

[System.Serializable]
public class UserCard : ISerializationCallbackReceiver 
{
    public int InstanceId;      // 고유 식별자 (필요시)
    public int CardId;          // 데이터 테이블 ID 참조
    public int Level;           // 카드 레벨
    public int Count;           // 보유 개수 (최대 99)
    public DateTime AcquiredTime;

    [HideInInspector] public long AcquiredTimeTicks;

    public CardData GetData() => DataManager.Instance.GetCard(CardId);

    public void OnBeforeSerialize()
    {
        AcquiredTimeTicks = AcquiredTime.Ticks;
    }

    public void OnAfterDeserialize()
    {
        try
        {
            AcquiredTime = new DateTime(AcquiredTimeTicks);
        }
        catch
        {
            AcquiredTime = DateTime.Now; // 에러 나면 현재 시간으로
        }
    }
}