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
    

    MonsterStatus _currentMonster;
    PhaseType _currentPhase;

    private void Awake()
    {
        _phaseChanger = new PhaseChanger();
    }
    private void Start()
    {
        _turnEndButton.onClick.AddListener(OnTurnEndButtonClick);
        _battleUIManager.Timer.OnTimeOver += EndPlayerPhase;
        

        //현재 페이즈를 PhaseType 자료형으로 전달해 주는 구독 연결
        //_phaseChanger.OnPhaseChanged += _handManager.(드로우 턴에 드로우 하게 만드는 매서드); (핸드 매니저의 드로우 판단 매서드가 인자값으로 PhaseType을 받아 PhaseType.Draw일 경우에만 드로우하도록 하면 좋을듯)
        _phaseChanger.OnPhaseChanged += _currentMonster.ChangePhase;
        _phaseChanger.OnPhaseChanged += GetCurrentPhase;

        //---------------------------------------------------------------------
        //행동이 끝났음을 전달받는 구독 연결

        //(배틀 시작 주체가 발행한 시작 시점 액션) += StartBattle();
        //HandManager가 발행한 Draw 끝난 시점 액션 += EndDrawPhase();
        _currentMonster.OnEnemyActTurnEnd += EndMonsterActPhase;

        //나중에 Destroy에서 구독 해제도 해줄것
    }

    private void OnDestroy() //파괴 시 구독관계 해제
    {
        _turnEndButton.onClick.RemoveListener(OnTurnEndButtonClick);
        _phaseChanger.OnPhaseChanged -= _currentMonster.ChangePhase;
        _currentMonster.OnEnemyActTurnEnd -= EndMonsterActPhase;
    }

    //----------------------------------------------------------------
    // 페이즈 변경용 함수
    //----------------------------------------------------------------
    public void StartBattle()//배틀 시작할 때 호출
    {
        _handManager.SetMonsterTarget(_currentMonster); //핸드 매니저에게 타겟 알려주기
        

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
        if (_currentPhase != PhaseType.Draw)
        {
            return;
        }
        _phaseChanger.ChangePhase(new PlayerActPhase());
        _battleUIManager.StartTimer();
    }

    public void EndPlayerPhase()//플레이어 턴 종료 단추에 연결
    {
        if (_currentPhase != PhaseType.PlayerAct)
        {
            return;
        }        
        _battleUIManager.StopTimer();
        _phaseChanger.ChangePhase(new EnemyActPhase());
    }

    public void EndMonsterActPhase()
    {
        if (_currentPhase != PhaseType.EnemyAct)
        {
            return;
        }
        _battleUIManager.CountTurn();
        _phaseChanger.ChangePhase(new DrawPhase());
    }
    //-----------------------------------------------------------------

    private void GetCurrentPhase(PhaseType phase) //PhaseChanger와 연동해서 현재 페이즈를 받아오는 함수(내부 구독용)
    {
        _currentPhase = phase;        
    }

    public void OnTurnEndButtonClick()
    {        
        EndPlayerPhase();
    }

}
