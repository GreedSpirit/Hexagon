using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMonsterHp : MonoBehaviour, IMonsterHpObserver
{
    [SerializeField] private Slider _monsterHpSlider;
    [SerializeField] private TextMeshProUGUI _monsterHpText;
    [SerializeField] private MonsterStatus _monsterStatus;
    [SerializeField] private TextMeshProUGUI _monsterShieldText;

    private void Awake()
    {
        _monsterStatus.AddHpObserver(this);
    }

    private void OnDestroy()
    {
        _monsterStatus.RemoveHpObserver(this);
    }

    public void OnMonsterHpChanged(int currentHp, int maxHp, int shield)
    {
        _monsterHpSlider.maxValue = maxHp;
        _monsterHpSlider.value = currentHp;
        _monsterHpText.text = currentHp.ToString("N0"); // 추후 표시 상황 변경 가능

        if(shield <= 0)
            _monsterShieldText.text = "";
        else
            _monsterShieldText.text = shield.ToString("N0");
    }
}
