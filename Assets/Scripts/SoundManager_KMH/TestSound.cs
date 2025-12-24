using UnityEngine;

public class TestSound : MonoBehaviour
{
    [SerializeField] AudioClip bgmClip;
    [SerializeField] AudioClip sfxClip;

    public void PlayBGM()
    {
        SoundManager.Instance.PlayBGM(BGMType.Battle);
    }
    public void PlaySFX()
    {
        // SoundManager.Instance.PlaySFX(sfxClip);
        SoundManager.Instance.PlaySFX(SFXType.Click);
    }
}
