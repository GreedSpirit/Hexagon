using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GlobalSaveData
{
    // [플레이어 정보]
    public int Level;
    public int CurrentExp;
    public int Money;
    public Vector3 PlayerPosition; // 플레이어 위치 (마을 내 좌표)

    // [진행도 정보]
    public int DungeonClearedIndex; // 최초 던전 클리어 여부 (스크립트 출력용)
    public string LastVillageName;     // 마지막으로 있었던 마을 이름 (위치 복구용)
    public int ScenarioPlayIndex;
    // [인벤토리 & 덱 정보]
    public List<UserCard> MyCards;     // 보유 카드 리스트 (UserCard는 이미 Serializable임)
    public List<int> MyDeck;           // 현재 장착중인 덱 (카드 ID 리스트)
}