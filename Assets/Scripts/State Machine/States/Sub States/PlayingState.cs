using UnityEngine;
using UnityEngine.InputSystem;

public class PlayingState : BaseState
{
    InputAction _move;
    public PlayingState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory)
    {
        // Initialization logic
        GameEvents.LvlUpEvent.AddListener(OnLvlUp);
        GameEvents.GamePaused.AddListener(OnGamePaused);
        GameEvents.GameOverEvent.AddListener(OnGameOver);
        _move = Game.Input.Move;
    }
    public override void EnterState()
    {
        GameEvents.LvlUpEvent.AddListener(OnLvlUp);
        GameEvents.GamePaused.AddListener(OnGamePaused);
        GameEvents.GameOverEvent.AddListener(OnGameOver);

        Time.timeScale = 1;
        _move.Enable();
    }

    void OnLvlUp(int oldLvl, int newLvl)
    {
        SwitchState(Factory.Upgrade());
    }

    void OnGamePaused()
    {
        SwitchState(Factory.Paused());
    }

    void OnGameOver()
    {
        SwitchState(Factory.GameOver());
    }

    public override void UpdateState()
    {
        Game.Player.Movement.SetInput(_move.ReadValue<Vector2>());
        CheckSwitchStates();
    }

    public override void ExitState() {
        GameEvents.LvlUpEvent.RemoveListener(OnLvlUp);
        GameEvents.GamePaused.RemoveListener(OnGamePaused);
        GameEvents.GameOverEvent.RemoveListener(OnGameOver);

        Time.timeScale = 0;
        _move.Disable();
    }

    public override void CheckSwitchStates() { }
    public override void InitializeSubState() { }
}
