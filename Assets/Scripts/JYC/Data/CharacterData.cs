using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    pc, npc
}


[System.Serializable]
public class CharacterData : CSVLoad, TableKey // 인터페이스 상속
{
    // 엑셀 컬럼명과 일치시킨 변수들
    public int Id { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    public CharacterType Type { get; set; }
    public float MoveSpeed { get; set; }
    public string Img { get; set; }
    public string Model { get; set; }

    // 인터페이스 구현: 들어온 데이터를 순서대로 변수에 넣는다.
    public void LoadFromCsv(string[] values)
    {
        // 순서 주의! 엑셀 파일의 열 순서대로 인덱스(0, 1, 2...)를 사용합니다.

        // 0번: Id (int)
        if (int.TryParse(values[0], out int idValue))
        {
            Id = idValue;
        }
        else
        {
            Id = 0; // 파싱 실패 시 기본값
        }

        // 1번: CharacterKey (string)
        Key = values[1];

        // 2번: Name (string)
        Name = values[2];

        // 3번: Desc (string)
        Desc = values[3];

        // 4번: Type(enum)
        if (Enum.TryParse(values[4], out CharacterType characterType))
        {
            Type = characterType;
        }
        else
        {
            Type = CharacterType.npc; //파싱 실패 시 기본값
        }

        // 5번: MoveSpeed (float)
        if (float.TryParse(values[5], out float moveSpeedValue))
        {
            MoveSpeed = moveSpeedValue;
        }
        else
        {
            MoveSpeed = 0.0f; //파싱 실패 시 기본값
        }

        // 6번: Img (string)
        Img = values[6];

        // 7번: Model(string)
        Model = values[7];
    }
}