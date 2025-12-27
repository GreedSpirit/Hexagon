using UnityEngine;
using UnityEngine.InputSystem;

public class TalkState : IInputState
{
    Player _player;
    PlayerModelController _controller;
    PlayerInputHandler _handler;

    public TalkState(Player player, PlayerInputHandler handler)
    {
        _player = player;
        _handler = handler;
        _controller = _player.GetComponent<PlayerModelController>();
    }

    public void OnEnter()
    {
        _handler.ChangeActionMap("Talk");
    }

    public void OnExit()
    {
        _controller.SetMoveInput(Vector2.zero);
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            _player.EndTalk();
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        return;
    }
}
