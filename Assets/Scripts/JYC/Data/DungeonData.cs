using UnityEngine;

[System.Serializable]
public class DungeonData : CSVLoad, TableKey
{
    // ���� �÷����� ��ġ��Ų ������Ƽ
    public int Id { get; set; }
    public string DungeonKey { get; set; }      // ���� �÷���: DungeonKey
    public string Name { get; set; }
    public string Desc { get; set; }
    public int RequiredLevel { get; set; }
    public int NumberOfStages { get; set; }

    public float Slot1Probability { get; set; }
    public float Slot2Probability { get; set; }
    public float Slot3Probability { get; set; }
    public float Slot4Probability { get; set; }
    public int RewardGroup { get; set; }
    public string Deck { get; set; }
    public int Exp { get; set; }

    // ���� �÷����� ���缭 Key ����
    string TableKey.Key
    {
        get { return DungeonKey; }
    }


    // CSV ������ �Ľ�

    public void LoadFromCsv(string[] values)
    {
        // �迭 ���� üũ�� ��� �׸� ����

        if (values.Length > 0)
        {
            int.TryParse(values[0], out int id);
            Id = id;
        }
        if (values.Length > 1) DungeonKey = values[1];
        if (values.Length > 2) Name = values[2];
        if (values.Length > 3) Desc = values[3];

        if (values.Length > 4) { int.TryParse(values[4], out int lv); RequiredLevel = lv; }
        if (values.Length > 5) { int.TryParse(values[5], out int num); NumberOfStages = num; }

        // �����Ͱ� ������ ���� �е��� ���ǹ� �߰�
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

        if (values.Length > 11)
        {
            Deck = values[11];
        }
        if (values.Length > 12)
        {
            int.TryParse(values[12], out int e);
            Exp = e;
        }
    }

    private float ParseFloatSafe(string val)
    {
        if (string.IsNullOrEmpty(val)) return 0f;
        if (float.TryParse(val, out float result)) return result;
        return 0f;
    }
}