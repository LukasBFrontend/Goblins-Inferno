using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputActions : Singleton<GameInputActions>
{
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference togglePause;
    public InputAction Move => move.action;
    public InputAction TogglePause => togglePause.action;
}
