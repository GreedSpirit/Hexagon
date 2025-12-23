using UnityEngine;
using UnityEngine.InputSystem;

public class BattleState : IInputState
{
    Player _player;
    PlayerModelController _controller;
    PlayerInputHandler _handler;

    public BattleState(Player player, PlayerInputHandler handler)
    {
        _player = player;
        _handler = handler;
        _controller = _player.GetComponent<PlayerModelController>();
    }

    public void OnEnter()
    {
        _handler.ChangeActionMap("Battle");
    }

    public void OnExit()
    {
        _controller.SetMoveInput(Vector2.zero);
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        return;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        return;
    }
}
