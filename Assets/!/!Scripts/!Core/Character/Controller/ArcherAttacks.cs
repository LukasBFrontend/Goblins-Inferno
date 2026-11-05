using UnityEngine;
using Luscious.Audio;

[RequireComponent(typeof(Enemy))]
public class ArcherAttacks : MonoBehaviour
{
    [Header("Shoot Arrow")]
    [SerializeField] PoolableData arrowProjectile;
    [SerializeField] Transform leftArrowSpawnPoint;
    [SerializeField] Transform rightArrowSpawnPoint;
    [SerializeField] int damage;
    [SerializeField] MultipleSoundData shootSound;

    private int _lastSoundIndex;
    private Enemy _archer;

    public void ShootArrowLeft()
    {
        ShootArrow(leftArrowSpawnPoint);
    }

    public void ShootArrowRight()
    {
        ShootArrow(rightArrowSpawnPoint);
    }

    private void Awake()
    {
        _archer = GetComponent<Enemy>();
    }

    private void ShootArrow(Transform origin)
    {
        Projectile projectile = Game.ObjectPooler.Spawn<Projectile>(arrowProjectile, origin.position, Quaternion.identity);
        projectile.Initialize(_archer, Game.Player, damage);

        Game.SoundFX.PlayRandomSoundFXClip(shootSound, origin.position, _lastSoundIndex, out int newSoundIndex, projectile.transform);
        _lastSoundIndex = newSoundIndex;
    }
}
