using UnityEngine.Events;

public static class GameEvents
{
    // Scene
    public static UnityEvent LevelStarted = new();
    public static UnityEvent LevelQuit = new();

    // Level
    public static UnityEvent<int, int> HealthChanged = new();
    public static UnityEvent<int, int> ExpChanged = new();
    public static UnityEvent<int, int> LvlUpEvent = new();

    public static void RaiseLevelStarted()
    {
        LevelStarted.Invoke();
    }

    public static void RaiseLevelQuit()
    {
        LevelQuit.Invoke();
    }

    public static void RaiseHealthChanged(int currentHealth, int maxHealth)
    {
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public static void RaiseExpChanged(int exp, int requiredExp)
    {
        ExpChanged?.Invoke(exp, requiredExp);
    }

    public static void RaiseLvlUpEvent(int oldLvl, int newLvl)
    {
        LvlUpEvent?.Invoke(oldLvl, newLvl);
    }
}
