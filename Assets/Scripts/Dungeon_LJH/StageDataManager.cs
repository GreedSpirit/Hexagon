using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageDataManager : MonoBehaviour
{
    public static StageDataManager Instance;
    public List<StageData> AllStageDataList = new List<StageData>();
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        AllStageDataList = CSVReader.Read<StageData>("Stage");
    }

    public List<StageData> GetStageByDungeonKey(string dungeonKey)
    {
        return AllStageDataList.Where(x => x.Dungeon == dungeonKey).ToList();
    }
}
