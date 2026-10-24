using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverState : BaseState
{
    public GameOverState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory) { }

    public override void EnterState()
    {
        GameEvents.StartLevelInvoked.AddListener(OnLevelStarted);

        Time.timeScale = 0.5f;
        Game.Player.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        var score = GameSaveState.Score;

        if (score.Current > score.HighScore)
        {
            score.SetHighScore(score.Current);
            GameSaveState.Save();
        }
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        GameEvents.StartLevelInvoked.RemoveListener(OnLevelStarted);
    }

    void OnLevelStarted()
    {
        SwitchState(Factory.Playing());
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public override void CheckSwitchStates() { }
    public override void InitializeSubState() { }
}
