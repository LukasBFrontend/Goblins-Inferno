using UnityEngine;

public enum ProjectileArcMode
{
    None,
    HitMiss,
    Bounce,
    Boomerang,
}

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Weapons/ProjectileData")]
public class SO_ProjectileData : ScriptableObject
{
    public ProjectileArcMode arcMode;
    public float lifeTime = 5f;
    public float range = 10f;
    [Header("Optional")]
    [Range(0f, 200f)]
    public float initialVelocity;
    [Range(0f, .2f)]
    public float targetReachedThreshold;
}
