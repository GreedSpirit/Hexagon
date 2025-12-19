using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringData : CSVLoad, TableKey
{
 
    //  ���� �÷����� ��ġ��Ų ������Ƽ
    public int Id { get; set; }
    public string StringKey { get; set; }   // ���� �÷���: StringKey
    public string Korean { get; set; }
    public string English { get; set; }

    // DataManager�� 'Key'�� �����ϸ� 'StringKey'�� ��ȯ
    string TableKey.Key
    {
        get { return StringKey; }
    }


    // CSV ������ �Ľ�
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
        //English = values[3];
    }
}