using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponsConfig", menuName = "Weapons/Config")]
public class SO_WeaponsConfig : ScriptableObject
{
    public List<SO_WeaponData> availableWeapons;
}
