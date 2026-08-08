using UnityEngine;

public static class EconomyProgress
{
    public const int FirstCompletionCoins = 50;
    public const int ReplayCompletionCoins = 10;
    public const int MaximumStarsPerLevel = 3;

    private const string CoinsKey = "BlockArena.Economy.Coins";
    private const string StarsPrefix = "BlockArena.Economy.LevelStars.";
    private const string FailedPrefix = "BlockArena.Economy.LevelFailed.";

    public static int Coins => Mathf.Max(0, PlayerPrefs.GetInt(CoinsKey, 0));

    public static int LastCoinsEarned { get; private set; }
    public static int LastStarsEarned { get; private set; }

    public static void ResetLastReward()
    {
        LastCoinsEarned = 0;
        LastStarsEarned = 0;
    }

    public static bool TrySpendCoins(int amount)
    {
        int safeAmount = Mathf.Max(0, amount);
        if (Coins < safeAmount)
        {
            return false;
        }

        PlayerPrefs.SetInt(CoinsKey, Coins - safeAmount);
        PlayerPrefs.Save();
        return true;
    }

    public static void GrantCoins(int amount)
    {
        int safeAmount = Mathf.Max(0, amount);
        PlayerPrefs.SetInt(CoinsKey, Coins + safeAmount);
        PlayerPrefs.Save();
    }

    public static int TotalStars
    {
        get
        {
            int total = 0;
            for (int level = 1; level <= GameProgression.MaximumLevel; level++)
            {
                total += GetLevelStars(level);
            }

            return total;
        }
    }

    public static int GetLevelStars(int level)
    {
        int safeLevel = Mathf.Clamp(level, 1, GameProgression.MaximumLevel);
        return Mathf.Clamp(
            PlayerPrefs.GetInt(StarsPrefix + safeLevel, 0),
            0,
            MaximumStarsPerLevel
        );
    }

    public static void RecordCurrentLevelDefeat()
    {
        if (!TryGetCurrentLevel(out int level))
        {
            return;
        }

        PlayerPrefs.SetInt(FailedPrefix + level, 1);
        PlayerPrefs.Save();
    }

    public static void RewardCurrentLevelCompletion()
    {
        ResetLastReward();

        if (!TryGetCurrentLevel(out int level))
        {
            return;
        }

        int previousStars = GetLevelStars(level);
        bool firstCompletion = previousStars == 0;
        bool wonOnFirstAttempt =
            PlayerPrefs.GetInt(FailedPrefix + level, 0) == 0;

        // 1: bölümü bitir, 2: yardım kullanmadan bitir,
        // 3: hiç kaybetmeden ilk denemede bitir.
        int earnedStars = wonOnFirstAttempt ? 3 : 2;
        int newStars = Mathf.Max(previousStars, earnedStars);
        LastStarsEarned = Mathf.Max(0, newStars - previousStars);
        LastCoinsEarned = firstCompletion
            ? FirstCompletionCoins
            : ReplayCompletionCoins;

        PlayerPrefs.SetInt(StarsPrefix + level, newStars);
        PlayerPrefs.SetInt(CoinsKey, Coins + LastCoinsEarned);
        PlayerPrefs.Save();
    }

    private static bool TryGetCurrentLevel(out int level)
    {
        GameProgression.GameMode mode =
            (GameProgression.GameMode)PlayerPrefs.GetInt(
                GameProgression.GameModeKey,
                (int)GameProgression.GameMode.Standard
            );
        level = PlayerPrefs.GetInt(GameProgression.SelectedLevelKey, 0);

        return mode == GameProgression.GameMode.Levels &&
               level >= 1 &&
               level <= GameProgression.MaximumLevel;
    }
}
