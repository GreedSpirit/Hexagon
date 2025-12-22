using UnityEngine;

/// <summary>
/// 추후 던전 혹은 마을 등 배경 및 BGM을 바꾸기 위해서 사용될 클래스
/// </summary>

public class EnvironmentSetting : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _backgroundRenderer;
    [SerializeField] private AudioSource _backgroundMusic;

    public void ChangeEnvironment(string bgimgKey, string bgmusicKey)
    {
        Sprite bgSprite = Resources.Load<Sprite>($"Backgrounds/{bgimgKey}");
        AudioSource bgMusic = Resources.Load<AudioSource>($"Backgrounds/{bgmusicKey}");

        if(bgSprite != null)
        {
            _backgroundRenderer.sprite = bgSprite;
        }
        if(bgMusic != null)
        {
            _backgroundMusic = bgMusic;
        }
    }
}
