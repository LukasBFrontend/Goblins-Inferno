using System.Collections;
using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [Tooltip("How much damage does the enemy deal on player collision?")]
    [Range(0, 40)]
    [SerializeField] int contactDamage = 20;
    [Range(0, 2)]
    [SerializeField] float cooldown = 1;
    WaitForSeconds _waitForSeconds;
    bool _canAttack = true;
    bool _isActive;

    void Awake()
    {
        _waitForSeconds = new(cooldown);
        _isActive = true;
        GameEvents.GameOverEvent.AddListener(() => _isActive = false);
    }

    void OnCollisionStay(Collision other)
    {
        if (!_isActive || !other.collider.TryGetComponent<Player>(out var player))
        {
            return;
        }
        StartCoroutine(ContactDamageRoutine(player));
    }

    IEnumerator ContactDamageRoutine(Player player)
    {
        if (!_canAttack)
        {
            yield break;
        }

        player.Health.TakeDamage(contactDamage);

        _canAttack = false;
        yield return _waitForSeconds;
        _canAttack = true;
    }
}
