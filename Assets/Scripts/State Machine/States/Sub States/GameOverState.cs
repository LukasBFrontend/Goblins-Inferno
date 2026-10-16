using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverState : BaseState
{
    public GameOverState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory) { }

    public override void EnterState()
    {
        GameEvents.LevelLoaded.AddListener(OnLevelStarted);

        Time.timeScale = 0.5f;
        Game.Player.Rigidbody.linearVelocity = Vector3.zero;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        GameEvents.LevelLoaded.RemoveListener(OnLevelStarted);
    }

    void OnLevelStarted()
    {
        SwitchState(Factory.Playing());
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public override void CheckSwitchStates() { }
    public override void InitializeSubState() { }
}
