using UnityEngine;
using UnityEngine.InputSystem;

public class PlayingState : BaseState
{
    InputAction _move;
    public PlayingState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory)
    {
        // Initialization logic
        _move = Game.Input.Move;
    }
    public override void EnterState()
    {
        _move.Enable();
    }

    public override void UpdateState()
    {
        Game.Player.Movement.SetInput(_move.ReadValue<Vector2>());
        CheckSwitchStates();
    }

    public override void ExitState() {
        _move.Disable();
    }

    public override void CheckSwitchStates() { }
    public override void InitializeSubState() { }
}
