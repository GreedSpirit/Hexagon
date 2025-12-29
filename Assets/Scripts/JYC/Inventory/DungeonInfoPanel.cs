using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.InputSystem;

public class DungeonInfoPanel : MonoBehaviour
{
    private DungeonData _currentDungeon;

    [Header("Background Settings")]
    [SerializeField] private Image _panelBackground;
    [SerializeField] private Sprite _dungeonBgSprite;
    [SerializeField] private Sprite _enemyBgSprite;

    [Header("Buttons")]
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _toEnemyInfoButton;
    [SerializeField] private Button _toDungeonInfoButton;

    [Header("Content Groups")]
    [SerializeField] private GameObject _dungeonContentGroup;
    [SerializeField] private GameObject _enemyContentGroup;

    [Header("Dungeon Info UI")]
    [SerializeField] private TextMeshProUGUI _dungeonNameText;
    [SerializeField] private TextMeshProUGUI _stageCountText;
    [SerializeField] private TextMeshProUGUI _dungeonDescText;
    [SerializeField] private Image _dungeonImage;

    [Header("Enemy Info UI")]
    [SerializeField] private Transform _enemyListParent;
    [SerializeField] private GameObject _enemySlotPrefab;
    [SerializeField] private Image _selectedEnemyImage;
    [SerializeField] private TextMeshProUGUI _enemyNameText;
    [SerializeField] private TextMeshProUGUI _enemyStatText;
    [SerializeField] private TextMeshProUGUI _enemyDescText;

    private List<EnemySlotUI> _spawnedSlots = new List<EnemySlotUI>();

    private void Start()
    {
        if (_closeButton != null) _closeButton.onClick.AddListener(Close);
        if (_toEnemyInfoButton != null) _toEnemyInfoButton.onClick.AddListener(ShowEnemyMode);
        if (_toDungeonInfoButton != null) _toDungeonInfoButton.onClick.AddListener(ShowDungeonMode);
    }

    private void Update()
    {
        if (gameObject.activeSelf && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Close();
        }
    }

    public void Open(DungeonData dungeon)
    {
        _currentDungeon = dungeon;
        gameObject.SetActive(true);
        ShowDungeonMode(); // 켜질 때 기본 모드
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    // 던전 정보
    public void ShowDungeonMode()
    {
        if (_panelBackground != null && _dungeonBgSprite != null) _panelBackground.sprite = _dungeonBgSprite;
        if (_dungeonContentGroup != null) _dungeonContentGroup.SetActive(true);
        if (_enemyContentGroup != null) _enemyContentGroup.SetActive(false);
        if (_toEnemyInfoButton != null) _toEnemyInfoButton.gameObject.SetActive(true);
        if (_toDungeonInfoButton != null) _toDungeonInfoButton.gameObject.SetActive(false);

        UpdateDungeonInfo();
    }

    // 적 정보
    public void ShowEnemyMode()
    {
        if (_panelBackground != null && _enemyBgSprite != null) _panelBackground.sprite = _enemyBgSprite;
        if (_dungeonContentGroup != null) _dungeonContentGroup.SetActive(false);
        if (_enemyContentGroup != null) _enemyContentGroup.SetActive(true);
        if (_toEnemyInfoButton != null) _toEnemyInfoButton.gameObject.SetActive(false);
        if (_toDungeonInfoButton != null) _toDungeonInfoButton.gameObject.SetActive(true);

        UpdateEnemyInfo();
    }

    private void UpdateDungeonInfo()
    {
        if (_currentDungeon == null) return;

        // 텍스트 갱신
        _dungeonNameText.text = DataManager.Instance.GetString(_currentDungeon.Name).Korean;
        _stageCountText.text = $"<color=yellow>총 {_currentDungeon.NumberOfStages} 스테이지</color>";
        _dungeonDescText.text = DataManager.Instance.GetString(_currentDungeon.Desc).Korean;

        // 이미지 갱신 (InventoryDB 사용)
        if (_dungeonImage != null && !string.IsNullOrEmpty(_currentDungeon.Img))
        {
            Sprite spr = DataManager.Instance.GetInventorySprite(_currentDungeon.Img);
            if (spr != null)
            {
                _dungeonImage.sprite = spr;
                _dungeonImage.color = Color.white;
            }
            else
            {
                _dungeonImage.color = Color.clear;

                Debug.LogError($"[오류] 던전 이미지 '{_currentDungeon.Img}'를 InventoryDB에서 찾을 수 없습니다.");
            }
        }
    }

    private void UpdateEnemyInfo()
    {
        if (StageDataManager.Instance == null)
        {
            Debug.LogError("[오류] StageDataManager가 없습니다!");
            return;
        }

        // 기존 슬롯 삭제
        foreach (Transform child in _enemyListParent) Destroy(child.gameObject);
        _spawnedSlots.Clear();

        Debug.Log($"[디버그] 적 정보 로드 시작. 던전키: {_currentDungeon.DungeonKey}");

        // 스테이지 데이터 가져오기
        var stages = StageDataManager.Instance.GetStageByDungeonKey(_currentDungeon.DungeonKey);
        if (stages.Count == 0) Debug.LogError($"[오류] 해당 던전({_currentDungeon.DungeonKey})의 스테이지 정보가 없습니다. (Stage.csv 확인)");

        // 몬스터 키 수집
        HashSet<string> monsterKeys = new HashSet<string>();
        foreach (var stage in stages)
        {
            if (!string.IsNullOrEmpty(stage.SpawnMonster))
            {
                monsterKeys.Add(stage.SpawnMonster);
            }
        }

        if (monsterKeys.Count == 0) Debug.LogWarning("[주의] 스테이지에 몬스터가 하나도 배치되지 않았습니다.");

        bool firstSelected = false;
        foreach (string monKey in monsterKeys)
        {
            // 몬스터 데이터 로드
            MonsterData monData = DataManager.Instance.GetMonsterStatData(monKey);

            if (monData != null)
            {
                // 슬롯 생성
                GameObject go = Instantiate(_enemySlotPrefab, _enemyListParent);
                EnemySlotUI slot = go.GetComponent<EnemySlotUI>();
                slot.Init(monData, OnEnemySlotClicked);
                _spawnedSlots.Add(slot);

                //  이미지 체크
                if (DataManager.Instance.GetInventorySprite(monData.Img) == null)
                {
                    Debug.LogError($"[오류] 몬스터 '{monData.Name}'의 이미지 '{monData.Img}'가 InventoryDB에 없습니다.");
                }

                // 첫 번째 몬스터 자동 선택
                if (!firstSelected)
                {
                    OnEnemySlotClicked(monData);
                    firstSelected = true;
                }
            }
            else
            {
                Debug.LogError($"[오류] 몬스터 데이터를 찾을 수 없습니다. 키값: {monKey}");
            }
        }
    }

    private void OnEnemySlotClicked(MonsterData data)
    {
        // 선택 표시 갱신
        foreach (var slot in _spawnedSlots) slot.SetSelected(false);

        // 텍스트 갱신
        _enemyNameText.text = DataManager.Instance.GetString(data.Name).Korean;
        _enemyDescText.text = DataManager.Instance.GetString(data.Desc).Korean;

        // 상세 이미지 갱신
        Sprite monSprite = DataManager.Instance.GetInventorySprite(data.Img);
        if (monSprite != null)
        {
            _selectedEnemyImage.sprite = monSprite;
            _selectedEnemyImage.color = Color.white;
        }
        else
        {
            _selectedEnemyImage.color = Color.clear;
        }

        // 스탯 갱신
        MonsterStatData baseStat = (data.MonGrade == MonsterGrade.Boss) ?
            DataManager.Instance.GetBossMonsterStatData(1) : DataManager.Instance.GetCommonMonsterStatData(1);

        if (baseStat != null)
        {
            int hp = Mathf.FloorToInt(baseStat.Hp * data.HpRate);
            int def = Mathf.FloorToInt(baseStat.Defense * data.DefRate);
            _enemyStatText.text = $"HP : {hp}   DEF : {def}\n속도 : {data.MoveSpeed}";
        }
    }
}
