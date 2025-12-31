using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 소환된 몬스터의 상태(체력, 방어력 등)를 관리하는 클래스
/// </summary>

public class MonsterStatus : MonoBehaviour, IBattleUnit
{
    public MonsterData MonsterData => _monsterData; //외부에서 몬스터 데이터에 접근할 수 있는 프로퍼티
    [SerializeField] private int _monsterId = 1; //몬스터 데이터 ID
    [SerializeField] private int _monsterLevel; //추후 스테이지 테이블에서 불러올 내용
    [SerializeField] private MonsterGrade _monsterGrade;
    [SerializeField] private int _monsterMaxHP;
    [SerializeField] private int _monsterCurHP;
    public int MonsterCurHP => _monsterCurHP; // 외부에서 몬스터의 현재 체력에 접근할 수 있는 프로퍼티
    [SerializeField] private int _monsterDefense;
    [SerializeField] private int _monsterShield = 0;
    [SerializeField] private MonsterData _monsterData;
    [SerializeField] private MonsterSkillSetData _monsterSkillSet;
    [SerializeField] private MonsterStatData _monsterStatData;
    [SerializeField] private CardData _currentSkillData; //현재 사용될 예정인 스킬 카드 데이터
    [SerializeField] private MonsterVisual _visual; //인스펙터에서 연결

    [Header("Sound Settings")]
    [SerializeField] private List<MonsterSoundStruct> _soundList;
    private Dictionary<MonsterSoundType, AudioClip> _soundTable = new Dictionary<MonsterSoundType, AudioClip>();

    [Header("VFX")]
    [SerializeField] private MonsterVFXController _vfxController;

    private List<IMonsterHpObserver> _hpObservers = new List<IMonsterHpObserver>(); //옵저버 목록을 관리할 List
    private List<IMonsterSkillObserver> _skillObservers = new List<IMonsterSkillObserver>();
    private List<IMonsterEffectObserver> _effectObservers = new List<IMonsterEffectObserver>();
    private string _selectedSkillKey = null; //선택된 스킬 키 임시 변수
    private int _selectedSkillSlot = -1; //선택된 스킬 슬롯 인덱스 임시 변수
    private int _selectedSkillValue = -1;
    private bool _isDead = false;
    private MonsterDeathEffect _deathEffect;

    private List<MonsterStatusEffectInstance> _statusEffects = new List<MonsterStatusEffectInstance>(); //몬스터에게 적용된 상태 이상 효과 인스턴스 목록
    public List<MonsterStatusEffectInstance> StatusEffects => _statusEffects;

    public event System.Action OnEnemyActTurnEnd; // 몬스터 턴 종료 시점에 발행할 이벤트
    public event System.Action<MonsterStatus> OnMonsterDeath; // 몬스터가 죽은 시점에 스테이지를 넘기거나 보상 방으로 진행할 이벤트

    public void ChangePhase(PhaseType newPhase) //OnPhaseChanged 이벤트 구독용 함수
    {
        if (this == null || gameObject == null || !gameObject.activeInHierarchy) 
        {
            return;
        }
        //유저 드로우 턴 페이즈일 때 행동
        if(newPhase == PhaseType.Draw || newPhase == PhaseType.Start)
        {
            GetRandomSkillFromSet();
        }
        else if(newPhase == PhaseType.EnemyAct)
        {
            StartCoroutine(ProcessEnemyAction());
        }
    }

    public void InitMonsterStatus(int monsterId, int level) //몬스터 스탯 초기화 함수
    {
        _monsterId = monsterId;
        _monsterData = DataManager.Instance.GetMonsterStatData(_monsterId);
        if(_monsterData == null)
        {
            Debug.LogError("몬스터 스탯 데이터를 불러오지 못했습니다. Id: 1");
            return;
        }
        
        if(_visual != null)
        {
            _visual.SetVisual(_monsterData.Model);
        }
        _monsterGrade = _monsterData.MonGrade;
        _monsterLevel = level;

        if(_monsterGrade == MonsterGrade.Normal)
        {
            _monsterStatData = DataManager.Instance.GetCommonMonsterStatData(_monsterLevel);

            _monsterMaxHP = Mathf.FloorToInt(_monsterStatData.Hp * _monsterData.HpRate); 
            _monsterDefense = Mathf.FloorToInt(_monsterStatData.Defense * _monsterData.DefRate);
        }
        else if(_monsterGrade == MonsterGrade.Boss)
        {
            _monsterStatData = DataManager.Instance.GetBossMonsterStatData(_monsterLevel);
            _monsterMaxHP = Mathf.FloorToInt(_monsterStatData.Hp * _monsterData.HpRate);
            _monsterDefense = Mathf.FloorToInt(_monsterStatData.Defense * _monsterData.DefRate);
        }
        _monsterSkillSet = DataManager.Instance.GetMonsterSkillSetData(_monsterData.SkillSet);
        _monsterCurHP = _monsterMaxHP;        

        NotifyHpObservers(); //초기 HP 상태 갱신
        NotifySkillObservers(); //초기 스킬 상태 갱신

        //사운드 작업(리스트를 빠른 검색을 위해서 딕셔너리로 전환)
        _soundTable.Clear();
        foreach(var soundData in _soundList)
        {
            if (!_soundTable.ContainsKey(soundData.type))
            {
                _soundTable.Add(soundData.type, soundData.clip);
            }
        }

        //등장 사운드 있을 시 여기
    }

    private void Start()
    {
        _deathEffect = GetComponent<MonsterDeathEffect>();  
    }

    public void TakeDamage(int damage) // 데미지를 입을 때 호출할 함수
    {
        if(_isDead) return;

        
        _vfxController.PlayHitEffect();

        if(_monsterShield > 0)
        {
            int shieldDamage = Mathf.Min(_monsterShield, damage);
            _monsterShield -= shieldDamage;
            damage -= shieldDamage;
            PlayMonsterSound(MonsterSoundType.Hit_Shield);
        }
        _monsterCurHP -= damage;
        if(damage > 0)
        {
            PlayMonsterSound(MonsterSoundType.Hit_Body);
            DamageTextManager.Instance.ShowDamage(damage, transform.position, DamageType.Normal); // 변경 : 보호막 수치 제외한 데미지만 텍스트로 표기
        }
        if (_monsterCurHP < 0)
            _monsterCurHP = 0;
        NotifyHpObservers(); //HP 변경 알림

        if (_monsterCurHP <= 0)
        {
            Death();
        }
        else
        {
            _visual.PlayHit();
        }
    }

    public float TakenDamageMultiplier() //약화 효과가 걸려있다면 데미지의 배율을 결정해서 정보를 전달
    {
        float multiplier = 1.0f;
        var vulnerableMods = _statusEffects.FindAll(e => e.EffectLogic == EffectLogic.DmgTaken);

        foreach(var mod in vulnerableMods)
        {
            multiplier += mod.Value;
        }

        return multiplier;
    }

    private float GetStatModMultiplier()
    {
        float multiplier = 1.0f;
        var StatMods = _statusEffects.FindAll(e => e.EffectLogic == EffectLogic.StatMod);

        foreach(var mod in StatMods)
        {
            multiplier += mod.Value;
        }
        return multiplier;
    }

    public int GetMonsterDefense()
    {
        return Mathf.Min(_monsterDefense, 7);
    }

    public void TakeTrueDamage(int damage) // 방어력 무시 데미지(상태 이상 데미지)를 입을 때 호출할 함수
    {
        _monsterCurHP -= damage;
        if (_monsterCurHP < 0)
            _monsterCurHP = 0;
        NotifyHpObservers(); //HP 변경 알림

        if (_monsterCurHP == 0)
        {
            Death();
        }
    }

    public void GetHp(int hp) // 체력을 회복할 때 호출할 함수
    {
        _monsterCurHP += hp;
        if (_monsterCurHP > _monsterMaxHP)
            _monsterCurHP = _monsterMaxHP;
        NotifyHpObservers(); //HP 변경 알림
    }

    public void GetShield(int shield) // 방어막을 얻을 때 호출할 함수
    {
        _monsterShield += shield;
        _monsterShield = Mathf.Min(_monsterShield, 99); //방어막 최대치 제한
        NotifyHpObservers(); //쉴드 변경 알림
    }

    private void Death()
    {
        if(_isDead) return;
        _visual.PlayDie();
        PlayMonsterSound(MonsterSoundType.Die);

        _isDead = true;
        
        //몬스터 죽음 처리 로직 추가
        Debug.Log("몬스터가 죽었습니다. ID: " + _monsterId);
        OnMonsterDeath?.Invoke(this); //이 몬스터가 죽었음을 알림
        _deathEffect.Die();


        if(_monsterGrade == MonsterGrade.Boss)
        {
            DropLoot();
        }
    }

    private void DropLoot()
    {
        //보스 몬스터 죽음 시 특별 처리 로직 추가
    }

    // 스킬 사용 및 공격 로직 시작 ------------------------------------------------------------

    private void GetRandomSkillFromSet() // 몬스터의 스킬셋에서 가중치에 따라 랜덤으로 스킬을 선택하는 함수
    {
        if(_monsterSkillSet == null || _monsterSkillSet.skillWeights.Count == 0)
        {
            Debug.LogError("몬스터 스킬셋 데이터가 없거나 스킬이 설정되어 있지 않습니다.");
            return;
        }
        if(_selectedSkillKey != null)
        {
            // 선턴을 잡을 경우 or 버그 방지 방어 코드 겸용
            Debug.Log("이미 선택된 스킬이 있습니다. 스킬 선택을 건너뜁니다.");
            return;
        }
        float totalRate = 0f;
        foreach(var skillweight in _monsterSkillSet.skillWeights)
        {
            totalRate += skillweight.Item2;
        }

        int idx = -1;
        float randomValue = Random.Range(0f, totalRate);
        float curSum = 0f;

        foreach(var skillSlot in _monsterSkillSet.skillWeights)
        {
            idx++;
            curSum += skillSlot.Item2;
            if(randomValue <= curSum)
            {
                Debug.Log("선택된 스킬 가중치: " + skillSlot.Item2);
                Debug.Log("선택된 스킬 레벨: " + _monsterSkillSet.skillLevels[idx]);
                _selectedSkillSlot = idx; //선택된 스킬 슬롯 인덱스 저장
                _selectedSkillKey = skillSlot.Item1;
                _currentSkillData = DataManager.Instance.GetCard(_selectedSkillKey); //선택된 스킬 카드 데이터 저장
                _selectedSkillValue = _currentSkillData.BaseValue + (_monsterSkillSet.skillLevels[_selectedSkillSlot] - 1) * _currentSkillData.ValuePerValue;
                if(_currentSkillData.CardType == CardType.Attack)
                {
                    _selectedSkillValue = Mathf.FloorToInt(_selectedSkillValue * GetStatModMultiplier());
                }
                //이곳에서 스킬 타입에 따른 아이콘과 수치 UI 갱신 로직 추가
                NotifySkillObservers(); //?선택된 스킬을 알림으로써 UI 갱신
                break;
            }
        }        
    }

    private void UseSkill() //턴마다 스킬을 사용하는 함수
    {
        if(_isDead) return;
        
        //_selectedSkillKey를 이용해서 스킬을 가져오고 _selectedSkillSlot 인덱스를 이용해서 스킬 레벨도 가져오기
        if(_selectedSkillKey != null)
        {
            //스킬 사용 로직 추가
            Debug.Log("몬스터가 스킬을 사용했습니다. 스킬 키: " + _selectedSkillKey + ", 스킬 레벨: " + _monsterSkillSet.skillLevels[_selectedSkillSlot]);
            
            if(_currentSkillData.CardType == CardType.Attack)
            {
                if(_currentSkillData.Key == "KeyCardFireless")
                {
                    _visual.PlayFireBall();
                    PlayMonsterSound(MonsterSoundType.Atk_Explosion);
                }
                else if(_currentSkillData.Key == "KeyCardFireball")
                {
                    PlayMonsterSound(MonsterSoundType.Atk_Fire);
                }
                else
                {
                    PlayMonsterSound(MonsterSoundType.Atk_Swing);
                }
                    
                _visual.PlayAttack();

                Player.Instance.TakeDamage(_selectedSkillValue); //추후 인자값으로 공격 강화 상태를 받을수도있음
                Debug.Log("플레이어에게 " + _selectedSkillValue + "의 데미지를 입혔습니다.");
            }
            else if(_currentSkillData.CardType == CardType.Healing)
            {
                _visual.PlaySkill();
                _vfxController.PlayHealEffect();
                GetHp(_selectedSkillValue);
                Debug.Log("몬스터가 " + _selectedSkillValue + "의 체력을 회복했습니다.");
            }
            else if(_currentSkillData.CardType == CardType.Shield)
            {
                GetShield(_selectedSkillValue);
                Debug.Log("몬스터가 " + _selectedSkillValue + "의 방어막을 얻었습니다.");
            }
            else
            {
                if(_currentSkillData.Key == "KeyCardLazer")
                {
                    PlayMonsterSound(MonsterSoundType.Atk_Laser);
                }
                if(_currentSkillData.Key == "KeyCardPride")
                {
                    PlayMonsterSound(MonsterSoundType.Atk_Pride);
                    _vfxController.PlayStatusEffect("KeyStatusPride");
                }
                _visual.PlayBig();
            }

            if(_currentSkillData.Target == Target.Self)
            {
                AddStatusEffect(_currentSkillData.StatusEffect, _currentSkillData.StatusEffectValue, _currentSkillData.Turn);
            }
            else if(_currentSkillData.Target == Target.Enemy){
                Player.Instance.AddStatusEffect(_currentSkillData.StatusEffect, _currentSkillData.StatusEffectValue, _currentSkillData.Turn);
            }


            //사용 후 선택된 스킬 초기화 (방어 코드 및 선턴을 잡을 경우 예외 사항)
            _selectedSkillKey = null;
            _selectedSkillValue = -1;
            _selectedSkillSlot = -1;
            NotifySkillObservers(); //스킬 사용 후 스킬 초기화 알림
        }
        else
        {
            Debug.LogError("선택된 스킬이 없습니다.");
        }
    }

    // 상태이상 효과 관련 메서드 ------------------------------------------------------------
    //상태이상 Key와 Duration, Stack를 받아서 상태이상을 적용하는 함수
    public void AddStatusEffect(string effectKey, int duration, int stack)
    {
        if(_isDead) return;
        if(effectKey == "") return;
        var tableData = DataManager.Instance.GetStatusEffectData(effectKey);

        var existingEffect = _statusEffects.Find(e => e.Key == effectKey);

        _vfxController.PlayStatusEffect(effectKey);

        if(existingEffect != null)
        {
            existingEffect.Duration += duration;
            existingEffect.Stack += stack;
        }
        else
        {
            MonsterStatusEffectInstance newEffect = new MonsterStatusEffectInstance()
            {
                Id = tableData.Id,
                Key = tableData.Key,
                Name = tableData.Name,
                Type = tableData.BuffType,
                Duration = duration,
                Stack = stack,
                EffectLogic = tableData.EffectLogic,
                Value = tableData.ValueFormula,
                Desc = tableData.Desc,
                AppliedTime = Time.time, // 적용 시점을 위함, 나중에 턴으로 바꿀수도
                Img = tableData.Img,
                decreaseType = tableData.DecreaseType,
            };
            
            _statusEffects.Add(newEffect);            
        }

        _statusEffects.Sort((a, b) => a.GetSortOrder(b)); //정렬

        NotifyEffectObservers();
        _selectedSkillValue = _currentSkillData.BaseValue + (_monsterSkillSet.skillLevels[_selectedSkillSlot] - 1) * _currentSkillData.ValuePerValue;
        _selectedSkillValue = Mathf.FloorToInt(_selectedSkillValue * GetStatModMultiplier());
        NotifySkillObservers(); //만약 상대방이 나에게 상태이상을 걸었을 때 밸류를 다시 체크해서 UI에 반영
    }

    public void TickStatusEffects() //지속 시간 감소를 위한 메서드
    {
        for(int i = _statusEffects.Count - 1; i >= 0; i--) // RemoveAt이 앞에서부터 될 경우 오류 발생 가능
        {
            _statusEffects[i].Duration--;
            _statusEffects[i].Stack--;
            if(_statusEffects[i].Duration <= 0 && _statusEffects[i].Stack <= 0)
            {
                _statusEffects.RemoveAt(i);
            }
        }
        _statusEffects.Sort((a, b) => a.GetSortOrder(b)); //정렬
        NotifyEffectObservers();
    }

    public IEnumerator ProcessDotEffectsCoroutine()
    {
        int poisionStack = GetDotDamageByType("KeyStatusPoison");
        if(poisionStack > 0)
        {
            Debug.Log("중독 피해 : " + poisionStack);
            TakeTrueDamage(poisionStack);
            yield return new WaitForSeconds(0.5f);
        }

        int burnStack = GetDotDamageByType("KeyStatusBurn");
        if(burnStack > 0)
        {
            Debug.Log("화상 피해 : " + burnStack);
            TakeTrueDamage(burnStack);
            yield return new WaitForSeconds(0.5f);
        }

        //지속 시간 감소는 턴이 끝날 때 일괄적으로 TickStatusEffects()에서 처리
    }

    public IEnumerator ProcessDotEffects()
    {
        int poisonDmg = GetDotDamageByType("KeyStatusPoison");
        int burnDmg = GetDotDamageByType("KeyStatusBurn");
        int totalDotDmg = poisonDmg + burnDmg;

        TakeTrueDamage(totalDotDmg);

        if (poisonDmg > 0)
        {
            DamageTextManager.Instance.ShowDamage(poisonDmg, transform.position, DamageType.Poison);
            _vfxController.PlayStatusEffect("KeyStatusPoison");
            PlayMonsterSound(MonsterSoundType.Hit_Poison);
        }
        // 2개의 상태 이상 데미지가 모두 들어올 경우 간격을 주어 텍스트가 겹치는 문제 해결
        if (poisonDmg > 0 && burnDmg > 0) yield return new WaitForSeconds(0.2f);
        
        if(burnDmg > 0)
        {
            DamageTextManager.Instance.ShowDamage(burnDmg, transform.position, DamageType.Burn);
            _vfxController.PlayStatusEffect("KeyStatusBurn");
            PlayMonsterSound(MonsterSoundType.Hit_Burn);
        }
        yield return new WaitForSeconds(1f);
    }

    public int GetTotalDotDamage() //도트뎀으로 얻는 총 데미지를 얻고 싶을 때 사용할 메서드
    {
        int damage = 0;
        var dots = _statusEffects.FindAll(e => e.EffectLogic == EffectLogic.TurnEndDmg);
        foreach (var dot in dots)
        {
            damage += dot.Stack;
        }
        return damage;
    }

    public int GetDotDamageByType(string key) //각 타입에 따라서 어느정도의 데미지가 들어오는지 받기
    {
        var effect = _statusEffects.Find(e => e.Key == key);
        if(effect != null && effect.EffectLogic == EffectLogic.TurnEndDmg)
        {
            return effect.Stack;
        }
        return 0;
    }

    public void ApplyStatusEffect() //IBattleUnit
    {
        StartCoroutine(ProcessDotEffects());
        TickStatusEffects();
    }

    private bool GetStatusEffect(string key)
    {
        var effect = _statusEffects.Find(e => e.Key == key);
        return effect != null;
    }


    //TODO 유니티 에디터를 사용할 때 체력 소모 이벤트에 대한 테스트를 위한 코드 추후 삭제 필요
    public void TestTakeDamage()
    {
        TakeDamage(10);
    }

    public void TestSelectSkill()
    {
        GetRandomSkillFromSet();
    }

    public void TestUseSkill()
    {
        UseSkill();
    }

    public void TestPosion()
    {
        AddStatusEffect("KeyStatusPoison", 0, 2);
    }
    public void TestBurn()
    {
        AddStatusEffect("KeyStatusBurn", 0, 2);
    }
    public void TestPride()
    {
        AddStatusEffect("KeyStatusPride", 2, 0);
    }

    public void TestVulnerable()
    {
        AddStatusEffect("KeyStatusVulnerable", 2, 0);
    }


    private void PlayMonsterSound(MonsterSoundType type)
    {
        if(_soundTable.TryGetValue(type, out AudioClip clip))
        {
            SoundManager.Instance.PlaySFX(clip);
        }
    }

    private IEnumerator ProcessEnemyAction()
    {
        UseSkill();

        yield return null; 

        Animator anim = GetComponentInChildren<Animator>();
        float waitTime = 3.0f; // 기본값 (혹시 못 찾을 경우 대비)

        if (anim != null)
        {
            if(anim.IsInTransition(0))
                waitTime = anim.GetNextAnimatorStateInfo(0).length;
            else
                waitTime = anim.GetCurrentAnimatorStateInfo(0).length;
        }
        yield return new WaitForSeconds(waitTime + 0.5f);

        OnEnemyActTurnEnd?.Invoke();
    }

































    // 옵저버 패턴 관련 메서드 ------------------------------------------------------------
    public void AddHpObserver(IMonsterHpObserver observer)
    {
        if (!_hpObservers.Contains(observer)) //방어 코드
        {
            _hpObservers.Add(observer);
        }
    }

    public void RemoveHpObserver(IMonsterHpObserver observer)
    {
        if (_hpObservers.Contains(observer)) //방어 코드
        {
            _hpObservers.Remove(observer);
        }
    }

    private void NotifyHpObservers() //옵저버들에게 HP 변경 알림
    {
        foreach (var observer in _hpObservers)
        {
            observer.OnMonsterHpChanged(_monsterCurHP, _monsterMaxHP, _monsterShield);
        }
    }

    public void AddSkillObserver(IMonsterSkillObserver observer)
    {
        if (!_skillObservers.Contains(observer)) //방어 코드
        {
            _skillObservers.Add(observer);
        }
    }
    public void RemoveSkillObserver(IMonsterSkillObserver observer)
    {
        if (_skillObservers.Contains(observer)) //방어 코드
        {
            _skillObservers.Remove(observer);
        }
    }
    private void NotifySkillObservers() //옵저버들에게 스킬 변경 알림
    {
        foreach (var observer in _skillObservers)
        {
            observer.OnMonsterSkillSelected(_currentSkillData, _selectedSkillValue);
        }
    }

    public void AddEffectObserver(IMonsterEffectObserver observer)
    {
        if(!_effectObservers.Contains(observer)) _effectObservers.Add(observer);
    }

    public void RemoveEffectObserver(IMonsterEffectObserver observer)
    {
        if(!_effectObservers.Contains(observer)) _effectObservers.Remove(observer);
    }

    private void NotifyEffectObservers()
    {
        foreach (var observer in _effectObservers)
        {
            observer.OnMonsterEffectChanged(_statusEffects);
        }
    }

    private void OnDestroy() {
        //혹시 모를 GC를 위해 초기화
        Debug.Log("monster die ? " + _isDead);
        OnEnemyActTurnEnd = null;
        OnMonsterDeath = null;
        _hpObservers.Clear();
        _skillObservers.Clear();
        _effectObservers.Clear();
    }

}
