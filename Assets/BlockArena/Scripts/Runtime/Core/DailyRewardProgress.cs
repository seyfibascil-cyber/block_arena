using System;
using UnityEngine;

public static class DailyRewardProgress
{
    private const string LastClaimKey = "BlockArena.DailyReward.LastClaim";
    private const string StreakKey = "BlockArena.DailyReward.Streak";

    private static readonly int[] Rewards =
    {
        25, 30, 40, 50, 65, 80, 100
    };

    public static int CurrentStreak => Mathf.Clamp(
        PlayerPrefs.GetInt(StreakKey, 0),
        0,
        Rewards.Length
    );

    public static bool IsAvailable(DateTime utcNow)
    {
        if (!TryGetLastClaimDate(out DateTime lastClaim))
        {
            return true;
        }

        return utcNow.Date > lastClaim.Date;
    }

    public static int GetNextReward(DateTime utcNow)
    {
        int nextStreak = GetNextStreak(utcNow);
        return Rewards[nextStreak - 1];
    }

    public static int Claim(DateTime utcNow)
    {
        DateTime safeNow = utcNow.ToUniversalTime();
        if (!IsAvailable(safeNow))
        {
            return 0;
        }

        int newStreak = GetNextStreak(safeNow);
        int reward = Rewards[newStreak - 1];

        PlayerPrefs.SetString(
            LastClaimKey,
            safeNow.Date.ToString("yyyy-MM-dd")
        );
        PlayerPrefs.SetInt(StreakKey, newStreak);
        PlayerPrefs.Save();
        EconomyProgress.GrantCoins(reward);
        return reward;
    }

    private static int GetNextStreak(DateTime utcNow)
    {
        if (!TryGetLastClaimDate(out DateTime lastClaim))
        {
            return 1;
        }

        int days = (utcNow.Date - lastClaim.Date).Days;
        if (days == 1)
        {
            return CurrentStreak >= Rewards.Length
                ? 1
                : CurrentStreak + 1;
        }

        return days <= 0 ? Mathf.Max(1, CurrentStreak) : 1;
    }

    private static bool TryGetLastClaimDate(out DateTime date)
    {
        return DateTime.TryParseExact(
            PlayerPrefs.GetString(LastClaimKey, ""),
            "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.AssumeUniversal,
            out date
        );
    }
}
