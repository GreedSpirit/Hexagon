using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 시작 시 게임매니저에서 1회 생성 및 호출할 클래스
/// 파서를 통해 테이블에서 읽어온 데이터 기반으로 PlayerStat을 생성 및 반환
/// </summary>
public class PlayerStatInitializer
{
    public PlayerStat InitPlayerStat()//인자값으로 CharactoerLevel 테이블, Character 테이블, String 테이블을 받게 할 것(파서 제작 이후)        
    {
        PlayerStat stat = new PlayerStat();
        stat.Name = SetName();
        stat.MoveSpeed = SetMoveSpeed();
        stat.LevelList = FillLevelList();
        stat.SetStats();
        return stat;
    }

    private List<StatsByLevel> FillLevelList()//인자값 : CharacterLevel 테이블(이거 파서로 변환할 때 각 세로열을 리스트로 변환 가능할까요??)
    {
        List<StatsByLevel> LevelList = new List<StatsByLevel>();
        for (int i = 0; i < 6; i++) //실제로는 여기 6 대신 CharacterLevel 테이블의 Level리스트.Count가 들어가야 함
        {
            LevelList.Add(SetStatsByLevel(i+1));
        }
        return LevelList;
        
    }

    private StatsByLevel SetStatsByLevel(int level) //인자값 : int level, CharacterLevel 테이블
    {
        StatsByLevel statsByLevel = new StatsByLevel();


        // statsByLevel.HP = Character Level 테이블에 HP[level - 1];
        // statsByLevel.Defense = Character Level 테이블에 Defense[level - 1];
        // statsByLevel.NeedExp = Character Level 테이블에 NeedExp[level - 1];
        // statsByLevel.TotalExp = Character Level 테이블에 TotalExp[level - 1];
        return statsByLevel;
    }

    private string SetName() //인자값 : Character 테이블, String 테이블
    {
        string name = "";
        //string key = Character 테이블에 id : 1 의 Name
        //name =  String Table에 id : key의 한국어나 영어
        return name;
    }
    private float SetMoveSpeed()//인자값 : Character 테이블
    {
        float moveSpeed = 0f;
        //MoveSpeed = Character 테이블에 id : 1 의 MoveSpeed
        return moveSpeed;
    }
}
