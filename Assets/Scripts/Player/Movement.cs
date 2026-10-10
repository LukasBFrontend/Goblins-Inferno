using UnityEngine;

[RequireComponent(typeof(Player))]
public class Movement : MonoBehaviour
{
    Player _player;

    public void SetInput(Vector2 moveInput)
    {
        Vector3 velocity = new Vector3(moveInput.x, 0f, moveInput.y) * _player.Stats.MovementSpeed;
        _player.Rigidbody.linearVelocity = velocity;

        _player.Animator.SetFloat("MoveSpeed", velocity.magnitude);

        if (moveInput.magnitude > .1f)
        {
            _player.RenderTransform.localRotation = Quaternion.Euler(0, -90 - Mathf.Rad2Deg * Mathf.Atan2(moveInput.y, moveInput.x), 0);
        }
    }

    void Awake()
    {
        _player = GetComponent<Player>();
    }
}
