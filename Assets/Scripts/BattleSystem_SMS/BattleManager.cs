using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] HandManager _handManager;
    [SerializeField] Button _turnEndButton; //턴 종료 버튼
    [SerializeField] BattleUIManager _battleUIManager;


    PhaseChanger _phaseChanger;
    public PhaseType PhaseToReturn {  get; private set; }
    PhaseType _currentPhase;
    [SerializeField] MonsterStatus _currentMonster; // 추후 스테이지 및 던전 추가되면 받아오기로 함. 지금만 인스펙터 연결.
    public Queue<IPlayable> Effects {  get; private set; }



    private void Awake()
    {
        _phaseChanger = new PhaseChanger();
        Effects = new Queue<IPlayable>();
    }
    private void Start()
    {
        //UI용 구독        
        _battleUIManager.Timer.OnTimeOver += EndPlayerPhase;


        //페이즈 알림용 구독
        _phaseChanger.OnPhaseChanged += _handManager.OnPhaseChanged;
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

        if (Player.Instance.GetCurrentHp() >= _currentMonster.MonsterCurHP)
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
        Player.Instance.ApplyStatusEffect();
        _phaseChanger.ChangePhase(new EnemyActPhase()); //이펙트 생기면 이부분 지우고 아래 두줄 활성화
        //_phaseChanger.ChangePhase(new EffectPhase(this));
        //PhaseToReturn = PhaseType.EnemyAct;
    }

    public void EndMonsterActPhase()
    {        
        _battleUIManager.CountTurn();
        Player.Instance.ResetShield();
        _currentMonster.ApplyStatusEffect();
        _phaseChanger.ChangePhase(new DrawPhase());        
    }
    //-----------------------------------------------------------------
    public void SetMonster(MonsterStatus monster)
    {
        _currentMonster = monster;
        _phaseChanger.OnPhaseChanged += _currentMonster.ChangePhase;
    }



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
        if (_currentPhase == PhaseType.PlayerAct)
        {
            EndPlayerPhase();
        }        
    }

    //------------------------------------------------------------------------------

    public void RequestAction(IPlayable action)
    {
        Effects.Enqueue(action);
        
        if (_currentPhase != PhaseType.Effect)
        {
            PhaseToReturn = _currentPhase;
            _phaseChanger.ChangePhase(new EffectPhase(this));
        }
    }

    public void ReturnToPhase(PhaseType type)
    {
        switch (type)
        {
            case PhaseType.PlayerAct:
                _phaseChanger.ChangePhase(new PlayerActPhase());
                break;
            case PhaseType.EnemyAct:
                _phaseChanger.ChangePhase(new EnemyActPhase());
                break;
            case PhaseType.Draw:
                _phaseChanger.ChangePhase(new DrawPhase());
                break;
        }        
    }
    

}
