using UnityEngine;

public class EndPhase : IPhase
{
    public PhaseType GetPhaseType()
    {
        return PhaseType.End;
    }

    public void OnEnter()
    {
        Debug.Log("엔드 페이즈 돌입");
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {

    }
}