using UnityEngine;

public class CharacterStatOption : UpgradeOption
{
    public Stat Stat => _stat;
    Stat _stat;

    public CharacterStatOption(Stat stat)
    {
        _stat = stat;
    }

    public override void AssignTo(SO_UpgradeOptionData data)
    {
        data.SetValues(this);
    }

    public override void Select()
    {
        Player player = Game.Player;
        Stat stat = player.Stats.CharacterModifiers.GetStat(_stat.Name);
        Debug.Log($"Stat <color=magenta>{stat.Name}</color> leveled up to <color=white>{stat.LevelUp()}</color>. New evaluated value: <color=white>{stat.Evaluate()}</color>");
    }
}
