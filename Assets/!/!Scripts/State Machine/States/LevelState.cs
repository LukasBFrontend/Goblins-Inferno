using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelState : BaseState
{
    public LevelState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory)
    {
        IsRootState = true;
        InitializeSubState();
    }

    public async override void EnterState()
    {
        if (Ctx.LogNewStateOnEnter)
        {
            Debug.Log($"{Ctx.name} entered <b><color=white>new state</color></b>: <color=yellow>{nameof(LevelState)}</color> \n<b><color=white>substate</color></b>: <color=yellow>{Factory.Playing().GetType().Name}</color>");
        }

        await Game.VolumeTransitions.GammaFadeIn();

        GameEvents.QuitLevelInvoked.AddListener(OnQuitLevelInvoked);
        GameSaveState.Score.Reset();
    }

    public override void ExitState()
    {
        GameEvents.QuitLevelInvoked.RemoveListener(OnQuitLevelInvoked);
    }

    async void OnQuitLevelInvoked()
    {
        await SceneManager.LoadSceneAsync(0);

        CurrentSubState.ExitState();
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
