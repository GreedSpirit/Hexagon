using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

/// <summary>
/// 게임 시작 시 게임매니저에서 1회 생성 및 호출할 클래스
/// 파서를 통해 테이블에서 읽어온 데이터 기반으로 PlayerStat을 생성 및 반환
/// </summary>
public class PlayerStatInitializer
{
    public PlayerStat InitPlayerStat(CharacterData characterData, List<CharacterLevelData> levelDatas, List<CharacterStatData> statDatas)//인자값으로 CharactoerLevel 테이블, Character 테이블, String 테이블을 받게 할 것(파서 제작 이후)        
    {
        PlayerStat stat = new PlayerStat();
        stat.Name = SetName(characterData);
        stat.MoveSpeed = SetMoveSpeed(characterData);
        stat.LevelList = FillLevelList(levelDatas, statDatas);
        stat.SetStats();
        return stat;
    }

    private List<StatsByLevel> FillLevelList(List<CharacterLevelData> levelDatas, List<CharacterStatData> statDatas)//인자값 : CharacterLevel 테이블(이거 파서로 변환할 때 각 세로열을 리스트로 변환 가능할까요??)
    {
        List<StatsByLevel> LevelList = new List<StatsByLevel>();
        if (levelDatas.Count != statDatas.Count)
        {
            Debug.LogError("CharacterLevelData와 CharacterStatData의 개수가 일치하지 않습니다.");            
        }
        for (int i = 0; i < levelDatas.Count; i++) //실제로는 여기 6 대신 CharacterLevel 테이블의 Level리스트.Count가 들어가야 함
        {
            LevelList.Add(SetStatsByLevel((i+1), levelDatas[i], statDatas[i]));
        }
        return LevelList;
        
    }

    private StatsByLevel SetStatsByLevel(int level, CharacterLevelData levelData, CharacterStatData statData) //인자값 : int level, CharacterLevel 테이블
    {
        StatsByLevel statsByLevel = new StatsByLevel();


         statsByLevel.NeedExp = levelData.NeedExp;
         statsByLevel.Hp = statData.Hp;
         statsByLevel.Defense = statData.Defense;
        
        return statsByLevel;
    }

    private string SetName(CharacterData characterData)
    {
        string name = "???";
        string key = DataManager.Instance.GetCharacter(1).Name;
        name = DataManager.Instance.GetString(key)?.Korean;
        return name;
    }
    private float SetMoveSpeed(CharacterData characterData)//인자값 : Character 테이블
    {
        float moveSpeed = 0f;
        moveSpeed = characterData.MoveSpeed;
        return moveSpeed;
    }
}
