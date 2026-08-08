using System;
using UnityEngine;

public static class InterstitialAdController
{
    private static bool transitionInProgress;

    public static void ContinueAfterPossibleAd(Action continuation)
    {
        if (transitionInProgress)
        {
            return;
        }

        transitionInProgress = true;

        if (!AdFrequencyPolicy.IsInterstitialDue(DateTime.UtcNow))
        {
            Complete(continuation);
            return;
        }

        if (!LevelPlayAdService.TryShowInterstitial(
                () => Complete(continuation)
            ))
        {
            // Reklam hazır değilse çocuk oyuncuyu bekletmeden devam et.
            Complete(continuation);
        }
    }

    private static void Complete(Action continuation)
    {
        transitionInProgress = false;
        continuation?.Invoke();
    }
}
