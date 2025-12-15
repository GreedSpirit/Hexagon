using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData : CSVLoad // 인터페이스 상속
{
    // 엑셀 컬럼명과 일치시킨 변수들
    public int Id;
    public string CharacterKey;
    public string Name;
    public int MoveSpeed;
    public string Img;
    public string Model;

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
        CharacterKey = values[1];

        // 2번: Name (string)
        Name = values[2];

        // 3번: MoveSpeed (int)
        if (int.TryParse(values[3], out int speedValue))
        {
            MoveSpeed = speedValue;
        }
        else
        {
            MoveSpeed = 0;
        }

        // 4번: Img (string)
        Img = values[4];

        // 5번: Model (string)
        Model = values[5];
    }
}