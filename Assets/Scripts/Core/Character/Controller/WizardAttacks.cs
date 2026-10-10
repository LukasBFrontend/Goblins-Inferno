using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class WizardAttacks : MonoBehaviour
{
    [Header("Energy Explosion")]
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] int damage;
    Enemy _archer;

    void Awake()
    {
        _archer = GetComponent<Enemy>();
    }

    public void StartChannelingExplosion()
    {
        StartExplosionSequence(Game.Player.transform);
    }

    private void StartExplosionSequence(Transform origin)
    {
        GameObject explosionInstance = Instantiate(explosionPrefab, origin.position, Quaternion.identity);

        Projectile projectile = explosionInstance.GetComponent<Projectile>();
        projectile.Initialize(_archer, Game.Player, damage);
    }
}
