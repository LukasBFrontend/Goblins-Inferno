using UnityEngine;

[RequireComponent(typeof(Player))]
public class Movement : MonoBehaviour
{
    [SerializeField] float baseMoveSpeed = 10;
    [SerializeField] Animator animator;
    Player _player;
    public void SetInput(Vector2 moveInput)
    {
        Vector3 velocity = new Vector3(moveInput.x, 0f, moveInput.y) * baseMoveSpeed;
        _player.Rigidbody.linearVelocity = velocity;
        animator.SetFloat("MoveSpeed", velocity.magnitude);
    }

    void Awake()
    {
        _player = GetComponent<Player>();
    }
}
