using UnityEngine;

[System.Serializable]
public class DeckData : CSVLoad, TableKey
{
    public int Id { get; set; }
    public string Key { get; set; } // DeckKey

    public int NormalCount;
    public int RareCount;
    public int EpicCount;

    public void LoadFromCsv(string[] values)
    {
        // 0번은 ID, 1번은 Key (CSV 순서에 맞춤)
        Id = int.Parse(values[0]);
        Key = values[1];

        // 숫자가 비어있거나 에러날 경우를 대비해 TryParse를 쓰거나 기본 0 처리
        int.TryParse(values[2], out NormalCount);
        int.TryParse(values[3], out RareCount);
        int.TryParse(values[4], out EpicCount);
    }
}