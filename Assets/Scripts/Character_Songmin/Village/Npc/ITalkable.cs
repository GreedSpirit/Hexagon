using UnityEngine;

public interface ITalkable
{
    public void SwitchIsTalking(bool talking);
    public string GetName();
    public string GetTalk();
    public string GetImage();
}
