using System;

public class PhaseChanger
{
    IPhase _currentPhase;
    public event Action<PhaseType> OnPhaseChanged; // 몬스터, 플레이어 등 행동 주체들이 알맞은 행동을 할 수 있도록 현재 페이즈를 알림으로 쏴줌


    public void ChangePhase(IPhase newPhase)
    {
        _currentPhase?.OnExit();
        _currentPhase = newPhase;
        _currentPhase.OnEnter();
        OnPhaseChanged?.Invoke(_currentPhase.GetPhaseType());
    }

}
