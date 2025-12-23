using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerStatusEffectUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("상태효과 툴팁 관련 필드")]
    [SerializeField] PlayerTultipUI _tultipUI;
    [SerializeField] Transform _tultipParent;
    Dictionary<StatusEffectData, PlayerTultipUI> _tultips = new Dictionary<StatusEffectData, PlayerTultipUI>();

    [Header("상태효과 아이콘 관련 필드")]
    [SerializeField] StatusEffectIcon _statusIconPrefab;
    [SerializeField] Transform _iconParent;
    Dictionary<StatusEffectData, StatusEffectIcon> _icons = new Dictionary<StatusEffectData, StatusEffectIcon>();
    

    Dictionary<StatusEffectData, int> _createOrder = new();

    int _createOrderCounter = 0;
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
                _tultips[data].UpdateTultip(stack);
            }
            else
            {
                CreateIcon(data, stack);
                CreateTultip(data, stack);
            }
        }
        // 기한 다 끝난 아이콘 및 툴팁 제거
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
            Destroy(_tultips[data].gameObject);
            _icons.Remove(data);
            _tultips.Remove(data);
            _createOrder.Remove(data);
        }
        SortIconsAndTultip(effects);
    }
    private void CreateIcon(StatusEffectData data, int stack)
    {
        StatusEffectIcon icon = Instantiate(_statusIconPrefab, _iconParent);
        icon.Init(data);
        icon.UpdateIcon(stack);
        _icons.Add(data, icon);
        _createOrder[data] = _createOrderCounter++;
    }
    private void CreateTultip(StatusEffectData data, int stack)
    {
        PlayerTultipUI tultip = Instantiate(_tultipUI, _tultipParent);
        tultip.Init(data);
        tultip.UpdateTultip(stack);        
        _tultips.Add(data, tultip);
        
    }
    private void SortIconsAndTultip(Dictionary<StatusEffectData, int> effects)
    {
        var sorted = _icons
            .Select(pair => new
            {
                Data = pair.Key,
                Icon = pair.Value,
                Tultip = _tultips[pair.Key],
                Stack = effects[pair.Key],
                Order = _createOrder[pair.Key]
            })
            .OrderByDescending(x => x.Stack)                         // 1순위
            .ThenBy(x => GetBuffPriority(x.Data.BuffType))           // 2순위
            .ThenBy(x => GetStatusPriority(x.Data))                  // 3순위 (독 > 화상)
            .ThenBy(x => x.Order)                                    // 4순위
            .ThenBy(x => x.Data.Id)                                  // 5순위
            .ToList();

        for (int i = 0; i < sorted.Count; i++)
        {
            sorted[i].Icon.transform.SetSiblingIndex(i);
            sorted[i].Tultip.transform.SetSiblingIndex(i);
        }
    }

    private int GetBuffPriority(BuffType type)
    {
        switch (type)
        {
            case BuffType.Buff: return 0;
            case BuffType.DeBuff: return 1;            
            default: return 2;
        }
    }

    private int GetStatusPriority(StatusEffectData data)
    {
        if (data.BuffType != BuffType.DoT)
            return 0;

        if (data.Key == "KeyStatusPoison")
            return 0;
        if (data.Key == "KeyStatusBurn")
            return 1;

        return 2;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tultipParent.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tultipParent.gameObject.SetActive(false);
    }
}
