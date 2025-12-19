using UnityEngine;

[System.Serializable]
public class DungeonData : CSVLoad, TableKey
{
    // 엑셀 컬럼명과 일치시킨 프로퍼티
    public int Id { get; set; }
    public string DungeonKey { get; set; }      // 엑셀 컬럼명: DungeonKey
    public string Name { get; set; }
    public string Desc { get; set; }
    public int RequiredLevel { get; set; }
    public int NumberOfStages { get; set; }

    public float Slot1Probability { get; set; }
    public float Slot2Probability { get; set; }
    public float Slot3Probability { get; set; }
    public float Slot4Probability { get; set; }
    public int RewardGroup { get; set; }

    // 엑셀 컬럼명에 맞춰서 Key 설정
    string TableKey.Key
    {
        get { return DungeonKey; }
    }


    // CSV 데이터 파싱

    public void LoadFromCsv(string[] values)
    {
        // 0: Id
        if (values.Length > 0)
        {
            int.TryParse(values[0], out int id);
            Id = id;
        }

        // 1: DungeonKey
        if (values.Length > 1) DungeonKey = values[1];

        // 2: Name
        if (values.Length > 2) Name = values[2];

        // 3: Desc
        if (values.Length > 3) Desc = values[3];

        // 4: RequiredLevel
        if (values.Length > 4)
        {
            int.TryParse(values[4], out int lv);
            RequiredLevel = lv;
        }

        // 5: NumberOfStages
        if (values.Length > 5)
        {
            int.TryParse(values[5], out int num);
            NumberOfStages = num;
        }
        // 확률 데이터 (데이터가 있을 때만 읽음)

        if (values.Length > 6) Slot1Probability = ParseFloatSafe(values[6]);
        if (values.Length > 7) Slot2Probability = ParseFloatSafe(values[7]);
        if (values.Length > 8) Slot3Probability = ParseFloatSafe(values[8]);
        if (values.Length > 9) Slot4Probability = ParseFloatSafe(values[9]);

        if (values.Length > 10)
        {
            int.TryParse(values[10], out int rg);
            RewardGroup = rg;
        }
        else
        {
            RewardGroup = 0;
        }
    }
    // 빈 문자열 안전 처리용 헬퍼
    private float ParseFloatSafe(string val)
    {
        if (string.IsNullOrEmpty(val)) return 0f;
        if (float.TryParse(val, out float result)) return result;
        return 0f;
    }
}