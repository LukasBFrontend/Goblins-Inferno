using UnityEngine;

public static class Game
{
    static GameInputActions _input;
    static VolumeTransitionHandler _volumeTransitionHandler;
    static UpgradeOptionManager _upgradeOptionManager;
    static ObjectPooler _objectPooler;
    static EnemySpawner _enemySpawner;
    static Player _player;

    static AvatarVariant _startAvatar;
    static int _score;
    static int _highScore;

    public static GameInputActions Input => GetSafely(_input);
    public static VolumeTransitionHandler VolumeTransitions => GetSafely(_volumeTransitionHandler);
    public static UpgradeOptionManager UpgradeOptions => GetSafely(_upgradeOptionManager);
    public static ObjectPooler ObjectPooler => GetSafely(_objectPooler);
    public static EnemySpawner EnemySpawner => GetSafely(_enemySpawner);
    public static Player Player => GetSafely(_player);

    public static AvatarVariant StartAvatar => _startAvatar;
    public static int Score => _score;
    public static int HighScore => _highScore;

    public static void AddPoints(int points)
    {
        _score += points;

        if (points > _highScore)
        {
            _highScore = _score;
        }

        GameEvents.RaiseScoreChanged(_score);
    }

    public static void SetStartHighScore(int score)
    {
        _highScore = score;
    }

    public static void ResetScore()
    {
        _score = 0;
    }

    public static void SetStartAvatar(AvatarVariant variant)
    {
        _startAvatar = variant;
    }

    static T GetSafely<T>(T type) where T: MonoBehaviour
    {
        type ??= Object.FindAnyObjectByType<T>();

        if (type == null)
        {
            throw new System.NullReferenceException($"No component {nameof(T)} found in scene");
        }
        return type;
    }
}
