using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Health))]
public abstract class BaseCharacter : MonoBehaviour
{
    [SerializeField] GameObject renderGroup;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] Collider collider;
    [SerializeField] Health health;
    public MeshRenderer[] Renderers => _renderers ??= renderGroup.GetComponentsInChildren<MeshRenderer>().Where(renderer => renderer.gameObject.activeSelf == true).ToArray();
    public GameObject RenderGroup => renderGroup;
    public Rigidbody Rigidbody => rigidbody;
    public Collider Collider => collider;
    public Health Health => health;
    public abstract void Die();
    MeshRenderer[] _renderers;

    public Vector3 DirectionTo(BaseCharacter character)
    {
        Vector3 position = rigidbody.position;
        Vector3 targetPosition = character.Rigidbody.position;
        return (targetPosition - position).normalized;
    }
}
