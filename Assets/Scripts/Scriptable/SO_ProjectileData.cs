using UnityEngine;

public enum ProjectileArcMode
{
    None,
    Track,
    Bounce,
    Boomerang,
}

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Weapons/ProjectileData")]
public class SO_ProjectileData : ScriptableObject
{
    [Range(.1f, 200f)]
    public float initialVelocity;
    public ProjectileArcMode arcMode;
    [Range(0f, .2f)]
    public float targetReachedThreshold;
    public float lifeTime = 5f;
    public float range = 10f;
}
