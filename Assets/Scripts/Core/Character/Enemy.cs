using UnityEngine;
using System.Collections;

public sealed class Enemy : BaseCharacter
{
    private static WaitForSeconds _waitForSeconds_5 = new(.5f);
    [SerializeField] Animator animator;
    [Tooltip("How much damage does the enemy deal on player collision?")]
    [Range(1, 100)]
    [SerializeField] int contactDamage;
    bool _canAttack = true;
    public Animator Animator => animator;

    IEnumerator AttackCooldown()
    {
        _canAttack = false;
        yield return _waitForSeconds_5;
        _canAttack = true;
    }
    void OnCollisionStay(Collision other)
    {
        if (!other.collider.TryGetComponent<Player>(out var player) || !_canAttack)
        {
            return;
        }

        player.Health.TakeDamage(contactDamage);

        StartCoroutine(AttackCooldown());
    }


    public override void Die()
    {
        Destroy(gameObject);
    }
}
