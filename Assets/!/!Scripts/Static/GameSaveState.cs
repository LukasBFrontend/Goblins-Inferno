using System.IO;
using UnityEngine;

public static class GameSaveState
{
    static SaveState gameSaveState = new();
    static string LocalPath => Application.persistentDataPath + "/savegame.json";

    static bool _hasBeenLoaded;

    public static bool HasBeenLoaded => _hasBeenLoaded;
    public static PlayerAvatar Avatar;
    public static PlayerScore Score;

    public static void Save()
    {
        var savegameJson = JsonUtility.ToJson(gameSaveState);
        File.WriteAllText(Application.persistentDataPath + "/savegame.json", savegameJson);
    }

    public static void Load()
    {
        if (!PathExists())
        {
            Save();
            return;
        }

        var savegameJson = File.ReadAllText(LocalPath);
        gameSaveState = JsonUtility.FromJson<SaveState>(savegameJson);

        Score = new(gameSaveState.highScore);
        Avatar = new(gameSaveState.avatar);

        _hasBeenLoaded = true;
    }

    static bool PathExists()
    {
        try
        {
            FileAttributes attributes = File.GetAttributes(LocalPath);

            return attributes == FileAttributes.Directory
                ? Directory.Exists(LocalPath)
                : File.Exists(LocalPath)
            ;
        }
        catch (FileNotFoundException)
        {
            return false;
        }
    }


    public struct PlayerAvatar
    {
        public PlayerAvatarVariant Current { get; private set; }

        public PlayerAvatar(PlayerAvatarVariant variant)
        {
            Current = variant;
        }

        public void Set(PlayerAvatarVariant avatarVariant)
        {
            Current = avatarVariant;
            gameSaveState.avatar = avatarVariant;
        }
    }

    public class PlayerScore
    {
        public int Current { get; private set; }
        public int HighScore { get; private set; }

        public PlayerScore(int highScore)
        {
            Current = 0;
            HighScore = highScore;
        }

        public void AddPoints(int points)
        {
            Current += points;

            GameEvents.RaiseScoreChanged(Current);
        }

        public void Reset()
        {
            Current = 0;
        }

        public void SetHighScore(int highScore)
        {
            HighScore = highScore;
            gameSaveState.highScore = highScore;
        }
    }
}

public enum PlayerAvatarVariant
{
    Default,
    Archer,
    Knight,
}

public struct SaveState
{
    public PlayerAvatarVariant avatar;
    public int highScore;
}
