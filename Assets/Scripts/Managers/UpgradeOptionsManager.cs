using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeOptionManager : Singleton<UpgradeOptionManager>
{
    [SerializeField] SO_UpgradeOptionData optionOneData;
    [SerializeField] SO_UpgradeOptionData optionTwoData;
    [SerializeField] SO_UpgradeOptionData optionThreeData;

    List<UpgradeOption> _currentOptions = new();

    public void AssignRandom()
    {
        RefreshOptions();

        var numbers = Enumerable.Range(0, _currentOptions.Count)
            .OrderBy(_ => Random.value)
            .Take(3)
            .ToArray()
        ;

        _currentOptions[numbers[0]].AssignTo(optionOneData);
        _currentOptions[numbers[1]].AssignTo(optionTwoData);
        _currentOptions[numbers[2]].AssignTo(optionThreeData);
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
    }
}
