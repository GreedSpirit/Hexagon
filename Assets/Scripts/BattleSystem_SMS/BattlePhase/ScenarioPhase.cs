using UnityEngine;

public class ScenarioPhase : IPhase
{
    public PhaseType GetPhaseType()
    {
        return PhaseType.Scenario;
    }

    public void OnEnter()
    {
        //Player.Instance.PlayScenario(Player.Instance.CurrentPlayedScenario);
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }
}
