
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateMachine : Singleton<StateMachine>
{
    [SerializeField] bool logNewStateOnEnter;
    public BaseState CurrentState { get => _currentState; set => _currentState = value; }
    public bool LogNewStateOnEnter => logNewStateOnEnter;
    BaseState _currentState;
    StateFactory _states;

    protected override void OnSingletonAwake()
    {
        _states = new StateFactory(this);

        _currentState = SceneManager.GetActiveScene().buildIndex == 0
            ? _states.MainMenu()
            : _states.Level()
        ;
        _currentState.EnterState();
    }

    void Update()
    {
        _currentState.UpdateStates();
    }
}
