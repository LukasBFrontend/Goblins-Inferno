using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverState : BaseState
{
    public GameOverState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory)
    {
        // Initialization logic
    }
    public override void EnterState()
    {
        GameEvents.LevelStarted.AddListener(OnLevelStarted);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        GameEvents.LevelStarted.RemoveListener(OnLevelStarted);
    }

    void OnLevelStarted()
    {
        SwitchState(Factory.Playing());
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public override void CheckSwitchStates() { }
    public override void InitializeSubState() { }
}
