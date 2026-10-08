using UnityEngine;

[System.Serializable]
public struct Avatars
{
    public Avatar defaultAvatar;
    public Avatar archerAvatar;
    public Avatar knightAvatar;
}

/// <summary>
/// The main player interface.
/// </summary>
[RequireComponent(typeof(Stats))]
[RequireComponent(typeof(Weapons))]
[RequireComponent(typeof(Movement))]
public class Player : BaseCharacter
{
    [SerializeField] Avatars avatars;
    [SerializeField] Stats stats;
    [SerializeField] Weapons weapons;
    [SerializeField] Movement movement;
    public Stats Stats => stats;
    public Weapons Weapons => weapons;
    public Movement Movement => movement;
    public int Lvl => _lvl;
    public int Exp => _exp;
    int _lvl = 1;
    int _exp = 0;
    static Player _instance;

    public void SetAvatar(AvatarVariant variant)
    {
        Animator.avatar = variant switch
        {
            AvatarVariant.Archer => avatars.archerAvatar,
            AvatarVariant.Knight => avatars.knightAvatar,
            _ => avatars.defaultAvatar,
        };
    }

    public AvatarVariant Avatar()
    {
        Avatar avatar = Animator.avatar;

        if (avatar == avatars.archerAvatar)
        {
            return AvatarVariant.Archer;
        }
        else if (avatar == avatars.knightAvatar)
        {
            return AvatarVariant.Knight;
        }
        
        return AvatarVariant.Default;
    }

    /// <summary>
    /// Calculates total exp required to lvl up from the current lvl to the next.
    /// </summary>
    /// <returns>The exp</returns>
    public int ExpToLvlUp(int lvl)
    {
        return 100 + (lvl - 1) * 10;
    }

    /// <summary>
    /// Adds exp to the player exp and invokes LvlUp() if the updated exp is sufficient. Cannot handle multiple level ups in the same call.
    /// </summary>
    /// <param name="amount">The experience amount to gain.</param>
    public void GainExp(int amount)
    {
        int requiredExp = ExpToLvlUp(_lvl);

        _exp += amount;

        if (_exp < requiredExp)
        {
            GameEvents.RaiseExpChanged(_exp, requiredExp);
            return;
        }
        int leftover = _exp - requiredExp;

        _exp = leftover;

        LvlUp();
        int newRequiredExp = ExpToLvlUp(_lvl);
        GameEvents.RaiseExpChanged(_exp, newRequiredExp);
    }

    public override void Die()
    {
        GameEvents.RaiseGameOverEvent();
        Animator.SetBool("IsDead", true);
    }

    private void LvlUp()
    {
        int oldLvl = _lvl;
        _lvl++;
        GameEvents.RaiseLvlUpEvent(oldLvl, _lvl);
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        SetAvatar(Game.StartAvatar);
    }
}
