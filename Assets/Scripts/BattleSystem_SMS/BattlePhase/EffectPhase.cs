using System.Collections;
using UnityEngine;

public class EffectPhase : IPhase
{
    BattleManager _battleManager;
    public EffectPhase(BattleManager manager)
    {
        _battleManager = manager;
    }


    public PhaseType GetPhaseType()
    {
        return PhaseType.Effect;
    }

    public void OnEnter()
    {
        Debug.Log("이펙트 페이즈 돌입");
        _battleManager.StartCoroutine(ViewEffects());
    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {
        
    }

    private IEnumerator ViewEffects()
    {
        // 큐에 쌓인 Effect 전부 처리
        while (_battleManager.Effects.Count > 0)
        {
            IPlayable effect = _battleManager.Effects.Dequeue();            
            yield return effect.Play();
        }

        // 다 끝났으면 복귀
        _battleManager.ReturnToPhase(_battleManager.PhaseToReturn);
    }

}
