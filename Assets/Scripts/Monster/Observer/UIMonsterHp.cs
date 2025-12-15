using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMonsterHp : MonoBehaviour, IMonsterHpObserver
{
    [SerializeField] private Slider _monsterHpSlider;
    [SerializeField] private TextMeshProUGUI _monsterHpText;
    [SerializeField] private MonsterStatus _monsterStatus;

    private void Awake()
    {
        _monsterStatus.AddHpObserver(this);
    }

    private void OnDestroy()
    {
        _monsterStatus.RemoveHpObserver(this);
    }

    public void OnMonsterHpChanged(int currentHp, int maxHp)
    {
        _monsterHpSlider.maxValue = maxHp;
        _monsterHpSlider.value = currentHp;
        _monsterHpText.text = $"{currentHp} / {maxHp}"; // 추후 표시 상황 변경 가능
    }
}
