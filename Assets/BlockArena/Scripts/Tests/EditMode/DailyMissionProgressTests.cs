using System;
using NUnit.Framework;
using UnityEngine;

public class DailyMissionProgressTests
{
    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();
    }

    [Test]
    public void CompletedMatchesAdvanceDailyMissions()
    {
        DailyMissionProgress.RecordCompletedMatch(true, true);
        DailyMissionState[] missions =
            DailyMissionProgress.GetMissions(DateTime.UtcNow);

        Assert.That(missions[0].Progress, Is.EqualTo(1));
        Assert.That(missions[1].Progress, Is.EqualTo(1));
        Assert.That(missions[2].Progress, Is.EqualTo(1));
    }

    [Test]
    public void CompletedMissionCanOnlyBeClaimedOnce()
    {
        DailyMissionProgress.RecordCompletedMatch(true, true);

        Assert.That(
            DailyMissionProgress.TryClaim(
                DailyMissionType.WinCampaignMatch,
                DateTime.UtcNow
            ),
            Is.True
        );
        Assert.That(
            DailyMissionProgress.TryClaim(
                DailyMissionType.WinCampaignMatch,
                DateTime.UtcNow
            ),
            Is.False
        );
        Assert.That(EconomyProgress.Coins, Is.EqualTo(50));
    }
}
