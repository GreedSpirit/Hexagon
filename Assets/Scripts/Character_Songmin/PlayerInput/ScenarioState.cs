using UnityEngine;
using UnityEngine.InputSystem;

public class ScenarioState : IInputState
{
    Player _player;
    PlayerModelController _controller;
    PlayerInputHandler _handler;

    public ScenarioState(Player player, PlayerInputHandler handler)
    {
        _player = player;
        _handler = handler;
        _controller = _player.GetComponent<PlayerModelController>();
    }

    public void OnEnter()
    {
        _handler.ChangeActionMap("Scenario");
    }

    public void OnExit()
    {
        _controller.SetMoveInput(Vector2.zero);
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            _player.UpdateScenario();
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        return;
    }
}
