using UnityEngine;
public class EnemyActPhase : IPhase
{
    public PhaseType GetPhaseType()
    {
        return PhaseType.EnemyAct;
    }

    public void OnEnter()
    {
        Debug.Log("에너미 페이즈 돌입");
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }
}
