using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] HandManager _handManager;
    PhaseChanger _phaseChanger;

    MonsterStatus _monsterStatus;


    private void Awake()
    {
        _phaseChanger = new PhaseChanger();
    }

    private void Start()
    {
        //현재 페이즈가 뭔지 알아야 하는 애들은 여기서 연결    
        //_phaseChanger.OnPhaseChanged += _handManager.(드로우 턴에 드로우 하게 만드는 매서드); (핸드 매니저의 드로우 판단 매서드가 인자값으로 PhaseType을 받아 PhaseType.Draw일 경우에만 드로우하도록 하면 좋을듯)
        // 이 밑으로 마찬가지 연결


        //자기 구독 연결은 여기로
        //(배틀 시작 주체가 발행한 시작 시점 액션) += StartBattle();
        //HandManager가 발행한 Draw 끝난 시점 액션 += EndDrawPhase();
        //몬스터가 발행한 턴 마지막 시점의 액션 += EndActPhase();
        

        //나중에 Destroy에서 구독 해제도 해줄것
    }

    public void StartBattle()
    {
        //if (Player.Instance.PushHp >= MonsterStatus.(체력 받아오는 함수나 프로퍼티))
        {
            _phaseChanger.ChangePhase(new DrawPhase());
        }
        //else
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
        _phaseChanger.ChangePhase(new EnemyActPhase());
    }

    public void EndMonsterActPhase()
    {
        _phaseChanger.ChangePhase(new DrawPhase());
    }
}
