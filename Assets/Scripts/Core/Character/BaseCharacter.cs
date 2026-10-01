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
}
