using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;

/// <summary>
/// 외부와 상호작용할 '플레이어 캐릭터'는 이거 하나. 
/// </summary>
public class Player : MonoBehaviour, IBattleUnit, ITalkable //나중에 싱글톤도 해주기
{
    public static Player Instance { get; private set; }
    public bool IsInitialized { get; private set; } = false;
    //스탯 관련 필드    
    PlayerStat _stat;
    public Action<int> OnMoneyChanged; //돈 수치 변화할 때마다 호출.
    public Action<int, int, int, int> OnHpChanged; //체력 수치 변화할 때마다 호출.    
    public Action<int> OnShieldChanged; //보호막 수치 변화할 때마다 호출.
    public Action<int, int> OnExpChanged; //경험치 획득할 때마다 호출.
    public Action<int> OnLevelChanged;
    public Action<int> OnDefenseChanged; //방어력 변화할 때마다 호출.
    public Action<int> OnLevelUp;//레벨업할 때마다 호출
    public Action<Dictionary<StatusEffectData, int>> OnStatusEffectChanged; //상태이상 변화할 때마다 호출

    //마을 관련 필드
    public Village Currentvillage { get; private set; }
    public Npc TalkingNpc { get; private set; }
    [SerializeField] TalkUI _talkUIPrefab;
    public TalkUI TalkUI { get; set; }
    public bool CanInteract { get; set; }
    public bool IsTalking { get; set; }

    Action _afterScenarioAction;
    bool _scenarioRequestInProgress = false;


    public Trigger_Type CurrentPlayedScenario { get; private set; }

    // [추가] 진행도 저장용 변수
    public int DungeonClearedIndex { get; set; } = -1;
    

    //Player에 붙은 다른 컴포넌트들
    private PlayerUIManager _playerUIManager;
    private PlayerModelController _playerModelController;
    private PlayerInputHandler _playerInputHandler;
    private ScenarioPlayer _scenarioPlayer;
    private Animator _animator;

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
        _playerUIManager = GetComponent<PlayerUIManager>();
        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _scenarioPlayer = GetComponent<ScenarioPlayer>();
        _animator = GetComponent<Animator>();
        _playerModelController = GetComponent<PlayerModelController>();
    }

    private void Start()
    {

    }



    ///게임 매니저에서 호출할 함수
    //연결된 UI 초기값도 여기서 할당
    public void Init(PlayerStat stat)
    {
        _stat = stat;
        GetHp(_stat.Hp);
        PushHp();
        PushMoney();
        PushExp();
        PushLevel();
        PushDefense();
        PushShield();
        PushStatusEffects();
        Debug.Log($"{_stat.Name}");
        Debug.Log($"남은 경험치 : {_stat.NeedExp}");
        Debug.Log($"체력 : {_stat.Hp}");
        Debug.Log($"현재 체력 : {_stat.CurrentHp}");
        Debug.Log($"방어력 : {_stat.Defense}");
        Debug.Log($"이동속도 : {_stat.MoveSpeed}");
        Debug.Log($"보유 재화 : {_stat.Money}");
        IsInitialized = true;
        Respawn();
    }

    // [추가] GameSaveManager와 데이터 연동

    public void LoadFromSaveData(GlobalSaveData data)
    {
        if (_stat == null) return;

        _stat.Level = data.Level;
        _stat.Money = data.Money;
        _stat.CurrentExp = data.CurrentExp;
        
        DungeonClearedIndex = data.DungeonClearedIndex;
        

        PropertyInfo expProp = typeof(PlayerStat).GetProperty("CurrentExp");
        if (expProp != null)
        {
            expProp.SetValue(_stat, data.CurrentExp);
        }
        _stat.SetStats();        // 레벨 기준 스탯 재계산
        _stat.GetHp(_stat.Hp);   // 최대체력 기준으로 회복
        if (data.PlayerPosition != Vector3.zero)
        {
            if (data.PlayerPosition.x > 6.5f)
            {
                transform.position = new Vector2(6.5f, -2.5f);
            }
            else
            {
                transform.position = data.PlayerPosition;
            }                
        }

        PushAllUI();
    }

    // UI 전체 갱신 헬퍼
    private void PushAllUI()
    {
        PushHp();
        PushMoney();
        PushExp();
        PushLevel();
        PushDefense();
        PushShield();
        PushStatusEffects();
    }

    //------------------------------------------------------

    #region UI 구독 직후 활성화용 푸쉬 함수

    public void PushHp()
    {
        if (_stat != null)
        {
            OnHpChanged?.Invoke(_stat.CurrentHp, _stat.Hp, _stat.Poison, _stat.Burn);
        }
    }
    public void PushExp()
    {
        if (_stat != null)
        {
            OnExpChanged?.Invoke(_stat.CurrentExp, _stat.NeedExp);
        }
    }
    public void PushDefense()
    {
        if (_stat != null)
        {
            OnDefenseChanged?.Invoke(_stat.Defense);
        }
    }
    public void PushShield()
    {
        if (_stat != null)
        {
            OnShieldChanged?.Invoke(_stat.Shield);
        }
    }

    public void PushLevel()
    {
        if (_stat != null)
        {
            OnLevelChanged?.Invoke(_stat.Level);
        }
    }

    public void PushStatusEffects()
    {
        if (_stat != null)
        {
            OnStatusEffectChanged?.Invoke(_stat.StatusEffects);
        }
    }

    public void PushMoney()
    {
        if (_stat != null)
        {
            OnMoneyChanged?.Invoke(_stat.Money);
        }
    }

    #endregion

    //------------------------------------------------------

    #region 외부에서 스탯 접근용 함수

    public int GetLevel() => _stat != null ? _stat.Level : 1;
    public int GetMoney() => _stat != null ? _stat.Money : 0;
    public int GetCurrentExp() => _stat != null ? _stat.CurrentExp : 0;

    //public int GetLevel()
    //{
    //    return _stat.Level;
    //}
    //public int GetMoney()
    //{
    //    return _stat.Money;
    //}
    public int GetCurrentHp()
    {
        return _stat.CurrentHp;
    }
    public int GetFullHp()
    {
        return _stat.Hp;
    }



    public float GetBuff()
    {
        return _stat.Buff;
    }


    public float GetMoveSpeed()
    {
        return _stat.MoveSpeed;
    }
    public string GetName()
    {
        return _stat.Name;
    }

    public string GetTalk()
    {
        return "Player Talking";
    }

    public string GetImage()
    {
        return "Img";
    }



    #endregion

    //------------------------------------------------------

    #region 전투용 스탯 관리 함수
    /// 이하 함수들은 전투 중 외부에서 호출.      
    public void TakeDamage(int damage) //공격 데미지를 입을 때마다 호출.
    {
        HitMotion();
        _stat.GetDamage(damage);
        OnShieldChanged?.Invoke(_stat.Shield);
        OnHpChanged?.Invoke(_stat.CurrentHp, _stat.Hp, _stat.Poison, _stat.Burn);
        OnStatusEffectChanged?.Invoke(_stat.StatusEffects);

    }

    public void GetHp(int hp) //체력을 회복할 때마다 호출
    {
        _stat.GetHp(hp);
        OnHpChanged?.Invoke(_stat.CurrentHp, _stat.Hp, _stat.Poison, _stat.Burn);
    }

    public void GetShield(int shield) //보호막을 얻을 때마다 호출
    {
        _stat.GetShield(shield);
        OnShieldChanged?.Invoke(_stat.Shield);
    }

    public void ResetShield() //보호막이 사라질 때(매 턴 시작 전 등)마다 호출
    {
        _stat.ResetShield();
        OnShieldChanged?.Invoke(_stat.Shield);
    }

    public void ResetCondition()
    {
        _stat.ResetCondition();
        OnHpChanged?.Invoke(_stat.CurrentHp, _stat.Hp, _stat.Poison, _stat.Burn);
        OnShieldChanged?.Invoke(_stat.Shield);
        OnStatusEffectChanged?.Invoke(_stat.StatusEffects);
    }


    public void GetExp(int exp)
    {
        _stat.GetExp(exp);
        OnHpChanged?.Invoke(_stat.CurrentHp, _stat.Hp, _stat.Poison, _stat.Burn);
        OnExpChanged?.Invoke(_stat.CurrentExp, _stat.NeedExp);
    }

    public void AddStatusEffect(string effectKey, int duration, int stack)
    {
        _stat.AddStatusEffect(effectKey, duration, stack);
        OnHpChanged?.Invoke(_stat.CurrentHp, _stat.Hp, _stat.Poison, _stat.Burn);
        OnStatusEffectChanged?.Invoke(_stat.StatusEffects);
        Debug.Log($"현재 걸린 상태 \n 공격 배율: {_stat.Buff}, 취약 배율 : {_stat.DeBuff}, 독: {_stat.Poison}, 화상 : {_stat.Burn}");
    }

    public void ApplyStatusEffect()
    {
        Debug.Log("플레이어 상태이상 대미지 적용");
        _stat.ApplyStatusEffect();
        Debug.Log($"현재 독 : {_stat.Poison}, 화상 : {_stat.Burn}");
        OnHpChanged?.Invoke(_stat.CurrentHp, _stat.Hp, _stat.Poison, _stat.Burn);
        OnStatusEffectChanged?.Invoke(_stat.StatusEffects);
    }

    public void AddPoison(int stack)
    {
        AddStatusEffect("KeyStatusPoison", 0, stack);
        Debug.Log($"현재 독 : {_stat.Poison}, 화상 : {_stat.Burn}");
    }

    public void AddBurn(int stack)
    {
        AddStatusEffect("KeyStatusBurn", 0, stack);
        Debug.Log($"현재 독 : {_stat.Poison}, 화상 : {_stat.Burn}");
    }

    #endregion

    //------------------------------------------------------
    public void AttackMotion()
    {
        _animator.SetTrigger("Attack");
    }

    public void HitMotion()
    {
        _animator.SetTrigger("Hit");
    }

    public void SetDeadMotion(bool die)
    {
        _animator.SetBool("IsDead", die);
    }

    //------------------------------------------------------
    public void Respawn()
    {
        SetDeadMotion(false);
        if (_stat != null)
        {
            GetHp(_stat.Hp);
            ResetCondition();
            SetStatUIView(true);
        }        

        if (Currentvillage != null)
        {
            gameObject.transform.position = Currentvillage.SpawnZone;
        }
        else
        {
            gameObject.transform.position = new Vector2(-1.5f, 2);
        }

    }

    public void SetVillage(Village village)
    {
        Currentvillage = village;
    }

    public void SetTalkingNpc(Npc npc)
    {
        TalkingNpc = npc;
    }


    public void SetTalkUI()
    {
        TalkUI = Instantiate(_talkUIPrefab, transform);
    }

    public void TalkMyself()
    {
        EnterTalkMod();
        TalkUI.EnterTalk(this);
    }

    public void TalkWithNpc()
    {
        if (CanInteract)
        {
            SetTalkUI();
            EnterTalkMod();
            Debug.Log($"{TalkingNpc.Name}와 상호작용!");
            if (TalkUI == null)
            {
                Debug.LogWarning("TalkUI Null");
            }
            if (TalkingNpc == null)
            {
                Debug.LogWarning("TalkingNpc Null");
            }
            TalkUI.EnterTalk(TalkingNpc);
        }
    }

    public void EndTalk()
    {
        TalkUI?.EndTalk();
        EnterMoveMod();
        // [추가] 대화 종료 후 저장 (진행도/퀘스트/스크립트 저장용)
        GameSaveManager.Instance.SaveGame();
        TalkUI = null;
    }

    public void PlusMoney(int cost)
    {
        _stat.PlusMoney(cost);
        OnMoneyChanged?.Invoke(_stat.Money);
    }

    public void MinusMoney(int cost)
    {
        _stat.MinusMoney(cost);
        OnMoneyChanged?.Invoke(_stat.Money);
    }

    public void UpdateScenario()
    {
        if (TalkUI == null)
            return;
        TalkUI.UpdateScenario();
    }
    public void ResetScenarioPlayed(Trigger_Type type)
    {
        if (_scenarioPlayer != null)
        {
            _scenarioPlayer.PlayedScenarios.Remove(type);
        }
    }


    //----------------------------------------------------------
    // UI 온오프 관련 함수들

    public void SwitchIsTalking(bool talking)
    {
        IsTalking = talking;
        if (!talking)
        {
            //EnterMoveMod();
            // [추가] 대화 종료 후 저장 (진행도/퀘스트/스크립트 저장용)
            GameSaveManager.Instance.SaveGame();
            TalkUI = null;
        }

        Currentvillage?.VillageManager?.OnOffTalkSlide(!talking);
        Currentvillage?.VillageManager?.OnOffVillageName(!talking);
        SetStatUIView(!talking);
        SetInventoryUIView(!talking);
    }

    private void SetStatUIView(bool readyToShow)
    {
        _playerUIManager.OnOffPlayerStatUi(readyToShow);
    }
    private void SetInventoryUIView(bool readyToShow)
    {
        _playerUIManager.OnOffPlayerInventoryUi(readyToShow);
    }


    //----------------------------------------------------------
    //입력 상태 전환 요청 함수들
    public void EnterBattleMod()
    {
        _playerInputHandler.ChangeInputState(new BattleState(this, _playerInputHandler));
    }
    public void EnterScenarioMod()
    {
        _playerInputHandler.ChangeInputState(new ScenarioState(this, _playerInputHandler));
    }
    public void EnterTalkMod()
    {
        _playerInputHandler.ChangeInputState(new TalkState(this, _playerInputHandler));
    }

    public void EnterMoveMod()
    {
        _playerInputHandler.ChangeInputState(new MoveState(this, _playerInputHandler));
    }
    //----------------------------------------------------------

    private class PlayerSaveData
    {
        public int Level;
        public int Money;
        public int Exp;
        public int ScenarioPlayIndex;
    }   

    public void EnterDungeonSelect()
    {
        _playerUIManager.OnOffPlayerInventoryUi(false);
        _playerUIManager.OnOffPlayerStatUi(false);
        EnterBattleMod();
    }

    public void BackToVillage()
    {
        _playerUIManager.OnOffPlayerInventoryUi(true);
        _playerUIManager.OnOffPlayerStatUi(true);
        _playerModelController.ResetModel();
        EnterMoveMod();
    }

    public void EnterBattle()
    {
        gameObject.transform.position = new Vector2(-3f, -1.5f);
        _playerModelController.ResetModel();
        _playerUIManager.OnOffPlayerInventoryUi(true);
        _playerUIManager.OnOffPlayerStatUi(true);
    }

    public void EnterReward()
    {
        _playerUIManager.OnOffPlayerInventoryUi(false);
        _playerUIManager.OnOffPlayerStatUi(false);
    }

    public void OnScenarioFinished()
    {
        SwitchIsTalking(false);

        
        _afterScenarioAction?.Invoke();
        _afterScenarioAction = null;

        _scenarioRequestInProgress = false;

        ResolveInputState();
    }
    public void ResolveInputState()
    {
        // 1. 시나리오가 재생 중이면
        if (_scenarioPlayer != null && _scenarioPlayer.IsPlaying)
        {
            EnterScenarioMod();
            return;
        }

        // 2. 대화 중이면
        if (IsTalking)
        {
            EnterTalkMod();
            return;
        }

        // 3. 그 외는 이동
        //EnterMoveMod();
    }
    public void EnsureTalkUI()
    {
        if (TalkUI == null)
            TalkUI = Instantiate(_talkUIPrefab, transform);
    }
    public void PlayScenarioGuaranteed(Trigger_Type type,Action afterScenario = null)
    {
        StartCoroutine(PlayScenarioGuaranteedRoutine(type, afterScenario));
    }

    private IEnumerator PlayScenarioGuaranteedRoutine(
     Trigger_Type type,
     Action afterScenario
 )
    {
        // 이미 요청 중이면 무시
        if (_scenarioRequestInProgress)
            yield break;

        _scenarioRequestInProgress = true;

        _afterScenarioAction = afterScenario;
        CurrentPlayedScenario = type;

        // 이미 재생된 경우
        if (_scenarioPlayer.IsScenarioPlayed(type))
        {
            Debug.Log($"이미 본 {type} 타입의 시나리오. 건너뜁니다.");
            _afterScenarioAction?.Invoke();
            _afterScenarioAction = null;
            _scenarioRequestInProgress = false;
            yield break;
        }

        // DataManager 준비 대기
        while (DataManager.Instance == null || !DataManager.Instance.IsReady)
            yield return null;

        EnterScenarioMod();

        // 실제 시작될 때까지 대기
        while (!_scenarioPlayer.RequestScenario(type))
            yield return null;
    }

    public void ClickSettingButton()
    {
        SoundManager.Instance.SetActivePanel();
    }
}
