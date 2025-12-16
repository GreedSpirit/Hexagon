using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringData : CSVLoad, TableKey
{
 
    //  엑셀 컬럼명과 일치시킨 프로퍼티
    public int Id { get; set; }
    public string StringKey { get; set; }   // 엑셀 컬럼명: StringKey
    public string Korean { get; set; }
    public string English { get; set; }

    // DataManager가 'Key'로 접근하면 'StringKey'를 반환
    string TableKey.Key
    {
        get { return StringKey; }
    }


    // CSV 데이터 파싱
    public void LoadFromCsv(string[] values)
    {
        // 0: Id
        if (int.TryParse(values[0], out int idValue))
        {
            Id = idValue;
        }
        else
        {
            Id = 0;
        }

        // 1: StringKey
        StringKey = values[1];

        // 2: Korean
        Korean = values[2];

        // 3: English
        English = values[3];
    }
}