using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Profiling;

public class UpgradeOptionManager : Singleton<UpgradeOptionManager>
{
    static readonly ProfilerMarker RefreshOptionsMarker = new("UpgradeOptions.RefreshOptions");
    static readonly ProfilerMarker AssignOptionsMarker = new("UpgradeOptions.AssignOptions");
    [SerializeField] SO_UpgradeOptionData optionOneData;
    [SerializeField] SO_UpgradeOptionData optionTwoData;
    [SerializeField] SO_UpgradeOptionData optionThreeData;

    List<UpgradeOption> _currentOptions = new();

    public void AssignRandom()
    {
        using (RefreshOptionsMarker.Auto())
        {
            RefreshOptions();
        }

        using (AssignOptionsMarker.Auto())
        {
            int count = _currentOptions.Count;
            int first = Random.Range(0, count);

            int second = Random.Range(0, count - 1);

            if (second >= first)
            {
                second++;
            }

            int third = Random.Range(0, count - 2);
            int lower = Mathf.Min(first, second);
            int higher = Mathf.Max(first, second);

            if (third >= lower)
            {
                third++;
            }

            if (third >= higher)
            {
                third++;
            }

            _currentOptions[first].AssignTo(optionOneData);
            _currentOptions[second].AssignTo(optionTwoData);
            _currentOptions[third].AssignTo(optionThreeData);
        }
    }

    public void SelectOptionOne()
    {
        optionOneData.Select();
    }

    public void SelectOptionTwo()
    {
        optionTwoData.Select();
    }

    public void SelectOptionThree()
    {
        optionThreeData.Select();
    }

    void Start()
    {
        AssignRandom();
    }

    void RefreshOptions()
    {
        _currentOptions.Clear();

        Stat[] characterStats = Game.Player.Stats.Character.All();
        Dictionary<string, WeaponStats> weaponsStats = Game.Player.Stats.Weapons;
        HashSet<Weapon> availableWeapons = Game.Player.Weapons.Available;
        HashSet<Weapon> unlockedWeapons = Game.Player.Weapons.Unlocked;

        foreach ((string weaponName, WeaponStats weaponStats) in weaponsStats)
        {
            if (!unlockedWeapons.Any(weapon => weapon.Name == weaponName))
            {
                Weapon weapon = availableWeapons.First(weapon => weapon.Name == weaponName);
                _currentOptions.Add(new WeaponUnlockOption(weaponName, weapon.Data.sprite));

                continue;
            }

            foreach (Stat stat in weaponStats.All())
            {
                if (stat.IncludeAsUpgradeOption)
                {
                    _currentOptions.Add(new WeaponStatOption(weaponName, stat));
                }
            }
        }

        foreach (Stat stat in characterStats)
        {
            if (stat.IncludeAsUpgradeOption)
            {
                _currentOptions.Add(new CharacterStatOption(stat));
            }
        }

        Debug.Log(
            $"Upgrade options created: {_currentOptions.Count} " +
            $"(weapons: {Game.Player.Stats.Weapons.Count}, " +
            $"character stats: {Game.Player.Stats.Character.All().Length})")
        ;
    }
}
