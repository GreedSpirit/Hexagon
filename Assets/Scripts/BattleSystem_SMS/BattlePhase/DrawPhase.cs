using System;
using UnityEngine;

public class DrawPhase : IPhase
{
    
    public PhaseType GetPhaseType()
    {
        return PhaseType.Draw;
    }

    public void OnEnter()
    {
        Debug.Log("드로우 페이즈 돌입");
    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {
        
    }    
}
