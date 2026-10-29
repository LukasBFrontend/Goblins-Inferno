using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class Movement : MonoBehaviour
{
    Player _player;
    Vector3 _lastMoveInput;
    bool _overrideMoveInput;

    public void SetInput(Vector2 moveInput)
    {
        if (_overrideMoveInput)
        {
            return;
        }

        _lastMoveInput = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 velocity = _lastMoveInput * _player.Stats.MovementSpeed;

        _player.Rigidbody.linearVelocity = velocity;
        _player.Animator.SetFloat("MoveSpeed", velocity.magnitude);

        if (moveInput.magnitude > .1f)
        {
            _player.transform.rotation = Quaternion.Euler(0, 90 - Mathf.Rad2Deg * Mathf.Atan2(moveInput.y, moveInput.x), 0);
        }
    }

    public void Roll()
    {
        StartCoroutine(RollRoutine());
    }

    IEnumerator RollRoutine()
    {
        _overrideMoveInput = true;
        _player.Animator.SetBool("DoRoll", true);

        Vector3 direction = _lastMoveInput * _player.Stats.MovementSpeed;

        float animationDuration = _player.Animator.GetCurrentAnimatorStateInfo(0).length;
        //_player.Health.SetInvincibilityFrames(animationDuration * .125f, .35f); Should cache WaitForSeconds.

        float elapsed = 0f;
        float duration = animationDuration * 0.85f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float multiplier = Mathf.Lerp(1.65f, .85f, t);

            _player.Rigidbody.linearVelocity = direction * multiplier;

            elapsed += Time.deltaTime;
            yield return null;
        }

        _player.Rigidbody.linearVelocity = direction;
        _player.Animator.SetBool("DoRoll", false);

        _overrideMoveInput = false;
    }

    void Awake()
    {
        _player = GetComponent<Player>();
    }

}
