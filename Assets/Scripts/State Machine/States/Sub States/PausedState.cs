using UnityEngine;

public class PausedState : BaseState
{
    public PausedState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory)
    {
        // Initialization logic
    }
    public override void EnterState()
    {
        GameEvents.GameResumed.AddListener(OnGameResumed);
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
