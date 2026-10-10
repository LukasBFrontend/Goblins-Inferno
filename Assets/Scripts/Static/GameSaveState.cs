using System.IO;
using UnityEngine;

public static class GameSaveState
{
    static SaveState gameSaveState = new();
    static string LocalPath => Application.persistentDataPath + "/savegame.json";

    public static void Save(int score, AvatarVariant selectedAvatar)
    {
        if (score > gameSaveState.highScore)
        {
            gameSaveState.highScore = score;
        }
        gameSaveState.avatar = selectedAvatar;

        var savegameJson = JsonUtility.ToJson(gameSaveState);
        File.WriteAllTextAsync(Application.persistentDataPath + "/savegame.json", savegameJson);
    }

    public static void Load()
    {
        if (!PathExists())
        {
            CreateDefault();
            return;
        }

        var savegameJson = File.ReadAllText(LocalPath);
        gameSaveState = JsonUtility.FromJson<SaveState>(savegameJson);

        SetGame();
    }

    static void SetGame()
    {
        Game.SetStartAvatar(gameSaveState.avatar);
        Game.SetStartHighScore(gameSaveState.highScore);
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

    static void CreateDefault()
    {
        Save(0, AvatarVariant.Default);
    }
}

public enum AvatarVariant
{
    Default,
    Archer,
    Knight,
}

public struct SaveState
{
    public AvatarVariant avatar;
    public int highScore;
}
