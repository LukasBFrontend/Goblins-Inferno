using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStatsConfig", menuName = "Stats/CharacterStatsConfig")]
public class SO_CharacterStatsConfig : ScriptableObject
{
    public AdditiveStatData maxHealth;
    public MultiplicativeStatData damageMultiplier;
    public MultiplicativeStatData healthMultiplier;
    public MultiplicativeStatData movementMultiplier;
    public MultiplicativeStatData attackSpeedMultiplier;
}
