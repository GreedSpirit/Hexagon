using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    private void Start()
    {
        // 타이틀 BGM 재생 (SoundManager)
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(BGMType.Title);
        }
    }

    // Start 버튼 연결용
    public void OnClickStart()
    {
        // 효과음 재생
        SoundManager.Instance?.PlaySFX(SFXType.Click);

        // 게임 씬으로 이동
        SceneTransitionManager.Instance?.LoadSceneWithFade("VillageScene_SMS");
    }

    // Exit 버튼 연결용
    public void OnClickExit()
    {
        SoundManager.Instance?.PlaySFX(SFXType.Click);

        Debug.Log("게임 종료");
        Application.Quit();

#if UNITY_EDITOR // 종료 테스트용
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}