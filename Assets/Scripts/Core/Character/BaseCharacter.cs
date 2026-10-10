using System.Linq;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour
{
    [SerializeField] Transform renderTransform;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] Collider collider;
    [SerializeField] Health health;

    public Renderer[] Renderers => _renderers ??= renderTransform.GetComponentsInChildren<Renderer>().Where(renderer => renderer.gameObject.activeSelf == true).ToArray();
    public Transform RenderTransform => renderTransform;
    public Animator Animator => animator;
    public Rigidbody Rigidbody => rigidbody;
    public Collider Collider => collider;
    public Vector3 ColliderCenter => collider.bounds.center;
    public Health Health => health;

    Renderer[] _renderers;

    public Vector3 DirectionTo(BaseCharacter character)
    {
        Vector3 position = rigidbody.position;
        Vector3 targetPosition = character.Rigidbody.position;
        return (targetPosition - position).normalized;
    }

    public float DistanceTo(BaseCharacter character)
    {
        Vector3 position = rigidbody.position;
        Vector3 targetPosition = character.Rigidbody.position;

        return Vector3.Distance(position, targetPosition);
    }

    public abstract void Die();
}
