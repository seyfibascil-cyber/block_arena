using UnityEngine;

public static class GameProgression
{
    public const int MaximumLevel = 150;
    public const string DifficultyKey = "BlockArena.Difficulty";
    public const string GameModeKey = "BlockArena.GameMode";
    public const string SelectedLevelKey = "BlockArena.SelectedLevel";
    public const string OpenLevelsMenuKey = "BlockArena.OpenLevelsMenu";
    public const string TutorialSeenKey = "BlockArena.TutorialSeen";

    private const string UnlockedLevelKey = "BlockArena.UnlockedLevel";
    private const string CompletedLevelKey = "BlockArena.CompletedLevel";

    public enum GameMode
    {
        Standard,
        Levels,
        Pvp
    }

    public static int UnlockedLevel => Mathf.Clamp(
        PlayerPrefs.GetInt(UnlockedLevelKey, 1),
        1,
        MaximumLevel
    );

    public static int HighestCompletedLevel => Mathf.Clamp(
        PlayerPrefs.GetInt(
            CompletedLevelKey,
            Mathf.Max(0, UnlockedLevel - 1)
        ),
        0,
        MaximumLevel
    );

    public static void StartStandardGame(
        AIController.Difficulty difficulty
    )
    {
        PlayerPrefs.SetInt(GameModeKey, (int)GameMode.Standard);
        PlayerPrefs.SetInt(DifficultyKey, (int)difficulty);
        PlayerPrefs.SetInt(SelectedLevelKey, 0);
        PlayerPrefs.Save();
    }

    public static void StartLevel(int levelNumber)
    {
        int safeLevel = Mathf.Clamp(levelNumber, 1, UnlockedLevel);

        PlayerPrefs.SetInt(GameModeKey, (int)GameMode.Levels);
        PlayerPrefs.SetInt(SelectedLevelKey, safeLevel);
        PlayerPrefs.SetInt(
            DifficultyKey,
            (int)GetDifficultyForLevel(safeLevel)
        );
        PlayerPrefs.Save();
    }

    public static void StartPvpBotGame()
    {
        PlayerPrefs.SetInt(GameModeKey, (int)GameMode.Pvp);
        PlayerPrefs.SetInt(
            DifficultyKey,
            (int)AIController.Difficulty.Impossible
        );
        PlayerPrefs.SetInt(SelectedLevelKey, 0);
        PlayerPrefs.SetInt("BlockArena.PvpOpponentIsBot", 1);
        PlayerPrefs.Save();
    }

    public static void CompleteCurrentLevel()
    {
        EconomyProgress.ResetLastReward();

        GameMode mode = (GameMode)PlayerPrefs.GetInt(
            GameModeKey,
            (int)GameMode.Standard
        );

        if (mode != GameMode.Levels)
        {
            return;
        }

        EconomyProgress.RewardCurrentLevelCompletion();

        int completedLevel = PlayerPrefs.GetInt(SelectedLevelKey, 1);
        int nextLevel = Mathf.Min(completedLevel + 1, MaximumLevel);

        if (completedLevel > HighestCompletedLevel)
        {
            PlayerPrefs.SetInt(CompletedLevelKey, completedLevel);
        }

        if (nextLevel > UnlockedLevel)
        {
            PlayerPrefs.SetInt(UnlockedLevelKey, nextLevel);
        }

        PlayerPrefs.Save();
    }

    public static AIController.Difficulty GetDifficultyForLevel(
        int levelNumber
    )
    {
        if (levelNumber <= 20)
        {
            return AIController.Difficulty.Easy;
        }

        if (levelNumber <= 40)
        {
            return AIController.Difficulty.Medium;
        }

        if (levelNumber <= 60)
        {
            return AIController.Difficulty.Hard;
        }

        return AIController.Difficulty.Impossible;
    }
}
