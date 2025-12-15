using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public Dictionary<int, CharacterData> CharacterDict { get; private set; }

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
        CharacterDict = ListToDict(CSVReader.Read<CharacterData>("Character"));

        // 테스트 로그
        Debug.Log($"데이터 로드 완료. Character 개수: {CharacterDict.Count}");
    }

    // 리스트를 딕셔너리로 변환
    private Dictionary<int, CharacterData> ListToDict(List<CharacterData> list)
    {
        Dictionary<int, CharacterData> dict = new Dictionary<int, CharacterData>();

        foreach (CharacterData item in list)
        {
            if (!dict.ContainsKey(item.Id))
            {
                dict.Add(item.Id, item);
            }
        }
        return dict;
    }

    public CharacterData GetCharacter(int id)
    {
        if (CharacterDict.TryGetValue(id, out CharacterData data))
        {
            return data;
        }
        return null;
    }
}