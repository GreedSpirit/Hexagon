using UnityEngine;
using UnityEngine.InputSystem;

public class MoveState : IInputState
{
    Player _player;
    PlayerModelController _controller;
    PlayerInputHandler _handler;
    
    public MoveState(Player player, PlayerInputHandler handler)
    {
        _player = player;
        _handler = handler;
        _controller = _player.GetComponent<PlayerModelController>();
    }

    public void OnEnter()
    {
        _handler.ChangeActionMap("Village");
    }

    public void OnExit()
    {
        _controller.SetMoveInput(Vector2.zero);
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _controller.Interact();
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {        
        if (ctx.canceled)
        {
            _controller.SetMoveInput(Vector2.zero);
            return;
        }
        Vector2 moveDirection = ctx.ReadValue<Vector2>();
        _controller.SetMoveInput(moveDirection);
    }
}
