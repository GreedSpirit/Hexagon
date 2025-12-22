using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] Player _player;
    [SerializeField] PlayerInput playerInput;
    IInputState _currentInput;

    private void Awake()
    {
        if (_player == null)
        {
            _player = Player.Instance;
        }
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }
    }

    private void Start()
    {
        ChangeInputState(new MoveState(_player, this));
    }

    public void ChangeActionMap(string mapName)
    {
        playerInput.SwitchCurrentActionMap(mapName);
    }

    public void ChangeInputState(IInputState newInputState)
    {
        _currentInput?.OnExit();
        _currentInput = newInputState;
        _currentInput.OnEnter();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        _currentInput?.OnMove(ctx);
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        _currentInput?.OnInteract(ctx);
    }
    public void OnSkip(InputAction.CallbackContext ctx)
    {
        _currentInput?.OnInteract(ctx);
    }
}
