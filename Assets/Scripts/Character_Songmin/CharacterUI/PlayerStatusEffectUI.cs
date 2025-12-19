using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusEffectUI : MonoBehaviour
{
    [SerializeField] StatusEffectIcon _statusIconPrefab;
    [SerializeField] Transform _iconParent;
    Dictionary<StatusEffectData, StatusEffectIcon> _icons = new Dictionary<StatusEffectData, StatusEffectIcon>();

    private void Start()
    {
        Player.Instance.OnStatusEffectChanged += UpdateStatusEffectUI;
        Player.Instance.PushStatusEffects();
    }

    private void OnDestroy()
    {
        Player.Instance.OnStatusEffectChanged -= UpdateStatusEffectUI;
    }

    public void UpdateStatusEffectUI(Dictionary<StatusEffectData, int> effects)
    {
        // 추가 / 갱신
        foreach (var pair in effects)
        {
            StatusEffectData data = pair.Key;
            int stack = pair.Value;

            if (_icons.ContainsKey(data))
            {
                _icons[data].UpdateIcon(stack);
            }
            else
            {
                CreateIcon(data, stack);
            }
        }
        // 기한 다 끝난 아이콘 제거
        List<StatusEffectData> removeList = new List<StatusEffectData>();

        foreach (var pair in _icons)
        {
            if (!effects.ContainsKey(pair.Key))
            {
                removeList.Add(pair.Key);
            }                
        }

        foreach (var data in removeList)
        {
            Destroy(_icons[data].gameObject);
            _icons.Remove(data);
        }
    }
    private void CreateIcon(StatusEffectData data, int stack)
    {
        StatusEffectIcon icon = Instantiate(_statusIconPrefab, _iconParent);
        icon.Init(data);
        icon.UpdateIcon(stack);
        _icons.Add(data, icon);
    }
}
