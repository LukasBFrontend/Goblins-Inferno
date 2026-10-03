using UnityEngine;

public abstract class BaseState
{
    protected bool IsRootState { get => _isRootState; set => _isRootState = value; }
    protected StateMachine Ctx => _ctx;
    protected StateFactory Factory => _factory;
    bool _isRootState = false;
    StateMachine _ctx;
    StateFactory _factory;
    BaseState _currentSuperState;
    BaseState _currentSubState;

    public BaseState(StateMachine currentContext, StateFactory StateFactory)
    {
        _ctx = currentContext;
        _factory = StateFactory;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    public void UpdateStates()
    {
        UpdateState();
        _currentSubState?.UpdateStates();
    }

    protected void SwitchState(BaseState newState)
    {
        if (Ctx.LogNewStateOnEnter && (_currentSuperState != null || _currentSubState != null))
        {
            string logString = $"{Ctx.name} switched to a <b><color=white>new state</color></b>: <color=yellow>{newState.GetType().Name}</color>";
            if (_currentSuperState != null)
            {
                logString += $"\n<b><color=white>super state:</color></b> <color=yellow>{_currentSuperState}</color>";
            }
            else if (_currentSubState != null)
            {
                logString += $"\n<b><color=white>sub state:</color></b> <color=yellow>{_currentSubState}</color>";
            }

            Debug.Log(logString);
        }

        ExitState();
        newState.EnterState();

        if (_isRootState)
        {
            Ctx.CurrentState = newState;
        }
        else
        {
            _currentSuperState?.SetSubState(newState);
        }
    }

    protected void SetSuperState(BaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(BaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
