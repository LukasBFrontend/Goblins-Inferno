using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuState : BaseState
{
    public MainMenuState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        if (Ctx.LogNewStateOnEnter)
        {
            Debug.Log($"{Ctx.name} entered <b><color=white>new state</color></b>: <color=yellow>{nameof(MainMenuState)}</color>");
        }

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            SceneManager.LoadScene(0);
        }

        Time.timeScale = 1;
        GameEvents.LevelStarted.AddListener(OnLevelStarted);
    }

    void OnLevelStarted()
    {
        SwitchState(Factory.Level());
    }

    public override void UpdateState(){ }

    public override void ExitState()
    {
        GameEvents.LevelStarted.RemoveListener(OnLevelStarted);
    }

    public override void CheckSwitchStates() { }
    public override void InitializeSubState() { }
}
