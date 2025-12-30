using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSaveManager : Singleton<GameSaveManager>
{
    private string SavePath => Path.Combine(Application.persistentDataPath, "GlobalSaveData.json");

    // 게임 시작 시 호출
    public void LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("[GameSaveManager] 저장된 파일이 없습니다. 새로 시작합니다.");
            // 신규 유저 초기화: 기본 덱 지급
            if (CardManager.Instance != null)
            {
                CardManager.Instance.InitStartingDeck();
            }
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            GlobalSaveData data = JsonUtility.FromJson<GlobalSaveData>(json);

            if (data == null)
            {
                Debug.LogError("[GameSaveManager] 데이터 로드 실패 (Null)");
                return;
            }

            // 카드 데이터 복구
            if (CardManager.Instance != null)
            {
                CardManager.Instance.LoadFromSaveData(data.MyCards, data.MyDeck);
            }

            // 플레이어 데이터 복구
            if (Player.Instance != null)
            {
                Player.Instance.LoadFromSaveData(data);
            }

            Debug.Log($"[GameSaveManager] 로드 완료! (Lv.{data.Level}, {data.Money}G)");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameSaveManager] 로드 중 에러 발생: {e.Message}");
        }
    }

    // 저장 시점마다 호출 (보상, 강화, 마을 이동 등)
    public void SaveGame()
    {
        GlobalSaveData data = new GlobalSaveData();

        // 플레이어 정보 수집
        if (Player.Instance != null)
        {
            data.Level = Player.Instance.GetLevel();
            data.CurrentExp = Player.Instance.GetCurrentExp();
            data.Money = Player.Instance.GetMoney();
            data.PlayerPosition = Player.Instance.transform.position;
            data.DungeonClearedIndex = Player.Instance.DungeonClearedIndex;
            data.ScenarioPlayIndex = Player.Instance.ScenarioPlayIndex;

            if (Player.Instance.Currentvillage != null)
                data.LastVillageName = Player.Instance.Currentvillage.Name;
        }

        // 카드 정보 수집
        if (CardManager.Instance != null)
        {
            data.MyCards = new List<UserCard>(CardManager.Instance.UserCardList);
            data.MyDeck = new List<int>(CardManager.Instance.CurrentDeck);
        }

        // JSON 변환 및 쓰기
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log("[GameSaveManager] 저장 완료!");
    }

    [ContextMenu("Delete Save File")]
    public void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("세이브 파일 삭제됨");
        }
    }
}