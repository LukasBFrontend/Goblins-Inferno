using System.Collections;
using UnityEngine;

public class WeaponModel : MonoBehaviour
{
    [Tooltip("Enemies inside the collider are considered within range of the tracker.")]
    [SerializeField] Collider collider;
    [SerializeField] MeshRenderer renderer;
    public MeshRenderer Renderer => renderer;
    SO_WeaponData _weapon;

    public void Initialize(SO_WeaponData weapon)
    {
        _weapon = weapon;

        if (renderer != null)
        {
            renderer.enabled = false;
        }
    }

    public void StartAnimation(float animationDuration)
    {
        StartCoroutine(WeaponAnimationRoutine(animationDuration));
    }

    IEnumerator WeaponAnimationRoutine(float animationDuration)
    {
        renderer.enabled = true;
        yield return new WaitForSeconds(animationDuration);
        renderer.enabled = false;
    }

    private void Awake()
    {
        if (collider == null)
        {
            Debug.LogWarning($"WeaponTargetTracker assigned to {name} is missing a Collider reference and will not function as intended.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Enemy>(out var enemy))
        {
            return;
        }

        _weapon.EnterIntoRange(enemy);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Enemy>(out var enemy))
        {
            return;
        }

        _weapon.ExitFromRange(enemy);
    }
}
