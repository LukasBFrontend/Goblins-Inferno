using UnityEngine;

public static class Game
{
    // Components are assumed to implement singleton pattern
    static GameInputActions _input;
    static Player _player;
    static UpgradeOptionManager _upgradeOptionManager;
    public static GameInputActions Input => GetSafe(_input);
    public static Player Player => GetSafe(_player);
    public static UpgradeOptionManager UpgradeOptionManager => GetSafe(_upgradeOptionManager);

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
