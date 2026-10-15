using UnityEngine;
using UnityEngine.InputSystem;

public class PlayingState : BaseState
{
    public PlayingState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory)
    {
        RegisterCallbacks();
    }
    public override void EnterState()
    {
        Game.Input.Move.Enable();
        Game.Input.Jump.Enable();

        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        RegisterCallbacks();
    }

    public override void UpdateState()
    {
        Game.Player.Movement.SetInput(Game.Input.Move.ReadValue<Vector2>());
        Game.Player.Weapons.Attack();
        Game.EnemySpawner.Run();

        CheckSwitchStates();
    }

    public override void ExitState() {
        Game.Input.Move.Disable();
        Game.Input.Jump.Disable();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        UnregisterCallbacks();
    }

    void RegisterCallbacks()
    {
        GameEvents.LvlUpEvent.AddListener(OnLvlUp);
        GameEvents.GameOverEvent.AddListener(OnGameOver);

        Game.Input.Jump.performed += OnJumpPerformed;
        Game.Input.TogglePause.performed += OnTogglePausePerformed;
    }

    void UnregisterCallbacks()
    {
        GameEvents.LvlUpEvent.RemoveListener(OnLvlUp);
        GameEvents.GameOverEvent.RemoveListener(OnGameOver);

        Game.Input.Jump.performed -= OnJumpPerformed;
        Game.Input.TogglePause.performed -= OnTogglePausePerformed;
    }

    void OnLvlUp(int oldLvl, int newLvl)
    {
        SwitchState(Factory.Upgrade());
    }

    void OnGameOver()
    {
        SwitchState(Factory.GameOver());
    }

    void OnJumpPerformed(InputAction.CallbackContext callbackContext)
    {
        if (Game.Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("ROLL"))
        {
            return;
        }
        Game.Player.Movement.Roll();
    }

    void OnTogglePausePerformed(InputAction.CallbackContext callbackContext)
    {
        GameEvents.RaiseGamePaused();
        SwitchState(Factory.Paused());
    }

    public override void CheckSwitchStates() { }
    public override void InitializeSubState() { }
}
