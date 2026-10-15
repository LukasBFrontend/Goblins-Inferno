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
    [Header("Trajectory")]
    [Range(0f, 200f)]
    public float initialVelocity;
    public ProjectileArcMode arcMode;
    [Header("Lifecycle")]
    public bool destroyOnContact;
    public float lifeTime = 5f;
    public GameObject onDestroyVFX;
    [Header("Targeting")]
    [Range(0f, .2f)]
    public float targetReachedThreshold;
    public float newTargetRange = 10f; // Replace use with weaponstat size + sizemultiplier.
    public LayerMask targetsLayerMask;

}
