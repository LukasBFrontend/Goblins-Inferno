using Luscious;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputActions : Singleton<GameInputActions>
{
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference jump;
    [SerializeField] InputActionReference togglePause;

    public InputAction Move => move.action;
    public InputAction Jump => jump.action;
    public InputAction TogglePause => togglePause.action;
}
