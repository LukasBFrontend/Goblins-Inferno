using UnityEngine;

public static class WeaponsUtils
{
    public static int RollCritModifier(int critChance, out int critLevel)
    {
        critLevel = critChance / 100;

        int damageCritModifier = critLevel + 1;

        if (Random.Range(0, 100) < (critChance % 100))
        {
            damageCritModifier += 1;
        }

        return damageCritModifier;
    }
}