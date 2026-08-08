using NUnit.Framework;
using UnityEngine;

public class EconomyProgressTests
{
    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();
    }

    [Test]
    public void FirstTryCompletionAwardsThreeStarsAndFiftyCoins()
    {
        GameProgression.StartLevel(1);
        EconomyProgress.RewardCurrentLevelCompletion();

        Assert.That(EconomyProgress.GetLevelStars(1), Is.EqualTo(3));
        Assert.That(EconomyProgress.Coins, Is.EqualTo(50));
    }

    [Test]
    public void CompletionAfterDefeatAwardsTwoStars()
    {
        GameProgression.StartLevel(1);
        EconomyProgress.RecordCurrentLevelDefeat();
        EconomyProgress.RewardCurrentLevelCompletion();

        Assert.That(EconomyProgress.GetLevelStars(1), Is.EqualTo(2));
    }

    [Test]
    public void ReplayAwardsOnlyTenCoins()
    {
        GameProgression.StartLevel(1);
        EconomyProgress.RewardCurrentLevelCompletion();
        EconomyProgress.RewardCurrentLevelCompletion();

        Assert.That(EconomyProgress.Coins, Is.EqualTo(60));
    }
}
