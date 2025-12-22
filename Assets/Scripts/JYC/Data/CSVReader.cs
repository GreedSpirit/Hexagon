using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVReader
{
    public static List<T> Read<T>(string file) where T : CSVLoad, new()
    {
        List<T> list = new List<T>();
        TextAsset data = Resources.Load<TextAsset>(file);

        if (data == null)
        {
            Debug.LogError($"CSV 파일을 찾을 수 없습니다: {file}");
            return list;
        }

        // 엔터키 처리 (\r\n 또는 \n)
        string[] lines = Regex.Split(data.text.Replace("\r\n", "\n").Replace("\r", "\n"), "\n");

        // i = 0 부터 시작
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            // 빈 줄이나 주석(#) 처리
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

            string[] values = line.Split(',');

            // 데이터가 비어있으면 건너뛰기 (최소한 ID나 Key는 있어야 함)
            if (values.Length < 1 || string.IsNullOrWhiteSpace(values[0])) continue;
            if (!int.TryParse(values[0], out int _))
            {
                // 숫자가 아니면 헤더일 가능성이 높으므로 건너뜀
                // 단, 진짜 String Key를 쓰는 테이블(예: StringData)은 예외 처리가 필요할 수 있음.

                string firstCol = values[0].Trim();
                if (IsHeader(firstCol)) continue;
            }

            try
            {
                T entry = new T();
                entry.LoadFromCsv(values);

                // ID가 0이면(파싱 실패 등으로 인해) 리스트에 넣지 않음
                // 만약 데이터 클래스에서 LoadFromCsv 내부에서 파싱 실패 시 예외를 던지거나 별도 처리를 했다면 여기서 걸러집니다.

                list.Add(entry);
            }
            catch (Exception e)
            {
                // 파싱 중 에러가 나도 게임이 멈추지 않고 로그만 남기고 다음 줄로 넘어감
                Debug.LogWarning($"CSV 파싱 건너뜀 ({file} - {i + 1}번 줄): {e.Message}");
            }
        }

        return list;
    }
    // 헤더인지 판별하는 함수
    private static bool IsHeader(string val)
    {
        // 대소문자 무시하고 비교
        string v = val.ToLower();
        return v == "id" || v == "key" || v == "int" || v == "string" || v == "float" ||
               v == "enum" || v == "bool" || v == "level" || v.StartsWith("[") || v.StartsWith("no");
    }
}