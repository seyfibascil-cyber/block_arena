using System;
using NUnit.Framework;
using UnityEngine;

public class DailyRewardProgressTests
{
    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();
    }

    [Test]
    public void FirstClaimAwardsDayOneCoins()
    {
        int reward = DailyRewardProgress.Claim(
            new DateTime(2026, 8, 1, 12, 0, 0, DateTimeKind.Utc)
        );

        Assert.That(reward, Is.EqualTo(25));
        Assert.That(EconomyProgress.Coins, Is.EqualTo(25));
    }

    [Test]
    public void SameDayCannotBeClaimedTwice()
    {
        DateTime now = new DateTime(2026, 8, 1, 12, 0, 0, DateTimeKind.Utc);
        DailyRewardProgress.Claim(now);

        Assert.That(DailyRewardProgress.Claim(now.AddHours(2)), Is.EqualTo(0));
    }

    [Test]
    public void ConsecutiveDayAdvancesStreak()
    {
        DateTime dayOne = new DateTime(2026, 8, 1, 12, 0, 0, DateTimeKind.Utc);
        DailyRewardProgress.Claim(dayOne);

        int reward = DailyRewardProgress.Claim(dayOne.AddDays(1));

        Assert.That(reward, Is.EqualTo(30));
        Assert.That(DailyRewardProgress.CurrentStreak, Is.EqualTo(2));
    }
}
