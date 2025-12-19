using UnityEngine;

[System.Serializable]
public class RewardData : CSVLoad, TableKey
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string RewardItem { get; set; }
    public string RewardType { get; set; }
    public int RewardGroup { get; set; }
    public int MinAmount { get; set; }
    public int MaxAmount { get; set; }
    public float Probability { get; set; }
    public string Quest { get; set; }
    public int Measure { get; set; }

    // TableKey 인터페이스 구현
    string TableKey.Key
    {
        get { return Key; }
    }

    // CSV 데이터 파싱
    public void LoadFromCsv(string[] values)
    {
        // 인덱스: 0:Id, 1:Key, 2:Item, 3:Type, 4:Group, 5:Min, 6:Max, 7:Prob, 8:Quest, 9:Measure

        int.TryParse(values[0], out int id); Id = id;
        Key = values[1];
        RewardItem = values[2];
        RewardType = values[3];
        int.TryParse(values[4], out int group); RewardGroup = group;
        int.TryParse(values[5], out int min); MinAmount = min;
        int.TryParse(values[6], out int max); MaxAmount = max;

        if (values.Length > 7)
        {
            float.TryParse(values[7], out float prob);
            Probability = prob;
        }

        // 퀘스트
        if (values.Length > 8)
        {
            Quest = values[8];
        }

        if (values.Length > 9)
        {
            int.TryParse(values[9], out int mes);
            Measure = mes;
        }
        else
        {
            Measure = 0;
        }
    }
}