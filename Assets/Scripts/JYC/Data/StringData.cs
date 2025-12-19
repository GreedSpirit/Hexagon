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
        // Id
        if (values.Length > 0)
        {
            int.TryParse(values[0], out int idValue);
            Id = idValue;
        }
        else
        {
            Id = 0;
        }

        // Key (StringKey)
        if (values.Length > 1)
        {
            StringKey = values[1];
        }

        // Korean
        if (values.Length > 2)
        {
            Korean = values[2];
        }

        // English (데이터가 있을 때만 파싱하도록 안전장치 추가)
        if (values.Length > 3)
        {
            English = values[3];
        }
        else
        {
            English = ""; // 데이터가 없으면 빈 문자열로 처리
        }
    }
}