using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] HandManager _handManager;
    [SerializeField] Button _turnEndButton; //턴 종료 버튼
    [SerializeField] BattleUIManager _battleUIManager;


    PhaseChanger _phaseChanger;
    

    [SerializeField] MonsterStatus _currentMonster;
    PhaseType _currentPhase;

    private void Awake()
    {
        _phaseChanger = new PhaseChanger();
    }
    private void Start()
    {
        //UI용 구독        
        _battleUIManager.Timer.OnTimeOver += EndPlayerPhase;


        //페이즈 알림용 구독
        _phaseChanger.OnPhaseChanged += _handManager.OnPhaseChanged;
        _phaseChanger.OnPhaseChanged += _currentMonster.ChangePhase;        
        _phaseChanger.OnPhaseChanged += GetCurrentPhase;


        //행동 마침 알림 구독        
        _handManager.OnDrawEnd += EndDrawPhase;
        _currentMonster.OnEnemyActTurnEnd += EndMonsterActPhase;
        
    }

    private void OnDestroy() //파괴 시 구독관계 해제
    {
        //UI용 구독        
        _battleUIManager.Timer.OnTimeOver -= EndPlayerPhase;

        //페이즈 알림용 구독
        _phaseChanger.OnPhaseChanged -= _handManager.OnPhaseChanged;        
        _phaseChanger.OnPhaseChanged -= _currentMonster.ChangePhase;
        _phaseChanger.OnPhaseChanged -= GetCurrentPhase;

        //행동 마침 알림 구독
        _currentMonster.OnEnemyActTurnEnd -= EndMonsterActPhase;
    }

    //----------------------------------------------------------------
    // 페이즈 변경용 함수
    //----------------------------------------------------------------
    public void StartBattle()//배틀 시작할 때 호출
    {        
        _handManager.SetMonsterTarget(_currentMonster); //핸드 매니저에게 타겟 알려주기
        _phaseChanger.ChangePhase(new StartPhase());

        if (Player.Instance.PushHp() >= _currentMonster.MonsterCurHP)
        {
            _battleUIManager.CountTurn();
            _phaseChanger.ChangePhase(new DrawPhase());
        }
        else
        {
            _phaseChanger.ChangePhase(new EnemyActPhase());
        }
    }
    public void EndDrawPhase()
    {       
        _phaseChanger.ChangePhase(new PlayerActPhase());        
    }

    public void EndPlayerPhase()//플레이어 턴 종료 단추에 연결
    {               
        _battleUIManager.StopTimer();
        _phaseChanger.ChangePhase(new EnemyActPhase());
    }

    public void EndMonsterActPhase()
    {        
        _battleUIManager.CountTurn();
        Player.Instance.ResetShield();
        _phaseChanger.ChangePhase(new DrawPhase());
    }
    //-----------------------------------------------------------------

    private void GetCurrentPhase(PhaseType phase) //PhaseChanger와 연동해서 현재 페이즈를 받아오는 함수(내부 구독용)
    {
        _currentPhase = phase;
        if (phase == PhaseType.PlayerAct)
        {
            _battleUIManager.StartTimer();
        }        
    }

    public void OnTurnEndButtonClick()
    {        
        EndPlayerPhase();
    }

}
