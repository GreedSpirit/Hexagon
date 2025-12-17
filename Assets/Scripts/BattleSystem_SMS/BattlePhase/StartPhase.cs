using UnityEngine;
public class StartPhase : IPhase
{
    public PhaseType GetPhaseType()
    {
        return PhaseType.Start;
    }

    public void OnEnter()
    {
        Debug.Log("전투 시작");
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }
}
