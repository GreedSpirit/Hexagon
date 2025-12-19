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
        int.TryParse(values[0], out int id); Id = id;
        DungeonKey = values[1];
        Name = values[2];
        Desc = values[3];
        int.TryParse(values[4], out int lv); RequiredLevel = lv;
        int.TryParse(values[5], out int num); NumberOfStages = num;

        // 확률 필드 (빈 값일 수 있으니 안전하게 파싱)
        Slot1Probability = ParseFloatSafe(values[6]);
        Slot2Probability = ParseFloatSafe(values[7]);
        Slot3Probability = ParseFloatSafe(values[8]);
        if (values.Length > 9) Slot4Probability = ParseFloatSafe(values[9]); // 4번 슬롯은 없을 수도 있음

        if (values.Length > 10)
        {
            int.TryParse(values[10], out int rg);
            RewardGroup = rg;
        }
        else
        {
            RewardGroup = 0; // 데이터가 없으면 0으로 초기화
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