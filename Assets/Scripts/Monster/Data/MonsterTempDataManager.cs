using System.Collections.Generic;
using UnityEngine;

public class MonsterTempDataManager : MonoBehaviour
{
    public static MonsterTempDataManager Instance { get; private set; }

    public Dictionary<int, MonsterStatData> MonsterStatDict { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAllData()
    {
        MonsterStatDict = ListToDict(CSVReader.Read<MonsterStatData>("Monster"));
        //MonsterSkillSetDict = ListToDict(CSVReader.Read<MonsterSkillSetData>("MonsterSkillSet"));
    }

    private Dictionary<int, T> ListToDict<T>(List<T> list) where T : CSVLoad
    {
        Dictionary<int, T> dict = new Dictionary<int, T>();

        foreach (T item in list)
        {
            int id = (int)item.GetType().GetProperty("Id").GetValue(item); //리플렉션을 사용하여 Id 프로퍼티 값 가져오기
            if (!dict.ContainsKey(id))
            {
                dict.Add(id, item);
            }
        }
        return dict;
    }

    public MonsterStatData GetMonsterStat(int id)
    {
        if (MonsterStatDict.TryGetValue(id, out MonsterStatData data))
        {
            return data;
        }
        return null;
    }


}
