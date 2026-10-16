using UnityEngine.Events;

public static class GameEvents
{
    public static UnityEvent MainMenuLoaded = new();
    public static UnityEvent LevelLoaded = new();
    public static UnityEvent LevelQuit = new();
    public static UnityEvent GamePaused = new();
    public static UnityEvent GameResumed = new();
    public static UnityEvent GameOverEvent =  new();

    public static UnityEvent<int, int> HealthChanged = new();
    public static UnityEvent<int, int> ExpChanged = new();
    public static UnityEvent<int> ScoreChanged = new();
    public static UnityEvent<int, int> LvlUpEvent = new();

    public static void RaiseMainMenuLoaded()
    {
        MainMenuLoaded.Invoke();
    }

    public static void RaiseLevelLoaded()
    {
        LevelLoaded.Invoke();
    }

    public static void RaiseLevelQuit()
    {
        LevelQuit.Invoke();
    }

    public static void RaiseGamePaused()
    {
        GamePaused.Invoke();
    }

    public static void RaiseGameResumed()
    {
        GameResumed.Invoke();
    }

    public static void RaiseGameOverEvent()
    {
        GameOverEvent.Invoke();
    }

    public static void RaiseHealthChanged(int currentHealth, int maxHealth)
    {
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public static void RaiseExpChanged(int exp, int requiredExp)
    {
        ExpChanged?.Invoke(exp, requiredExp);
    }

    public static void RaiseScoreChanged(int newScore)
    {
        ScoreChanged?.Invoke(newScore);
    }

    public static void RaiseLvlUpEvent(int oldLvl, int newLvl)
    {
        LvlUpEvent?.Invoke(oldLvl, newLvl);
    }
}
