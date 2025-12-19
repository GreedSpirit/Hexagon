using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMonsterHp : MonoBehaviour, IMonsterHpObserver, IMonsterEffectObserver
{
    [SerializeField] private TextMeshProUGUI _monsterHpText;
    [SerializeField] private MonsterStatus _monsterStatus;
    [SerializeField] private TextMeshProUGUI _monsterShieldText;

    [Header("HP Bar Layers")]
    [SerializeField] private Image _greyBar;   // 1. 잔상 (회색)
    [SerializeField] private Image _greenBar;  // 2. 중독 (초록)
    [SerializeField] private Image _orangeBar; // 3. 화상 (주황)
    [SerializeField] private Image _redBar;    // 4. 실제 체력 (빨강)

    [Header("Settings")]
    [SerializeField] private float _lerpSpeed = 5f; // 보간 속도

    private float _currentDisplayHp; // 회색바용 현재 표시 체력
    private float _maxHpCached;      // 최대 체력 

    private void Awake()
    {
        _monsterStatus.AddHpObserver(this);
        _monsterStatus.AddEffectObserver(this); // 효과 변경 시에도 체력바 갱신 필요 (도트 예상 구간 때문)
    }

    private void OnDestroy()
    {
        _monsterStatus.RemoveHpObserver(this);
        _monsterStatus.RemoveEffectObserver(this);
    }

    private void Update()
    {
        // 회색바(잔상) 보간 애니메이션
        if (_currentDisplayHp > _monsterStatus.MonsterCurHP)
        {
            _currentDisplayHp = Mathf.Lerp(_currentDisplayHp, _monsterStatus.MonsterCurHP, Time.deltaTime * _lerpSpeed);
            
            // 거의 다 왔으면 값 고정
            if (Mathf.Abs(_currentDisplayHp - _monsterStatus.MonsterCurHP) < 0.1f)
                _currentDisplayHp = _monsterStatus.MonsterCurHP;
        }
        else
        {
            _currentDisplayHp = _monsterStatus.MonsterCurHP;
        }

        UpdateBars();
    }

    public void OnMonsterHpChanged(int currentHp, int maxHp, int shield)
    {
        _maxHpCached = maxHp;
        UpdateBars();
        UpdateText(currentHp, shield);
    }

    // 상태 이상(중독/화상)이 추가/삭제될 때
    public void OnMonsterEffectChanged(List<MonsterStatusEffectInstance> effects)
    {
        UpdateBars();
    }

    private void UpdateBars()
    {
        if (_maxHpCached <= 0) return;

        float curHp = _monsterStatus.MonsterCurHP;
        float maxHp = _maxHpCached;

        // 1. 도트 데미지 가져오기
        int poisonDmg = _monsterStatus.GetDotDamageByType("KeyStatusPoison");
        int burnDmg = _monsterStatus.GetDotDamageByType("KeyStatusBurn");

        // 2. 각 구간
        // 회색바: 이전 체력에서 천천히 줄어드는 값
        float greyFill = _currentDisplayHp / maxHp;

        // 초록바(중독): 현재 체력 전체를 덮음 (가장 밑바닥)
        float greenFill = curHp / maxHp;

        // 주황바(화상): 현재 체력에서 중독 데미지를 뺀 만큼 덮음
        float orangeFill = Mathf.Max(0, curHp - poisonDmg) / maxHp;

        // 빨강바(안전): 현재 체력에서 모든 도트 데미지를 뺀 만큼 덮음
        float redFill = Mathf.Max(0, curHp - poisonDmg - burnDmg) / maxHp;

        // 3. UI 적용
        _greyBar.fillAmount = greyFill;
        _greenBar.fillAmount = greenFill;
        _orangeBar.fillAmount = orangeFill;
        _redBar.fillAmount = redFill;
    }

    private void UpdateText(int currentHp, int shield)
    {
        _monsterHpText.text = currentHp.ToString("N0");
        if (shield <= 0)
            _monsterShieldText.text = "";
        else
            _monsterShieldText.text = shield.ToString("N0");
    }
}

