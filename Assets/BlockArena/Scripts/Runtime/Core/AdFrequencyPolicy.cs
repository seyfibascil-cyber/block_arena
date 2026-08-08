using System;
using UnityEngine;

public static class AdFrequencyPolicy
{
    public const int CompletedGamesPerInterstitial = 2;

    private const string CompletedGamesKey =
        "BlockArena.Ads.CompletedGamesSinceInterstitial";
    private const string PolicyVersionKey = "BlockArena.Ads.PolicyVersion";
    private const int CurrentPolicyVersion = 3;

    public static int CompletedGamesSinceInterstitial
    {
        get
        {
            EnsureCurrentPolicy();
            return Mathf.Max(0, PlayerPrefs.GetInt(CompletedGamesKey, 0));
        }
    }

    public static bool IsCurrentMatchAdEligible()
    {
        EnsureCurrentPolicy();
        return true;
    }

    public static void RecordCompletedMatch()
    {
        if (!IsCurrentMatchAdEligible())
        {
            return;
        }

        PlayerPrefs.SetInt(
            CompletedGamesKey,
            CompletedGamesSinceInterstitial + 1
        );
        PlayerPrefs.Save();
    }

    public static bool IsInterstitialDue(DateTime utcNow)
    {
        EnsureCurrentPolicy();
        return CompletedGamesSinceInterstitial >=
               CompletedGamesPerInterstitial;
    }

    public static void RecordInterstitialShown(DateTime utcNow)
    {
        EnsureCurrentPolicy();
        PlayerPrefs.SetInt(CompletedGamesKey, 0);
        PlayerPrefs.Save();
    }

    private static void EnsureCurrentPolicy()
    {
        if (PlayerPrefs.GetInt(PolicyVersionKey, 0) == CurrentPolicyVersion)
        {
            return;
        }

        PlayerPrefs.SetInt(PolicyVersionKey, CurrentPolicyVersion);
        PlayerPrefs.SetInt(CompletedGamesKey, 0);
        PlayerPrefs.Save();
    }
}
