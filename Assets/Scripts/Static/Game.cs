using UnityEngine;

public static class Game
{
    static GameInputActions _input;
    static UpgradeOptionManager _upgradeOptionManager;
    static Player _player;
    
    static AvatarVariant _startAvatar;
    static float _score;
    static float _highScore;

    public static GameInputActions Input => GetSafe(_input);
    public static UpgradeOptionManager UpgradeOptionManager => GetSafe(_upgradeOptionManager);
    public static Player Player => GetSafe(_player);

    public static AvatarVariant StartAvatar => _startAvatar;
    public static float Score => _score;
    public static float HighScore => _highScore;

    public static void SetScore(float score)
    {
        _score = score;

        if (score > _highScore)
        {
            _highScore = score;
        }
    }

    public static void SetStartHighScore(float score)
    {
        _highScore = score;
    }

    public static void SetStartAvatar(AvatarVariant variant)
    {
        _startAvatar = variant;
    }

    static T GetSafe<T>(T type) where T: MonoBehaviour
    {
        type ??= Object.FindAnyObjectByType<T>();

        if (type == null)
        {
            throw new System.NullReferenceException($"No component {nameof(T)} found in scene");
        }
        return type;
    }
}
