using UnityEngine;

[System.Serializable]
public struct Avatars
{
    public GameObject defaultAvatar;
    public GameObject archerAvatar;
    public GameObject knightAvatar;
}

/// <summary>
/// The main player interface.
/// </summary>
[RequireComponent(typeof(Stats))]
[RequireComponent(typeof(Weapons))]
[RequireComponent(typeof(Movement))]
public class Player : BaseCharacter
{
    [Header("Player")]
    [SerializeField] Avatars avatars;
    [SerializeField] Stats stats;
    [SerializeField] Weapons weapons;
    [SerializeField] Movement movement;

    int _lvl = 1;
    int _exp = 0;
    static Player _instance;

    public Stats Stats => stats;
    public Weapons Weapons => weapons;
    public Movement Movement => movement;
    public int Lvl => _lvl;
    public int Exp => _exp;

    public void SetAvatar(PlayerAvatarVariant variant)
    {
        GameObject avatar = variant switch
        {
            PlayerAvatarVariant.Archer => avatars.archerAvatar,
            PlayerAvatarVariant.Knight => avatars.knightAvatar,
            _ => avatars.defaultAvatar,
        };

        while (RenderTransform.childCount > 0) {
            DestroyImmediate(RenderTransform.GetChild(0).gameObject);
        }

        Instantiate(avatar, RenderTransform.transform);
        Animator.Rebind();
    }

    public PlayerAvatarVariant Avatar()
    {
        Avatar avatar = Animator.avatar;

        if (avatar == avatars.archerAvatar)
        {
            return PlayerAvatarVariant.Archer;
        }
        else if (avatar == avatars.knightAvatar)
        {
            return PlayerAvatarVariant.Knight;
        }

        return PlayerAvatarVariant.Default;
    }

    /// <summary>
    /// Calculates total exp required to lvl up from the current lvl to the next.
    /// </summary>
    /// <returns>The exp</returns>
    public int ExpToLvlUp(int lvl)
    {
        return 70 + (lvl - 1) * 15;
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

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        SetAvatar(GameSaveState.Avatar.Current);
    }

    void LvlUp()
    {
        int oldLvl = _lvl;
        _lvl++;

        var levelUpSound = Game.SoundFX.Default.levelUpSound;
        Game.SoundFX.PlaySoundFXClip(levelUpSound.audioClip, transform, levelUpSound.volume, true);

        GameEvents.RaiseLvlUpEvent(oldLvl, _lvl);
    }
}
