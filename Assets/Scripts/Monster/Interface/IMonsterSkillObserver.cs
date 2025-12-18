using UnityEngine;

public interface IMonsterSkillObserver
{
    public void OnMonsterSkillSelected(CardData skillData, int skillValue);
}
