using UnityEngine;

public class GameOverState : BaseState
{
    public GameOverState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory)
    {
        // Initialization logic
    }
    public override void EnterState()
    {

    }

    void OnGameResumed()
    {
        SwitchState(Factory.Playing());
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        GameEvents.GameResumed.RemoveListener(OnGameResumed);
    }

    public override void CheckSwitchStates() { }
    public override void InitializeSubState() { }
}
