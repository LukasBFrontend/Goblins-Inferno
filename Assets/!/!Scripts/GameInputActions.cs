using Luscious;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputActions : Singleton<GameInputActions>
{
    [Header("Player")]
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference jump;
    [SerializeField] InputActionReference togglePause;
    [Header("UI")]
    [SerializeField] InputActionReference navigate;
    [SerializeField] InputActionReference click;

    public InputAction Move => move.action;
    public InputAction Jump => jump.action;
    public InputAction TogglePause => togglePause.action;

    public InputAction UINavigate => navigate.action;
    public InputAction UIClic => click.action;
}
