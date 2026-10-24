using UnityEngine;

public static class Game
{
    static Player _player;

    public static GameInputActions Input => GetSingleton<GameInputActions>();
    public static VolumeTransitionHandler VolumeTransitions => GetSingleton<VolumeTransitionHandler>();
    public static UpgradeOptionManager UpgradeOptions => GetSingleton<UpgradeOptionManager>();
    public static ObjectPooler ObjectPooler => GetSingleton<ObjectPooler>();
    public static EnemySpawner EnemySpawner => GetSingleton<EnemySpawner>();

    public static Player Player => GetFirstOrDefault(_player);

    static T GetFirstOrDefault<T>(T type) where T: MonoBehaviour
    {
        if (type == null)
        {
            type ??= Object.FindAnyObjectByType<T>();
        }

        if (type == null)
        {
            throw new System.NullReferenceException($"No component {nameof(T)} found in scene");
        }

        return type;
    }

    static T GetSingleton<T>() where T: Singleton<T>
    {
        return Singleton<T>.GetInstance();
    }
}
