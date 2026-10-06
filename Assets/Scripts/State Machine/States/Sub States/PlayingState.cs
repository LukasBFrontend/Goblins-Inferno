using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayingState : BaseState
{
    public PlayingState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory)
    {
        RegisterCallbacks();
    }
    public override void EnterState()
    {
        Game.Input.Move.Enable();
        RegisterCallbacks();

        Time.timeScale = 1;
    }

    public override void UpdateState()
    {
        Game.Player.Movement.SetInput(Game.Input.Move.ReadValue<Vector2>());
        Game.Player.Weapons.Attack();
        CheckSwitchStates();
    }

    public override void ExitState() {
        Game.Input.Move.Disable();
        UnregisterCallbacks();
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
        GameEvents.RaiseGamePaused();
        SwitchState(Factory.Paused());
    }

    public override void CheckSwitchStates() { }
    public override void InitializeSubState() { }
}
