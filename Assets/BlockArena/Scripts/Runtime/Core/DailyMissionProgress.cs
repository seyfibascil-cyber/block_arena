using System;
using UnityEngine;

public enum DailyMissionType
{
    PlayMatches,
    WinMatches,
    WinCampaignMatch,
    PlaceObstacles
}

public readonly struct DailyMissionState
{
    public DailyMissionState(
        DailyMissionType type,
        string title,
        int progress,
        int target,
        int reward,
        bool claimed
    )
    {
        Type = type;
        Title = title;
        Progress = progress;
        Target = target;
        Reward = reward;
        Claimed = claimed;
    }

    public DailyMissionType Type { get; }
    public string Title { get; }
    public int Progress { get; }
    public int Target { get; }
    public int Reward { get; }
    public bool Claimed { get; }
    public bool IsComplete => Progress >= Target;
}

public static class DailyMissionProgress
{
    private const string DateKey = "BlockArena.Missions.Date";
    private const string PlaysKey = "BlockArena.Missions.Plays";
    private const string WinsKey = "BlockArena.Missions.Wins";
    private const string CampaignWinsKey = "BlockArena.Missions.CampaignWins";
    private const string ObstaclesKey = "BlockArena.Missions.Obstacles";
    private const string ClaimedPrefix = "BlockArena.Missions.Claimed.";
    private const string BonusClaimedKey = "BlockArena.Missions.BonusClaimed";

    public static void RecordCompletedMatch(bool won, bool campaign)
    {
        EnsureCurrentDay(DateTime.UtcNow);
        PlayerPrefs.SetInt(PlaysKey, PlayerPrefs.GetInt(PlaysKey, 0) + 1);

        if (won)
        {
            PlayerPrefs.SetInt(WinsKey, PlayerPrefs.GetInt(WinsKey, 0) + 1);
            if (campaign)
            {
                PlayerPrefs.SetInt(
                    CampaignWinsKey,
                    PlayerPrefs.GetInt(CampaignWinsKey, 0) + 1
                );
            }
        }

        PlayerPrefs.Save();
    }

    public static void RecordPlacedObstacle()
    {
        EnsureCurrentDay(DateTime.UtcNow);
        PlayerPrefs.SetInt(
            ObstaclesKey,
            PlayerPrefs.GetInt(ObstaclesKey, 0) + 1
        );
        PlayerPrefs.Save();
    }

    public static DailyMissionState[] GetMissions(DateTime utcNow)
    {
        EnsureCurrentDay(utcNow);
        return new[]
        {
            Create(DailyMissionType.PlayMatches, "3 MAÇ TAMAMLA", PlaysKey, 3, 40),
            Create(DailyMissionType.WinMatches, "2 MAÇ KAZAN", WinsKey, 2, 60),
            Create(DailyMissionType.WinCampaignMatch, "1 BÖLÜM KAZAN", CampaignWinsKey, 1, 50),
            Create(DailyMissionType.PlaceObstacles, "10 ENGEL YERLEŞTİR", ObstaclesKey, 10, 75)
        };
    }

    public static bool TryClaim(DailyMissionType type, DateTime utcNow)
    {
        foreach (DailyMissionState mission in GetMissions(utcNow))
        {
            if (mission.Type != type || !mission.IsComplete || mission.Claimed)
            {
                continue;
            }

            PlayerPrefs.SetInt(ClaimedPrefix + type, 1);
            PlayerPrefs.Save();
            EconomyProgress.GrantCoins(mission.Reward);
            return true;
        }

        return false;
    }

    public static bool CanClaimDailyBonus(DateTime utcNow)
    {
        if (PlayerPrefs.GetInt(BonusClaimedKey, 0) == 1)
        {
            return false;
        }

        foreach (DailyMissionState mission in GetMissions(utcNow))
        {
            if (!mission.Claimed)
            {
                return false;
            }
        }

        return true;
    }

    public static bool TryClaimDailyBonus(DateTime utcNow)
    {
        if (!CanClaimDailyBonus(utcNow))
        {
            return false;
        }

        PlayerPrefs.SetInt(BonusClaimedKey, 1);
        PlayerPrefs.Save();
        EconomyProgress.GrantCoins(150);
        return true;
    }

    private static DailyMissionState Create(
        DailyMissionType type,
        string title,
        string progressKey,
        int target,
        int reward
    )
    {
        return new DailyMissionState(
            type,
            title,
            Mathf.Min(PlayerPrefs.GetInt(progressKey, 0), target),
            target,
            reward,
            PlayerPrefs.GetInt(ClaimedPrefix + type, 0) == 1
        );
    }

    private static void EnsureCurrentDay(DateTime utcNow)
    {
        string today = utcNow.ToUniversalTime().Date.ToString("yyyy-MM-dd");
        if (PlayerPrefs.GetString(DateKey, "") == today)
        {
            return;
        }

        PlayerPrefs.SetString(DateKey, today);
        PlayerPrefs.SetInt(PlaysKey, 0);
        PlayerPrefs.SetInt(WinsKey, 0);
        PlayerPrefs.SetInt(CampaignWinsKey, 0);
        PlayerPrefs.SetInt(ObstaclesKey, 0);
        PlayerPrefs.SetInt(BonusClaimedKey, 0);
        foreach (DailyMissionType type in Enum.GetValues(typeof(DailyMissionType)))
        {
            PlayerPrefs.DeleteKey(ClaimedPrefix + type);
        }
        PlayerPrefs.Save();
    }
}
