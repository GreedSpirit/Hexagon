using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 유저가 실제로 보유한 카드 정보
[System.Serializable]
public class UserCard
{
    public int InstanceId;      // 고유 식별자 (필요시)
    public int CardId;          // 데이터 테이블 ID 참조
    public int Level;           // 카드 레벨
    public int Count;           // 보유 개수 (최대 99)
    public DateTime AcquiredTime; // 획득 시간 (정렬용)

    public CardData GetData() => DataManager.Instance.GetCard(CardId);
}
