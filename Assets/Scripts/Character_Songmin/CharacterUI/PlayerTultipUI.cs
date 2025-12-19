using TMPro;
using UnityEngine;

//개별 툴팁 UI 프리팹에 붙이기
public class PlayerTultipUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _tultipName;
    [SerializeField] TextMeshProUGUI _tultipDesc;
    StatusEffectData _data;
    string _basicDesc;

    public void Init(StatusEffectData data)
    {
        _data = data;
        StringData nameString = DataManager.Instance.GetString(data.Name);
        StringData DescString = DataManager.Instance.GetString(data.Desc);
        _tultipName.text = nameString.Korean;
        _tultipDesc.text = DescString.Korean;
        
        _basicDesc = _tultipDesc.text;
    }

    public void UpdateTultip(int turn)
    {
        string turns;
        if (_data.BuffType == BuffType.DoT)
        {
            turns = $"<color=#415D66>스택 수: {turn}</color>";
            
        }
        else
        {
            turns = $"지속 턴 수: {turn}";            
        }
        _tultipDesc.text = $"{_basicDesc} {turns}";

    }
}
