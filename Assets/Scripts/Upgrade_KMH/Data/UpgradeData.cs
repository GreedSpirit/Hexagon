using System;
using UnityEngine;

public class UpgradeData : CSVLoad, TableKey
{
    public int Id { get; set; }                     // id
    public string Key => Id.ToString();             // Key
    public CardGrade CardGrade { get; set; }        // 카드 등급
    public int Level { get; set; }                  // 현재 강화 단계
    public int NextLevel { get; set; }              // 강화 후 단계
    public int ReqCardAmount { get; set; }          // 필요 카드량
    public string ReqCurrency { get; set; }         // 소모 재화
    public int ReqCurrencyAmount { get; set; }      // 필요 재화량


    public void LoadFromCsv(string[] values)
    {
        if (int.TryParse(values[0], out int idValue))
            Id = idValue;
        else
            Id = 0;

        if (Enum.TryParse(values[1], out CardGrade grade))
            CardGrade = grade;
        else
        {
            Debug.LogError($"{Key} 의 CardGrade가 비어있습니다.");
            CardGrade = CardGrade.Null;
        }

        if (int.TryParse(values[2], out int level))
            Level = level;
        else
            Level = 0;

        if (int.TryParse(values[3], out int nextLevel))
            NextLevel = nextLevel;
        else
            NextLevel = 0;

        if (int.TryParse(values[4], out int reqCardAmount))
            ReqCardAmount = reqCardAmount;
        else
            ReqCardAmount = 0;

        ReqCurrency = values[5];

        if (int.TryParse(values[6], out int reqCurrencyAmount))
            ReqCurrencyAmount = reqCurrencyAmount;
        else
            ReqCurrencyAmount = 0;
    }
}
