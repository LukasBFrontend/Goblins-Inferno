using System.Threading.Tasks;
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

        Time.timeScale = 1;
        GameSaveState.Load();
        GameEvents.StartLevelInvoked.AddListener(OnStartLevelInvoked);
    }

    async void OnStartLevelInvoked()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(1);
        loadOperation.allowSceneActivation = false;

        Task loadingTask = WaitForSceneLoaded(loadOperation);
        Task fadeTask = Game.VolumeTransitions.ColorFadeOut();

        await Task.WhenAll(loadingTask, fadeTask);

        loadOperation.allowSceneActivation = true;

        while (!loadOperation.isDone)
        {
            await Task.Yield();
        }

        SwitchState(Factory.Level());
    }

    async Task WaitForSceneLoaded(AsyncOperation operation)
    {
        while (operation.progress < 0.9f)
        {
            await Task.Yield();
        }
    }

    public override void ExitState()
    {
        GameEvents.StartLevelInvoked.RemoveListener(OnStartLevelInvoked);
    }

    public override void UpdateState(){ }

    public override void CheckSwitchStates() { }
    public override void InitializeSubState() { }
}
