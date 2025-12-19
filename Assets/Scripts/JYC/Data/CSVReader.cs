using UnityEngine;
using System.Collections.Generic;
using System;

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
        string[] lines = data.text.Replace("\r\n", "\n").Split('\n');

        // i = 0 부터 시작
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            // 빈 줄이나 주석(#) 처리
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

            string[] values = line.Split(',');

            // 데이터가 비어있으면 건너뛰기
            if (values.Length == 0 || string.IsNullOrWhiteSpace(values[0])) continue;

            // 첫 번째 칸(ID)이 '숫자'가 아니면 헤더나 잘못된 줄로 간주하고 무시!
            if (!int.TryParse(values[0], out int _)) continue;

            try
            {
                T entry = new T();
                entry.LoadFromCsv(values); // 각 데이터 클래스의 파싱 로직 실행
                list.Add(entry);
            }
            catch (Exception e)
            {
                // 에러가 나도 멈추지 않고 로그만 찍고 다음 줄로 넘어감
                Debug.LogError($"CSV 파싱 오류 ({file} - {i}번 줄): {e.Message}");
            }
        }

        return list;
    }
}