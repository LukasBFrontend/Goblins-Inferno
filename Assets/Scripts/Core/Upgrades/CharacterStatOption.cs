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
        Player player = Player.Instance;
        Stat stat = player.Stats.CharacterModifiers.GetStat(_stat.Name);
        Debug.Log($"Stat '{stat.Name}' leveled up to {stat.LevelUp()}. New value: {stat.Evaluate()}");
    }
}
