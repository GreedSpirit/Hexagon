public interface IPhase
{
    public PhaseType GetPhaseType();

    public void OnEnter();
    public void OnUpdate();
    public void OnExit();
}
