using UnityEngine;

public class TestSound : MonoBehaviour
{
    [SerializeField] AudioClip bgmClip;
    [SerializeField] AudioClip sfxClip;

    public void PlayBGM()
    {
        SoundManager.Instance.PlayBGM(bgmClip);
    }
    public void PlaySFX()
    {
        SoundManager.Instance.PlaySFX(sfxClip);
    }
}
