using UnityEngine;

public class DungeonEntrance : MonoBehaviour
{
    [SerializeField] DungeonPresenter _presenter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            _presenter.ShowDungeon();
            player.EnterBattleMod();
        }
    }    
}
