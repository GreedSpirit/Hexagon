using UnityEngine;

public class EffectPhase : IPhase
{
    public PhaseType GetPhaseType()
    {
        return PhaseType.Effect;
    }

    public void OnEnter()
    {
        Debug.Log("이펙트 페이즈 돌입");
    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {
        
    }
    
}
