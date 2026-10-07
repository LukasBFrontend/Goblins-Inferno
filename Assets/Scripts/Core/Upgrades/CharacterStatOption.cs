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
        Stat stat = player.Stats.Character.GetStat(_stat.Name);
        stat.LevelUp();
        Debug.Log($"Stat <color=magenta>{stat.Name}</color> leveled up to <color=white>{stat.Lvl}</color>. New evaluated value: <color=white>{stat.Evaluate()}</color>");
    }
}
