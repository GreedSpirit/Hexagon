using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum SpriteType
{
    Character,
    Card,
    Monster,
    Status,
    Inventory,

}

public interface TableKey
{
    int Id { get; }
    string Key { get; }
}



public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public bool IsReady { get; private set; }

    [Header("Sprite Databases")]
    public SpriteDatabase characterDB;
    public SpriteDatabase cardDB;
    public SpriteDatabase monsterDB;
    public SpriteDatabase statusDB;
    public SpriteDatabase inventoryDB;

    // 내부 관리용 딕셔너리
    private Dictionary<SpriteType, SpriteDatabase> _dbMap = new Dictionary<SpriteType, SpriteDatabase>();

    // (ID: int 기반)
    public Dictionary<int, CharacterData> CharacterDict { get; private set; }
    public Dictionary<int, CardData> CardDict { get; private set; }
    public Dictionary<int, MonsterData> MonsterStatDict { get; private set; }
    public Dictionary<int, MonsterSkillSetData> MonsterSkillSetDict { get; private set; }
    public Dictionary<int, MonsterStatData> CommonMonsterStatDataDict { get; private set; }
    public Dictionary<int, MonsterStatData> BossMonsterStatDataDict { get; private set; }
    public Dictionary<int, CharacterLevelData> CharacterLevelDict { get; private set; }
    public Dictionary<int, CharacterStatData> CharacterStatDict { get; private set; }
    public Dictionary<int, CardNumberOfAvailableData> CommonCardNoADict { get; private set; }
    public Dictionary<int, CardNumberOfAvailableData> RareCardNoADict { get; private set; }
    public Dictionary<int, CardNumberOfAvailableData> EpicCardNoADict { get; private set; }
    public Dictionary<int, CardNumberOfAvailableData> LegendaryCardNoADict { get; private set; }
    public Dictionary<int, UpgradeData> CommonCardUpgradeDict { get; private set; }
    public Dictionary<int, UpgradeData> RareCardUpgradeDict { get; private set; }
    public Dictionary<int, UpgradeData> EpicCardUpgradeDict { get; private set; }
    public Dictionary<int, UpgradeData> LegendaryCardUpgradeDict { get; private set; }
    public Dictionary<int, StatusEffectData> StatusEffectDict { get; private set; }

    public Dictionary<int, DungeonData> DungeonDict { get; private set; }
    public Dictionary<int, StageData> StageDict { get; private set; }
    public Dictionary<int, StringData> StringDict { get; private set; }
    public Dictionary<int, RewardData> RewardDict { get; private set; }
    public Dictionary<int, DeckData> DeckDict { get; private set; }
    public Dictionary<int, NpcData> NpcDict { get; private set; }
    public Dictionary<int, NpcTalkData> NpcTalkDict { get; private set; }
    public Dictionary<int, VillageData> VillageDict { get; private set; }
    public Dictionary<int, ScenarioData> ScenarioDict { get; private set; }

    // (Key: string 기반)
    public Dictionary<string, CharacterData> CharacterKeyDict { get; private set; }
    public Dictionary<string, CardData> CardKeyDict { get; private set; }
    public Dictionary<string, CardNumberOfAvailableData> CommonCardNoAKeyDict { get; private set; }
    public Dictionary<string, CardNumberOfAvailableData> RareCardNoAKeyDict { get; private set; }
    public Dictionary<string, CardNumberOfAvailableData> EpicCardNoAKeyDict { get; private set; }
    public Dictionary<string, CardNumberOfAvailableData> LegendaryCardNoAKeyDict { get; private set; }
    public Dictionary<string, UpgradeData> CommonCardUpgradeKeyDict { get; private set; }
    public Dictionary<string, UpgradeData> RareCardUpgradeKeyDict { get; private set; }
    public Dictionary<string, UpgradeData> EpicCardUpgradeKeyDict { get; private set; }
    public Dictionary<string, UpgradeData> LegendaryCardUpgradeKeyDict { get; private set; }
    public Dictionary<string, StatusEffectData> StatusEffectKeyDict { get; private set; }
    public Dictionary<string, MonsterData> MonsterStatKeyDict { get; private set; }
    public Dictionary<string, MonsterSkillSetData> MonsterSkillSetKeyDict { get; private set; }
    public Dictionary<string, MonsterStatData> CommonMonsterStatDataKeyDict { get; private set; }
    public Dictionary<string, MonsterStatData> BossMonsterStatDataKeyDict { get; private set; }
     public Dictionary<string, CharacterLevelData> CharacterLevelKeyDict { get; private set; }
     public Dictionary<string, CharacterStatData> CharacterStatKeyDict { get; private set; }
    public Dictionary<string, DungeonData> DungeonKeyDict { get; private set; }
    public Dictionary<string, StageData> StageKeyDict { get; private set; }
    public Dictionary<string, StringData> StringKeyDict { get; private set; }
    public Dictionary<string, RewardData> RewardKeyDict { get; private set; }
    public Dictionary<string, DeckData> DeckKeyDict { get; private set; }
    public Dictionary<string, NpcData> NpcKeyDict { get; private set; }
    public Dictionary<string, NpcTalkData> NpcTalkKeyDict { get; private set; }
    public Dictionary<string, VillageData> VillageKeyDict { get; private set; }
    public Dictionary<string, ScenarioData> ScenarioKeyDict { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitSpriteDB();
            LoadAllData();
            IsReady = true;
}
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitSpriteDB()
    {
        if (characterDB != null) _dbMap.Add(SpriteType.Character, characterDB);
        if (cardDB != null) _dbMap.Add(SpriteType.Card, cardDB);
        if (monsterDB != null) _dbMap.Add(SpriteType.Monster, monsterDB);
        if (statusDB != null) _dbMap.Add(SpriteType.Status, statusDB);
        if (inventoryDB != null) _dbMap.Add(SpriteType.Inventory, inventoryDB);
    }

    public Sprite GetSprite(SpriteType type, string key)
    {
        if (_dbMap.TryGetValue(type, out SpriteDatabase db))
        {
            return db.GetSprite(key);
        }
        Debug.LogWarning($"[DataManager] '{type}' 타입의 DB가 연결되지 않았습니다! (Key: {key})");
        return null;
    }

    public Sprite GetCharacterSprite(string key) => GetSprite(SpriteType.Character, key);
    public Sprite GetCardSprite(string key) => GetSprite(SpriteType.Card, key);
    public Sprite GetMonsterSprite(string key) => GetSprite(SpriteType.Monster, key);
    public Sprite GetStatusSprite(string key) => GetSprite(SpriteType.Status, key);
    public Sprite GetInventorySprite(string key) => GetSprite(SpriteType.Inventory, key);


    private void LoadAllData()
    {
        // 모든 테이블에 대해 ID와 Key 딕셔너리를 동시에 생성하여 로드합니다.
        CharacterDict = LoadAndCreateKeyDict(CSVReader.Read<CharacterData>("Character"), out Dictionary<string, CharacterData> tempCharKeyDict);
        CharacterKeyDict = tempCharKeyDict;
        CharacterLevelDict = LoadAndCreateKeyDict(CSVReader.Read<CharacterLevelData>("CharacterLevel"), out Dictionary<string, CharacterLevelData> tempCharLevelKeyDict);
        CharacterLevelKeyDict = tempCharLevelKeyDict;
        CharacterStatDict = LoadAndCreateKeyDict(CSVReader.Read<CharacterStatData>("CharacterStat"), out Dictionary<string, CharacterStatData> tempCharStatKeyDict);
        CharacterStatKeyDict = tempCharStatKeyDict;

        // [Card]
        CardDict = LoadAndCreateKeyDict(CSVReader.Read<CardData>("Card"), out Dictionary<string, CardData> tempCardKeyDict);
        CardKeyDict = tempCardKeyDict;
        CommonCardNoADict = LoadAndCreateKeyDict(CSVReader.Read<CardNumberOfAvailableData>("CommonCardNoA"), out Dictionary<string, CardNumberOfAvailableData> tempCommonCardNoaKeyDict);
        CommonCardNoAKeyDict = tempCommonCardNoaKeyDict;
        RareCardNoADict = LoadAndCreateKeyDict(CSVReader.Read<CardNumberOfAvailableData>("RareCardNoA"), out Dictionary<string, CardNumberOfAvailableData> tempRareCardNoaKeyDict);
        RareCardNoAKeyDict = tempRareCardNoaKeyDict;
        EpicCardNoADict = LoadAndCreateKeyDict(CSVReader.Read<CardNumberOfAvailableData>("EpicCardNoA"), out Dictionary<string, CardNumberOfAvailableData> tempEpicCardNoaKeyDict);
        EpicCardNoAKeyDict = tempEpicCardNoaKeyDict;
        LegendaryCardNoADict = LoadAndCreateKeyDict(CSVReader.Read<CardNumberOfAvailableData>("LegendaryCardNoA"), out Dictionary<string, CardNumberOfAvailableData> tempLegandaryCardNoaKeyDict);
        LegendaryCardNoAKeyDict = tempLegandaryCardNoaKeyDict;
        CommonCardUpgradeDict = LoadAndCreateKeyDict(CSVReader.Read<UpgradeData>("Common"), out Dictionary<string, UpgradeData> tempCommonKeyDict);
        CommonCardUpgradeKeyDict = tempCommonKeyDict;
        RareCardUpgradeDict = LoadAndCreateKeyDict(CSVReader.Read<UpgradeData>("Rare"), out Dictionary<string, UpgradeData> tempRareKeyDict);
        RareCardUpgradeKeyDict = tempRareKeyDict;
        EpicCardUpgradeDict = LoadAndCreateKeyDict(CSVReader.Read<UpgradeData>("Epic"), out Dictionary<string, UpgradeData> tempEpicKeyDict);
        EpicCardUpgradeKeyDict = tempEpicKeyDict;
        LegendaryCardUpgradeDict = LoadAndCreateKeyDict(CSVReader.Read<UpgradeData>("Legendary"), out Dictionary<string, UpgradeData> tempLegendaryKeyDict);
        LegendaryCardUpgradeKeyDict = tempLegendaryKeyDict;

        // [Status]
        StatusEffectDict = LoadAndCreateKeyDict(CSVReader.Read<StatusEffectData>("StatusEffect"), out Dictionary<string, StatusEffectData> tempStatusKeyDict);
        StatusEffectKeyDict = tempStatusKeyDict;

        // [Monster]
        MonsterStatDict = LoadAndCreateKeyDict(CSVReader.Read<MonsterData>("Monster"), out Dictionary<string, MonsterData> tempMonsterKeyDict);
        MonsterStatKeyDict = tempMonsterKeyDict;
        MonsterSkillSetDict = LoadAndCreateKeyDict(CSVReader.Read<MonsterSkillSetData>("SkillSet"), out Dictionary<string, MonsterSkillSetData> tempMonsterSkillSetKeyDict);
        MonsterSkillSetKeyDict = tempMonsterSkillSetKeyDict;
        CommonMonsterStatDataDict = LoadAndCreateKeyDict(CSVReader.Read<MonsterStatData>("CommonMonsterStat"), out Dictionary<string, MonsterStatData> tempCommonMonsterStatKeyDict);
        CommonMonsterStatDataKeyDict = tempCommonMonsterStatKeyDict;
        BossMonsterStatDataDict = LoadAndCreateKeyDict(CSVReader.Read<MonsterStatData>("BossMonsterStat"), out Dictionary<string, MonsterStatData> tempBossMonsterStatKeyDict);
        BossMonsterStatDataKeyDict = tempBossMonsterStatKeyDict;

        // [Skill]
        // SkillDict = LoadAndCreateKeyDict(CSVReader.Read<SkillData>("Skill"), out Dictionary<string, SkillData> tempSkillKeyDict);
        // SkillKeyDict = tempSkillKeyDict;

        // [Dungeon]
        DungeonDict = LoadAndCreateKeyDict(CSVReader.Read<DungeonData>("Dungeon"), out Dictionary<string, DungeonData> tempDungeonKeyDict);
        DungeonKeyDict = tempDungeonKeyDict;

        // [Deck]
        DeckDict = LoadAndCreateKeyDict(CSVReader.Read<DeckData>("Deck"), out Dictionary<string, DeckData> tempDeckKeyDict);
        DeckKeyDict = tempDeckKeyDict;

        // [Stage]
        StageDict = LoadAndCreateKeyDict(CSVReader.Read<StageData>("Stage"), out Dictionary<string, StageData> tempStageKeyDict);
        StageKeyDict = tempStageKeyDict;

        // [String]
        StringDict = LoadAndCreateKeyDict(CSVReader.Read<StringData>("String"), out Dictionary<string, StringData> tempStringKeyDict);
        StringKeyDict = tempStringKeyDict;

        // [Reward]
        RewardDict = LoadAndCreateKeyDict(CSVReader.Read<RewardData>("Reward"), out Dictionary<string, RewardData> tempRewardKeyDict);
        RewardKeyDict = tempRewardKeyDict;

        // [Npc]
        NpcDict = LoadAndCreateKeyDict(CSVReader.Read<NpcData>("Npc"), out Dictionary<string, NpcData> tempNpcKeyDict);
        NpcKeyDict = tempNpcKeyDict;

        NpcTalkDict = LoadAndCreateKeyDict(CSVReader.Read<NpcTalkData>("NpcTalk"), out Dictionary<string, NpcTalkData> tempNpcTalkKeyDict);
        NpcTalkKeyDict = tempNpcTalkKeyDict;

        // [Village]
        VillageDict = LoadAndCreateKeyDict(CSVReader.Read<VillageData>("Village"), out Dictionary<string, VillageData> tempVillageKeyDict);
        VillageKeyDict = tempVillageKeyDict;

        // [Scenario]
        ScenarioDict = LoadAndCreateKeyDict(CSVReader.Read<ScenarioData>("Scenario"), out Dictionary<string, ScenarioData> tempScenarioKeyDict);
        ScenarioKeyDict = tempScenarioKeyDict;
        // 테스트 로그
        //Debug.Log($"데이터 로드 완료. Character 개수: {CharacterDict.Count}");
    }
    private Dictionary<int, T> LoadAndCreateKeyDict<T>(List<T> list, out Dictionary<string, T> keyDict) where T : CSVLoad, TableKey
    {
        Dictionary<int, T> idDict = new Dictionary<int, T>();
        keyDict = new Dictionary<string, T>();

        foreach (T data in list)
        {

            // ID 딕셔너리에 추가
            if (!idDict.ContainsKey(data.Id))
            {
                idDict.Add(data.Id, data);
            }
            else
            {
                Debug.LogWarning($"ID 중복 발생 (무시됨): {typeof(T).Name} 테이블 - ID {data.Id}");
            }

            // Key 딕셔너리에 추가
            if (!string.IsNullOrEmpty(data.Key) && !keyDict.ContainsKey(data.Key))
            {
                keyDict.Add(data.Key, data);
            }
            else if (!string.IsNullOrEmpty(data.Key))
            {
                Debug.LogWarning($"Key 중복 발생 (무시됨): {typeof(T).Name} 테이블 - Key {data.Key}");
            }
        }
        return idDict;
    }

    // 데이터 접근 함수

    public CharacterData GetCharacter(int id) => CharacterDict.TryGetValue(id, out var data) ? data : null;
    public CharacterData GetCharacter(string key) => CharacterKeyDict.TryGetValue(key, out var data) ? data : null;

    public CardData GetCard(int id) => CardDict.TryGetValue(id, out var data) ? data : null;
    public CardData GetCard(string key) => CardKeyDict.TryGetValue(key, out var data) ? data : null;

    public MonsterData GetMonsterStatData(int id) => MonsterStatDict.TryGetValue(id, out var data) ? data : null;
    public MonsterData GetMonsterStatData(string key) => MonsterStatKeyDict.TryGetValue(key, out var data) ? data : null;

    public MonsterSkillSetData GetMonsterSkillSetData(int id) => MonsterSkillSetDict.TryGetValue(id, out var data) ? data : null;
    public MonsterSkillSetData GetMonsterSkillSetData(string key) => MonsterSkillSetKeyDict.TryGetValue(key, out var data) ? data : null;

    public MonsterStatData GetCommonMonsterStatData(int id) => CommonMonsterStatDataDict.TryGetValue(id, out var data) ? data : null;
    public MonsterStatData GetCommonMonsterStatData(string key) => CommonMonsterStatDataKeyDict.TryGetValue(key, out var data) ? data : null;

    public MonsterStatData GetBossMonsterStatData(int id) => BossMonsterStatDataDict.TryGetValue(id, out var data) ? data : null;
    public MonsterStatData GetBossMonsterStatData(string key) => BossMonsterStatDataKeyDict.TryGetValue(key, out var data) ? data : null;
    
    public CharacterLevelData GetCharacterLevel(int id) => CharacterLevelDict.TryGetValue(id, out var data) ? data : null;
    public CharacterLevelData GetCharacterLevel(string key) => CharacterLevelKeyDict.TryGetValue(key, out var data) ? data : null;

    public CharacterStatData GetCharacterStat(int id) => CharacterStatDict.TryGetValue(id, out var data) ? data : null;
    public CharacterStatData GetCharacterStat(string key) => CharacterStatKeyDict.TryGetValue(key, out var data) ? data : null;

    public CardNumberOfAvailableData GetCommonCardNoAData(int id) => CommonCardNoADict.TryGetValue(id, out var data) ? data : null;
    public CardNumberOfAvailableData GetCommonCardNoAData(string key) => CommonCardNoAKeyDict.TryGetValue(key, out var data) ? data : null;

    public CardNumberOfAvailableData GetRareCardNoAData(int id) => RareCardNoADict.TryGetValue(id, out var data) ? data : null;
    public CardNumberOfAvailableData GetRareCardNoAData(string key) => RareCardNoAKeyDict.TryGetValue(key, out var data) ? data : null;

    public CardNumberOfAvailableData GetEpicCardNoAData(int id) => EpicCardNoADict.TryGetValue(id, out var data) ? data : null;
    public CardNumberOfAvailableData GetEpicCardNoAData(string key) => EpicCardNoAKeyDict.TryGetValue(key, out var data) ? data : null;

    public CardNumberOfAvailableData GetLegendaryCardNoAData(int id) => LegendaryCardNoADict.TryGetValue(id, out var data) ? data : null;
    public CardNumberOfAvailableData GetLegendaryCardNoAData(string key) => LegendaryCardNoAKeyDict.TryGetValue(key, out var data) ? data : null;

    public UpgradeData GetCommonCardUpgradeData(int id) => CommonCardUpgradeDict.TryGetValue(id, out var data) ? data : null;
    public UpgradeData GetCommonCardUpgradeData(string key) => CommonCardUpgradeKeyDict.TryGetValue(key, out var data) ? data : null;

    public UpgradeData GetRareCardUpgradeData(int id) => RareCardUpgradeDict.TryGetValue(id, out var data) ? data : null;
    public UpgradeData GetRareCardUpgradeData(string key) => RareCardUpgradeKeyDict.TryGetValue(key, out var data) ? data : null;

    public UpgradeData GetEpicCardUpgradeData(int id) => EpicCardUpgradeDict.TryGetValue(id, out var data) ? data : null;
    public UpgradeData GetEpicCardUpgradeData(string key) => EpicCardUpgradeKeyDict.TryGetValue(key, out var data) ? data : null;

    public UpgradeData GetLegendaryCardUpgradeData(int id) => LegendaryCardUpgradeDict.TryGetValue(id, out var data) ? data : null;
    public UpgradeData GetLegendaryCardUpgradeData(string key) => LegendaryCardUpgradeKeyDict.TryGetValue(key, out var data) ? data : null;

    public StatusEffectData GetStatusEffectData(int id) => StatusEffectDict.TryGetValue(id, out var data) ? data : null;
    public StatusEffectData GetStatusEffectData(string key) => StatusEffectKeyDict.TryGetValue(key, out var data) ? data : null;


    //public SkillData GetSkill(int id) => SkillDict.TryGetValue(id, out var data) ? data : null;
    //public SkillData GetSkill(string key) => SkillKeyDict.TryGetValue(key, out var data) ? data : null;

    public DungeonData GetDungeon(int id) => DungeonDict.TryGetValue(id, out var data) ? data : null;
    public DungeonData GetDungeon(string key) => DungeonKeyDict.TryGetValue(key, out var data) ? data : null;

    public StageData GetStage(int id) => StageDict.TryGetValue(id, out var data) ? data : null;
    public StageData GetStage(string key) => StageKeyDict.TryGetValue(key, out var data) ? data : null;

    public StringData GetString(int id) => StringDict.TryGetValue(id, out var data) ? data : null;
    public StringData GetString(string key) => StringKeyDict.TryGetValue(key, out var data) ? data : null;
    public RewardData GetReward(int id) => RewardDict.TryGetValue(id, out var data) ? data : null;
    public RewardData GetReward(string key) => RewardKeyDict.TryGetValue(key, out var data) ? data : null;
    public DeckData GetDeck(int id) => DeckDict.TryGetValue(id, out var data) ? data : null;
    public DeckData GetDeck(string key) => DeckKeyDict.TryGetValue(key, out var data) ? data : null;

    public NpcData GetNpc(int id) => NpcDict.TryGetValue(id, out var data) ? data : null;
    public NpcData GetNpc(string key) => NpcKeyDict.TryGetValue(key, out var data) ? data : null;

    public NpcTalkData GetNpcTalk(int id) => NpcTalkDict.TryGetValue(id, out var data) ? data : null;
    public NpcTalkData GetNpcTalk(string key) => NpcTalkKeyDict.TryGetValue(key, out var data) ? data : null;

    public VillageData GetVillage(int id) => VillageDict.TryGetValue(id, out var data) ? data : null;
    public VillageData GetVillage(string key) => VillageKeyDict.TryGetValue(key, out var data) ? data : null;

    public ScenarioData GetScenario(int id) => ScenarioDict.TryGetValue(id, out var data) ? data : null;
    public ScenarioData GetScenario(string key) => ScenarioKeyDict.TryGetValue(key, out var data) ? data : null;
}