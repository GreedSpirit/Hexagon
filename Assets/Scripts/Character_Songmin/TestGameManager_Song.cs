using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestGameManager_Song : MonoBehaviour
{
    private void Start()
    {
        if (Player.Instance != null && Player.Instance.IsInitialized)
        {
            return;
        }
        InitPlayer();
        if (GameSaveManager.Instance != null)
        {
            Debug.Log("[TestGameManager] 초기화 후 세이브 데이터 로드");
            GameSaveManager.Instance.LoadGame();
        }
    }

    public void InitPlayer()
    {
        Player player = Player.Instance;
        PlayerStatInitializer initializer = new PlayerStatInitializer();

        CharacterData characterData = DataManager.Instance.GetCharacter(1);
        string key = characterData.Name;
        string name =  DataManager.Instance.GetString(key)?.Korean.Trim('"');        
        List<CharacterLevelData> levelDatas = new List<CharacterLevelData>();
        List<CharacterStatData> statDatas = new List<CharacterStatData>();

        for (int i = 1; i <= 100; i++)
        {
            CharacterLevelData levelData = DataManager.Instance.GetCharacterLevel(i);
            if (levelData == null)
            {
                break;
            }
            levelDatas.Add(levelData);
        }

        for (int i = 1; i <= 100; i++)
        {
            CharacterStatData statData = DataManager.Instance.GetCharacterStat(i);
            if (statData == null)
            {
                break;
            }
            statDatas.Add(statData);
        }
        player.Init(initializer.InitPlayerStat(characterData, levelDatas, statDatas, name));
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(0);
    }

}
