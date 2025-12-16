using System;
using UnityEngine;

public class CardNumberOfAvailableData : CSVLoad, TableKey
{
    public CardGrade CardGrade { get; set; }    // 카드 등급
    public int Level {  get; set; }             // 카드 강화 단계
    public int NumberOfAvailable {  get; set; } // 카드 사용 가능 횟수

    int TableKey.Id => Level;
    string TableKey.Key => Level.ToString();

    public void LoadFromCsv(string[] values)
    {
        if (Enum.TryParse(values[0], out CardGrade grade))
            CardGrade = grade;
        else
            CardGrade = CardGrade.Null;

        if (int.TryParse(values[1], out int level))
            Level = level;
        else
            Level = 0;

        if (int.TryParse(values[2], out int noa))
            NumberOfAvailable = noa;
        else
            NumberOfAvailable = 0;
    }
}
