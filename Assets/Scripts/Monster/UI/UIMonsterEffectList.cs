using System.Collections.Generic;
using UnityEngine;

public class UIMonsterEffectList : MonoBehaviour, IMonsterEffectObserver
{
    [SerializeField] private MonsterStatus _monsterStatus;
    [SerializeField] private GameObject _iconPrefab; // UIMonsterEffectIcon이 붙은 프리팹
    [SerializeField] private Transform _iconParent;  // Horizontal Layout Group이 있는 오브젝트

    private void Awake()
    {
        _monsterStatus.AddEffectObserver(this);
    }

    private void OnDestroy()
    {
        _monsterStatus.RemoveEffectObserver(this);
    }

    public void OnMonsterEffectChanged(List<MonsterStatusEffectInstance> effects)
    {
        // 기존 아이콘 싹 지우기 (추후 Object Pooling 바꿀 듯)
        foreach (Transform child in _iconParent)
        {
            Destroy(child.gameObject);
        }

        // 정렬된 리스트를 순회하며 아이콘 생성
        foreach (var effect in effects)
        {
            GameObject iconObj = Instantiate(_iconPrefab, _iconParent);
            UIMonsterEffectIcon iconScript = iconObj.GetComponent<UIMonsterEffectIcon>();
            
            iconScript.Init(effect, _monsterStatus);
        }
    }
}