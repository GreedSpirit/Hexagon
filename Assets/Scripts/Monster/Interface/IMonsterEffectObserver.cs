using System.Collections.Generic;
using UnityEngine;

public interface IMonsterEffectObserver
{
    void OnMonsterEffectChanged(List<MonsterStatusEffectInstance> effects);
}
