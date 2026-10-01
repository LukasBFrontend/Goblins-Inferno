using UnityEngine;

[RequireComponent(typeof(Health))]
public abstract class BaseCharacter : MonoBehaviour
{
    [SerializeField] MeshRenderer renderer;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] Collider collider;
    [SerializeField] Health health;
    public MeshRenderer Renderer => renderer;
    public Rigidbody Rigidbody => rigidbody;
    public Collider Collider => collider;
    public Health Health => health;
    public abstract void Die();

    public Vector3 DirectionTo(BaseCharacter character)
    {
        Vector3 position = rigidbody.position;
        Vector3 targetPosition = character.Rigidbody.position;
        return (targetPosition - position).normalized;
    }
}
