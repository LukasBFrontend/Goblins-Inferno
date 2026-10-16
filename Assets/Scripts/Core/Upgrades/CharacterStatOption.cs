using UnityEngine;

public class CharacterStatOption : UpgradeOption
{
    Stat _stat;

    public Stat Stat => _stat;

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

        Stat maxHealth = player.Stats.Character.MaxHealth;
        Stat healthMultiplier = player.Stats.Character.HealthMultiplier;

        if (stat == maxHealth || stat == healthMultiplier)
        {
            int newMaxHealth = (int)(maxHealth.Evaluate() * healthMultiplier.Evaluate());
            player.Health.SetMaxHealth(newMaxHealth);
        }
    }
}
