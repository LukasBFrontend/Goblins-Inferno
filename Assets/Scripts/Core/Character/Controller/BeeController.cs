using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class BeeController : MonoBehaviour
{
    [Range(0.5f, 50f)]
    [SerializeField] float moveSpeed;
    Enemy _enemy;
    Rigidbody _rigidbody;

    void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _rigidbody = _enemy.Rigidbody;
    }


    void Update()
    {
        Vector3 dir = _enemy.DirectionTo(Game.Player);

        _rigidbody.linearVelocity = dir * moveSpeed;
    }
}
