using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeState : BaseState
{
    private const float MOUSE_MOVE_THRESHOLD_SQR = 0.01f;

    private Vector2 _mouseStart;
    private bool _mouseMoveThresholdSurpassed;
    private static WaitForSecondsRealtime _waitForSeconds_05 = new(.5f);

    public UpgradeState(StateMachine currentContext, StateFactory StateFactory) : base(currentContext, StateFactory) { }

    public override void EnterState()
    {
        _mouseStart = Mouse.current.position.ReadValue();

        GameEvents.GameResumed.AddListener(OnGameResumed);
        Game.UpgradeOptions.AssignRandom();
        Time.timeScale = 0;

        Game.Input.UIClic.Disable();
        Game.Player.StartCoroutine(DisableUINavigateRoutine());
    }

    void OnGameResumed()
    {
        SwitchState(Factory.Playing());
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        if (!_mouseMoveThresholdSurpassed)
        {
            Vector2 mouseMoveDelta = Mouse.current.position.ReadValue() - _mouseStart;

            if (mouseMoveDelta.sqrMagnitude > MOUSE_MOVE_THRESHOLD_SQR)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Game.Input.UIClic.Enable();

                _mouseMoveThresholdSurpassed = true;
            }
        }
    }

    public override void ExitState()
    {
        GameEvents.GameResumed.RemoveListener(OnGameResumed);
        Game.Input.UIClic.Enable();
        Game.Input.UINavigate.Enable();
    }

    private IEnumerator DisableUINavigateRoutine()
    {
        Game.Input.UINavigate.Disable();
        yield return _waitForSeconds_05;
        Game.Input.UINavigate.Enable();
    }

    public override void CheckSwitchStates() { }
    public override void InitializeSubState() { }
}
