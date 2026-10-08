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

        var characterStats = Game.Player.Stats.Character.All();
        var weaponsStats = Game.Player.Stats.Weapons;
        var availableWeapons = Game.Player.Weapons.Available;
        var unlockedWeapons = Game.Player.Weapons.Unlocked;

        foreach (var (weaponName, weaponStats) in weaponsStats)
        {
            // If weapon is lvl 0 create weapon unlock option and continue
            foreach (var stat in weaponStats.All())
            {
                if (stat.IncludeAsUpgradeOption)
                {
                    _currentOptions.Add(new WeaponStatOption(weaponName, stat));
                }
            }
        }

        foreach (var stat in characterStats)
        {
            if (stat.IncludeAsUpgradeOption)
            {
                _currentOptions.Add(new CharacterStatOption(stat));
            }
        }
    }
}
