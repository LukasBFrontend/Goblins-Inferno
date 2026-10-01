using UnityEngine;

public static class Game
{
    static GameInputActions _input;
    public static GameInputActions Input => GetSafe(_input);

    static T GetSafe<T>(T type) where T: MonoBehaviour
    {
        type ??= Object.FindAnyObjectByType<T>() ?? GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<T>();

        if (type == null)
        {
            throw new System.NullReferenceException($"No compnent {nameof(T)} found in scene");
        }
        return type;
    }
}
