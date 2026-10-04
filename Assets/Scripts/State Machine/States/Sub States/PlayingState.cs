using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayingState : BaseState
{
    InputAction _move;
    public PlayingState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory)
    {
        _move = Game.Input.Move;
        RegisterCallbacks();
    }
    public override void EnterState()
    {
        _move.Enable();
        RegisterCallbacks();

        Time.timeScale = 1;
    }

    public override void UpdateState()
    {
        Game.Player.Movement.SetInput(_move.ReadValue<Vector2>());
        CheckSwitchStates();
    }

    public override void ExitState() {
        _move.Disable();
        UnregisterCallbacks();

        Time.timeScale = 0;
    }

    void RegisterCallbacks()
    {
        GameEvents.LvlUpEvent.AddListener(OnLvlUp);
        GameEvents.GameOverEvent.AddListener(OnGameOver);
        Game.Input.TogglePause.performed += OnTogglePausePerformed;
    }

    void UnregisterCallbacks()
    {
        GameEvents.LvlUpEvent.RemoveListener(OnLvlUp);
        GameEvents.GameOverEvent.RemoveListener(OnGameOver);
        Game.Input.TogglePause.performed -= OnTogglePausePerformed;
    }

    void OnLvlUp(int oldLvl, int newLvl)
    {
        SwitchState(Factory.Upgrade());
    }

    void OnGameOver()
    {
        SwitchState(Factory.GameOver());
    }

    void OnTogglePausePerformed(InputAction.CallbackContext callbackContext)
    {
        SwitchState(Factory.Paused());
        GameEvents.RaiseGamePaused();
    }

    public override void CheckSwitchStates() { }
    public override void InitializeSubState() { }
}
