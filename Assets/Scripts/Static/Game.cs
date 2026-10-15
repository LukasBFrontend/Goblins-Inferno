using UnityEngine;

public static class Game
{
    static GameInputActions _input;
    static UpgradeOptionManager _upgradeOptionManager;
    static ObjectPooler _objectPoolManager;
    static EnemySpawner _enemySpawner;
    static Player _player;

    static AvatarVariant _startAvatar;
    static int _score;
    static int _highScore;

    public static GameInputActions Input => GetSafe(_input);
    public static UpgradeOptionManager UpgradeOption => GetSafe(_upgradeOptionManager);
    public static ObjectPooler ObjectPool => GetSafe(_objectPoolManager);
    public static EnemySpawner EnemySpawner => GetSafe(_enemySpawner);
    public static Player Player => GetSafe(_player);

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
