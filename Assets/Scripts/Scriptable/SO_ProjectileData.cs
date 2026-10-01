using UnityEngine;

public enum ProjectileArcMode
{
    Simple,
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
}
