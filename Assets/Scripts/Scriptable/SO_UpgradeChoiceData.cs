using UnityEngine;
using Unity.Properties;
using UnityEngine.UIElements;

[System.Serializable]
public enum StatScalingType
{
    Additive,
    Multiplicative,
}

public enum UpgradeType
{
    CharacterStat,
    WeaponUnlock,
    WeaponStat,
}

[CreateAssetMenu(fileName = "UpgradeChoiceData", menuName = "Runtime/UpgradeChoiceData")]
public class SO_UpgradeOptionData : ScriptableObject
{
    [Header("Card data")]
    public UpgradeType upgradeType;
    public Sprite sprite;
    public string title;
    public string metaDescription;
    [Header("Stat data")]
    public StatScalingType scalingType;
    public string statname;
    public float statIncrease;
    public int lvl;
    [Header("Default value")]
    [SerializeField] SO_UpgradeOptionData defaultValues;
    UpgradeOption _upgradeOptionRef;

    // Scaling type (Additive / Multiplicative)
    [CreateProperty]
    public StyleEnum<DisplayStyle> DisplayAddativeStatType => scalingType == StatScalingType.Additive ? DisplayStyle.Flex : DisplayStyle.None;
    [CreateProperty]
    public StyleEnum<DisplayStyle> DisplayMultiplicativeStatType => scalingType == StatScalingType.Multiplicative ? DisplayStyle.Flex : DisplayStyle.None;

    // Upgrade type (Character / Weapon / WeaponUnlock)
    public StyleEnum<DisplayStyle> DisplayCharacter => upgradeType == UpgradeType.CharacterStat ? DisplayStyle.Flex : DisplayStyle.None;
    public StyleEnum<DisplayStyle> DisplayWeapon => upgradeType == UpgradeType.WeaponStat ? DisplayStyle.Flex : DisplayStyle.None;
    public StyleEnum<DisplayStyle> DisplayWeaponUnlock => upgradeType == UpgradeType.WeaponUnlock ? DisplayStyle.Flex : DisplayStyle.None;

    public void SetValues(CharacterStatOption optionRef)
    {
        upgradeType = UpgradeType.CharacterStat;

        _upgradeOptionRef = optionRef;
        Stat stat = optionRef.Stat;

        scalingType = stat is AdditiveStat
            ? StatScalingType.Additive
            : StatScalingType.Multiplicative
        ;

        sprite = stat.Sprite;
        title = stat.Name;
        statname = stat.ShortName;
        statIncrease = stat.PerLevelIncrease;
        lvl = stat.Lvl + 1;
    }

    public void SetValues(WeaponUnlockOption optionRef)
    {
        _upgradeOptionRef = optionRef;

        upgradeType = UpgradeType.WeaponUnlock;
        title = optionRef.WeaponName;
        lvl = 1;
    }

    public void SetValues(WeaponStatOption optionRef)
    {
        _upgradeOptionRef = optionRef;

        upgradeType = UpgradeType.WeaponStat;
        title = optionRef.WeaponName;
        Stat stat = optionRef.Stat;

        scalingType = stat is AdditiveStat
            ? StatScalingType.Additive
            : StatScalingType.Multiplicative
        ;

        sprite = stat.Sprite;
        statname = stat.ShortName;
        statIncrease = stat.PerLevelIncrease;
        lvl = stat.Lvl + 1;
    }

    public void Select()
    {
        _upgradeOptionRef.Select();
    }
}
