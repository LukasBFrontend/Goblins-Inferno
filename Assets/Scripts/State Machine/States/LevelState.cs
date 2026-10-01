using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelState : BaseState
{
    public LevelState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory)
    {
        IsRootState = true;
    }
    public override void EnterState()
    {
        if (Ctx.LogNewStateOnEnter)
        {
            Debug.Log($"{Ctx.name} entered <b><color=white>new state</color></b>: <color=yellow>{nameof(LevelState)}</color> \n<b><color=white>substate</color></b>: <color=yellow>{Factory.Playing().GetType().Name}</color>");
        }

        if (SceneManager.GetActiveScene().buildIndex != 1)
        {
            SceneManager.LoadScene(1);
        }

        GameEvents.LevelQuit.AddListener(OnLevelQuit);
        SetSubState(Factory.Playing());
    }

    void OnLevelQuit()
    {
        SwitchState(Factory.MainMenu());
    }

    public override void UpdateState(){ }

    public override void ExitState()
    {
        GameEvents.LevelQuit.RemoveListener(OnLevelQuit);
    }

    public override void CheckSwitchStates() { }
    public override void InitializeSubState() { }
}
