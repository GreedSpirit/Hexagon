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
        string[] lines = data.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 4) return list; // 헤더(1~3행) 제외 데이터가 없으면 리턴

        // 4행(인덱스 3)부터 실제 데이터 시작
        for (int i = 3; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');

            // 데이터가 비어있으면 건너뛰기
            if (values.Length == 0 || string.IsNullOrEmpty(values[0])) continue;

            T entry = new T();

            // LoadFromCsv 함수 호출
            entry.LoadFromCsv(values);

            list.Add(entry);
        }

        return list;
    }
}