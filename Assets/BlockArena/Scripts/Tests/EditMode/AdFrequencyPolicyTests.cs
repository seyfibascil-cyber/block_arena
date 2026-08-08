using System;
using NUnit.Framework;
using UnityEngine;

public class AdFrequencyPolicyTests
{
    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();
    }

    [Test]
    public void InterstitialBecomesDueAfterTwoEligibleMatches()
    {
        GameProgression.StartStandardGame(AIController.Difficulty.Easy);
        AdFrequencyPolicy.RecordCompletedMatch();
        Assert.IsFalse(AdFrequencyPolicy.IsInterstitialDue(DateTime.UtcNow));

        AdFrequencyPolicy.RecordCompletedMatch();
        Assert.IsTrue(AdFrequencyPolicy.IsInterstitialDue(DateTime.UtcNow));
    }

    [Test]
    public void CampaignLevelsCountTowardInterstitialFromTheStart()
    {
        GameProgression.StartLevel(1);
        AdFrequencyPolicy.RecordCompletedMatch();

        Assert.AreEqual(1, AdFrequencyPolicy.CompletedGamesSinceInterstitial);
    }

    [Test]
    public void ElapsedTimeDoesNotDelayAnAdAfterTwoMatches()
    {
        GameProgression.StartLevel(1);
        AdFrequencyPolicy.RecordCompletedMatch();
        AdFrequencyPolicy.RecordCompletedMatch();
        AdFrequencyPolicy.RecordInterstitialShown(DateTime.UtcNow);
        AdFrequencyPolicy.RecordCompletedMatch();
        AdFrequencyPolicy.RecordCompletedMatch();

        Assert.IsTrue(AdFrequencyPolicy.IsInterstitialDue(DateTime.UtcNow));
    }

    [Test]
    public void ShowingInterstitialResetsCounter()
    {
        GameProgression.StartStandardGame(AIController.Difficulty.Easy);
        AdFrequencyPolicy.RecordCompletedMatch();
        AdFrequencyPolicy.RecordCompletedMatch();

        AdFrequencyPolicy.RecordInterstitialShown(DateTime.UtcNow);

        Assert.AreEqual(0, AdFrequencyPolicy.CompletedGamesSinceInterstitial);
    }
}
