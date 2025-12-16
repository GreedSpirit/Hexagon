using UnityEngine;

public class PlayerActPhase : IPhase
{
    public PhaseType GetPhaseType()
    {
        return PhaseType.PlayerAct;
    }

    public void OnEnter()
    {
        Debug.Log("플레이어 페이즈 돌입");
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }
}
