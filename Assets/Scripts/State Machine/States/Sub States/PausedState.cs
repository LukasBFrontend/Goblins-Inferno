using UnityEngine;
using UnityEngine.InputSystem;

public class PausedState : BaseState
{
    public PausedState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory)
    {
        // Initialization logic
    }
    public override void EnterState()
    {
        RegisterCallbacks();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        UnregisterCallbacks();
    }

    void RegisterCallbacks()
    {
        Game.Input.TogglePause.performed += OnTogglePausePerformed;
        GameEvents.GameResumed.AddListener(OnGameResumed);
    }

    void UnregisterCallbacks()
    {
        Game.Input.TogglePause.performed -= OnTogglePausePerformed;
        GameEvents.GameResumed.RemoveListener(OnGameResumed);
    }

    void OnTogglePausePerformed(InputAction.CallbackContext callbackContext)
    {
        GameEvents.RaiseGameResumed();
    }

    void OnGameResumed()
    {
        SwitchState(Factory.Playing());
    }

    public override void CheckSwitchStates() { }
    public override void InitializeSubState() { }
}
