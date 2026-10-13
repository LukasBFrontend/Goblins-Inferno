using UnityEngine;

public enum ProjectileArcMode
{
    None,
    HitMiss,
    Physics, // Currently specific to bomb weapon.
    Boomerang,
}

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Weapons/ProjectileData")]
public class SO_ProjectileData : ScriptableObject
{
    public ProjectileArcMode arcMode;
    public float lifeTime = 5f;
    public float newTargetRange = 10f; // Replace use with weaponstat size + sizemultiplier.
    public bool destroyOnContact;
    [Header("Optional")]
    [Range(0f, 200f)]
    public float initialVelocity;
    [Range(0f, .2f)]
    public float targetReachedThreshold;
    public GameObject onDestroyVFX;
}
