using UnityEngine;

public enum MonsterSoundType
{
    None,
    Hit_Body,
    Hit_Burn,
    Hit_Poison,
    Hit_Shield,
    Atk_Swing,
    Atk_Explosion,
    Atk_Fire,
    Atk_Laser,
    Atk_Pride,
    Die,
    Reward_get
}

[System.Serializable]
public struct MonsterSoundStruct
{
    public MonsterSoundType type;
    public AudioClip clip;
}
