using System.Collections.Generic;
using UnityEngine;

public class ScenarioPlayer : MonoBehaviour
{
    public HashSet<Trigger_Type> _playedScenarios = new();
    public bool IsPlaying { get; private set; }
    Dictionary<Trigger_Type, List<ScenarioData>> _scenarioMap;
    bool _initialized;
    private void Awake()
    {
        _scenarioMap = new Dictionary<Trigger_Type, List<ScenarioData>>();        
    }
    private void EnsureInit()
    {
        if (_initialized) return;

        // DataManager 준비 보장
        if (DataManager.Instance == null) return;
        if (!DataManager.Instance.IsReady) return;

        FillScenarioDatas();
        _initialized = true;
    }

    public bool IsScenarioPlayed(Trigger_Type trigger)
    {
        if (trigger == Trigger_Type.gamestart)
            return Player.Instance.ScenarioPlayIndex > 0;

        return _playedScenarios.Contains(trigger);
    }

    public bool RequestScenario(Trigger_Type trigger)
    {
        if (IsPlaying) return false;

        EnsureInit();

        if (!_scenarioMap.TryGetValue(trigger, out var list)) return false;
        if (list == null || list.Count == 0) return false;

        IsPlaying = true;

        Player.Instance.SetTalkUI();
        Player.Instance.TalkUI.EnterScenario(list);
        Player.Instance.TalkUI.OnScenarioEnd += OnScenarioEnd;

        return true;
    }

    private void OnScenarioEnd()
    {
        IsPlaying = false;

        // 여기서만 재생 기록
        _playedScenarios.Add(Player.Instance.CurrentPlayedScenario);

        Player.Instance.TalkUI.OnScenarioEnd -= OnScenarioEnd;
        Player.Instance.OnScenarioFinished();
    }

    public void FillScenarioDatas()
    {
        _scenarioMap[Trigger_Type.gamestart] = Load("scn_Intro_");
        _scenarioMap[Trigger_Type.dungeonenter] = Load("scn_dun_envy_");
        _scenarioMap[Trigger_Type.stageenter] = Load("scn_dun_envy_st1_");
        _scenarioMap[Trigger_Type.prebattle] = Load("scn_boss_pride_");
        _scenarioMap[Trigger_Type.preseal] = Load("scn_boss_seal_");
        _scenarioMap[Trigger_Type.clear] = Load("scn_pride_clear_");
    }
    List<ScenarioData> Load(string key)
    {
        List<ScenarioData> list = new();
        for (int i = 1; i < int.MaxValue; i++)
        {
            ScenarioData data = DataManager.Instance.GetScenario($"{key}{i:D3}");
            if (data == null) break;
            list.Add(data);
        }
        return list;
    }    
}