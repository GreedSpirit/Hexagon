using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NpcTalkSlideUI : MonoBehaviour
{
    [SerializeField] RectTransform panel;
    [SerializeField] TextMeshProUGUI Name;
    [SerializeField] float duration = 0.3f;
    Npc _npc;

    Vector2 _hiddenPos;
    Vector2 _viewPos;
    Coroutine _routine;

    void Awake()
    {
        _hiddenPos = panel.anchoredPosition;
        _viewPos = _hiddenPos + new Vector2(-panel.rect.width, 0); // ¿ÞÂÊ
        panel.anchoredPosition = _viewPos;

        Hide();
    }

    public void Show()
    {
        StartSlide(_viewPos);
    }

    public void Hide()
    {
        StartSlide(_hiddenPos);
    }

    public void SetNpc(Npc npc)
    {
        _npc = npc;
        Name.text = _npc.Name;
    }

    void StartSlide(Vector2 target)
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
        }            

        _routine = StartCoroutine(Slide(target));
    }
    


    private void OnDestroy()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
        }
    }

    IEnumerator Slide(Vector2 target)
    {
        Vector2 start = panel.anchoredPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            panel.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        panel.anchoredPosition = target;
    }
}
