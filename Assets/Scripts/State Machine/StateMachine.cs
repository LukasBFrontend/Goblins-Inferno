
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateMachine : Singleton<StateMachine>
{
    [SerializeField] bool logNewStateOnEnter;
    public BaseState CurrentState { get => _currentState; set => _currentState = value; }
    public bool LogNewStateOnEnter => logNewStateOnEnter;
    BaseState _currentState;
    StateFactory _states;

    void Start()
    {
        _states = new StateFactory(this);

        _currentState = SceneManager.GetActiveScene().buildIndex == 0
            ? _states.MainMenu()
            : _states.Level()
        ;
        _currentState.EnterState();
        _currentState.InitializeSubState();
    }

    void Update()
    {
        _currentState.UpdateStates();
    }
}
