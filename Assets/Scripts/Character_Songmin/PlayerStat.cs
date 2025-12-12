using UnityEngine;

public class PlayerStat
{
    //Character 테이블에서 받아올 필드.
    public string Name { get; private set; }
    public int MoveSpeed { get; private set; }

    //CharacterLevel 테이블에서 받아올 필드.
    public int Hp { get; private set; }
    public int Defense { get; private set; }
    public int NeedExp { get; private set; }
    public int TotalExp { get; private set; }

    //플레이에 따라 변동되는 필드.
    public int Level { get; set; }
    public int CurrentExp { get; set; }
    public int CurrentHp {  get; set; }    
    public int Shield { get; set; }



    
    public void FirstInit()//맨 처음 캐릭터 생성할 때 호출
    {
        Level = 1;
        SetName();
        SetMoveSpeed();
        SetStat();
        
        CurrentExp = 0;
        CurrentHp = Hp;
    }

    public void LevelUp()
    {
        //if(_level < Character Level 테이블의 id의 개수)
        {
            Level++;
            SetStat();
        }
        //else
        {
            Debug.Log("이미 최대 레벨입니다.");
        }        
    }

















    private void SetStat()
    {
        // HP = Character Level 테이블에 id : _level 의 HP
        // Defense = Character Level 테이블에 id : _level의 Defense
        // NeedExp = Character Level 테이블에 id : _level의 NeedExp
        // TotalExp = Character Level 테이블에 id : _level의 TotalExp
    }

    private void SetName()
    {
        //string key = Character 테이블에 id : 1 의 Name
        //Name =  String Table에 id : key의 한국어나 영어
    }
    private void SetMoveSpeed()
    {
        //MoveSpeed = Character 테이블에 id : 1 의 MoveSpeed
    }
}
