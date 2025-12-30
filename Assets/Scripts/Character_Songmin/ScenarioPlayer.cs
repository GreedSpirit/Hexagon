using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ScenarioPlayer : MonoBehaviour
{
    public List<Trigger_Type> PlayedScenarios = new();
    public bool IsPlaying { get; private set; }

    Dictionary<Trigger_Type, List<ScenarioData>> _scenarioMap;
    bool _initialized;
    public bool IsInitialized => _initialized;

    private void Awake()
    {
        _scenarioMap = new Dictionary<Trigger_Type, List<ScenarioData>>();
        StartCoroutine(InitRoutine());
    }

    private IEnumerator InitRoutine()
    {
        
        while (DataManager.Instance == null || !DataManager.Instance.IsReady)
            yield return null;

        FillScenarioDatas();
        _initialized = true;
    }

    public bool IsScenarioPlayed(Trigger_Type trigger)
    {
        return PlayedScenarios.Contains(trigger);
    }

    public bool RequestScenario(Trigger_Type trigger)
    {
        if (IsPlaying || !_initialized)
            return false;

        if (!_scenarioMap.TryGetValue(trigger, out var list))
            return false;

        if (list == null || list.Count == 0)
            return false;

        IsPlaying = true;

        Player.Instance.EnterScenarioMod();

        Player.Instance.EnsureTalkUI();

        Player.Instance.TalkUI.EnterScenario(list);
        Player.Instance.TalkUI.OnScenarioEnd += OnScenarioEnd;

        return true;
    }

    private void OnScenarioEnd()
    {
        Player.Instance.TalkUI.OnScenarioEnd -= OnScenarioEnd;
        IsPlaying = false;

        var trigger = Player.Instance.CurrentPlayedScenario;
        if (!PlayedScenarios.Contains(trigger))
        {
            PlayedScenarios.Add(trigger);
        }            

        Player.Instance.OnScenarioFinished();
    }

    private void FillScenarioDatas()
    {
        _scenarioMap[Trigger_Type.gamestart] = Load("scn_Intro_");
        _scenarioMap[Trigger_Type.dungeonenter] = Load("scn_dun_envy_");
        _scenarioMap[Trigger_Type.stageenter] = Load("scn_dun_envy_st1_");
        _scenarioMap[Trigger_Type.prebattle] = Load("scn_boss_pride_");
        _scenarioMap[Trigger_Type.preseal] = Load("scn_boss_seal_");
        _scenarioMap[Trigger_Type.clear] = Load("scn_pride_clear_");
    }

    private List<ScenarioData> Load(string key)
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
    public void RestorePlayedScenarios(List<Trigger_Type> list)
    {
        PlayedScenarios.Clear();

        if (list != null)
            PlayedScenarios.AddRange(list);
    }
}
