public class GameOverState : BaseState
{
    public GameOverState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory)
    {
        // Initialization logic
    }
    public override void EnterState()
    {

    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates() { }
    public override void InitializeSubState() { }
}
