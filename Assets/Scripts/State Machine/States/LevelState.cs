using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelState : BaseState
{
    public LevelState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory)
    {
        IsRootState = true;
        InitializeSubState();
    }

    public override void EnterState()
    {
        if (Ctx.LogNewStateOnEnter)
        {
            Debug.Log($"{Ctx.name} entered <b><color=white>new state</color></b>: <color=yellow>{nameof(LevelState)}</color> \n<b><color=white>substate</color></b>: <color=yellow>{Factory.Playing().GetType().Name}</color>");
        }

        SetSubState(Factory.Playing());

        if (SceneManager.GetActiveScene().buildIndex != 1)
        {
            SceneManager.LoadScene(1);
        }

        GameEvents.LevelQuit.AddListener(OnLevelQuit);
        Game.ResetScore();
    }

    public override void ExitState()
    {
        GameSaveState.Save(Game.Score, AvatarVariant.Default);
        GameEvents.LevelQuit.RemoveListener(OnLevelQuit);
    }

    void OnLevelQuit()
    {
        SetSubState(null);
        SwitchState(Factory.MainMenu());
    }

    public override void UpdateState(){ }

    public override void CheckSwitchStates() { }
    public override void InitializeSubState()
    {
        BaseState playingState = Factory.Playing();

        SetSubState(playingState);
        playingState.EnterState();
    }
}
