using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuState : BaseState
{
    public MainMenuState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory)
    {
        IsRootState = true;
        GameSaveState.Load();
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
        GameEvents.LevelLoaded.AddListener(OnLevelStarted);
        GameEvents.RaiseMainMenuLoaded();
    }

    void OnLevelStarted()
    {
        SwitchState(Factory.Level());
    }

    public override void UpdateState(){ }

    public override void ExitState()
    {
        GameEvents.LevelLoaded.RemoveListener(OnLevelStarted);
    }

    public override void CheckSwitchStates() { }
    public override void InitializeSubState() { }
}
