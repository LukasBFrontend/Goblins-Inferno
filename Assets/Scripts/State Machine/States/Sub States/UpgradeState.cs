using UnityEngine;

public class UpgradeState : BaseState
{
    public UpgradeState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory)
    {
        // Initialization logic
    }
    public override void EnterState()
    {
        GameEvents.GameResumed.AddListener(OnGameResumed);
        Game.UpgradeOptionManager.AssignRandom();
        Time.timeScale = 0;
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
