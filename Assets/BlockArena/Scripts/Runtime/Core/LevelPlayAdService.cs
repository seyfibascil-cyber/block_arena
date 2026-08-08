using System;
using Unity.Services.LevelPlay;
using UnityEngine;

public sealed class LevelPlayAdService : MonoBehaviour
{
    private const string AppKey = "276d357ad";
    private const string InterstitialAdUnitId = "pux9umeinndds4b6";
    private const string RewardedAdUnitId = "7st0cqk4ge95rml7";
    private const string TestSuiteShownKey =
        "BlockArena.Ads.LevelPlayTestSuiteShown";

    private static LevelPlayAdService instance;

    private LevelPlayInterstitialAd interstitialAd;
    private LevelPlayRewardedAd rewardedAd;
    private Action interstitialContinuation;
    private Action<bool> rewardedContinuation;
    private bool rewardReceived;
    private bool launchTestSuite;

    public static bool IsInitialized { get; private set; }
    public static bool IsFullScreenAdShowing { get; private set; }
    public static bool IsInterstitialReady =>
        instance != null &&
        instance.interstitialAd != null &&
        instance.interstitialAd.IsAdReady();
    public static bool IsRewardedReady =>
        instance != null &&
        instance.rewardedAd != null &&
        instance.rewardedAd.IsAdReady();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateService()
    {
#if UNITY_EDITOR
        // The LevelPlay editor stub can emit internal NullReferenceExceptions
        // while constructing simulated ad info. Ads are device-only for this
        // project, so keep editor play mode deterministic and noise-free.
        return;
#else
        if (instance != null)
        {
            return;
        }

        GameObject serviceObject = new GameObject("LevelPlayAdService");
        instance = serviceObject.AddComponent<LevelPlayAdService>();
        DontDestroyOnLoad(serviceObject);
#endif
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Block Arena 6-12 yaş grubuna yöneliktir. Bu ayarlar SDK
        // başlamadan önce tüm desteklenen ağlara çocuk durumunu iletir.
        LevelPlayPrivacySettings.SetCOPPA(true);
        LevelPlayPrivacySettings.SetCCPA(true);
        LevelPlay.SetMetaData("is_deviceid_optout", "true");
        LevelPlay.SetMetaData("is_child_directed", "true");
        LevelPlay.SetPauseGame(true);

        launchTestSuite = Debug.isDebugBuild &&
            PlayerPrefs.GetInt(TestSuiteShownKey, 0) == 0;
        if (launchTestSuite)
        {
            LevelPlay.SetMetaData("is_test_suite", "enable");
        }

        LevelPlay.OnInitSuccess += OnInitializationSucceeded;
        LevelPlay.OnInitFailed += OnInitializationFailed;
        LevelPlay.Init(AppKey);
    }

    public static bool TryShowInterstitial(Action continuation)
    {
        if (!IsInterstitialReady || instance.interstitialContinuation != null)
        {
            return false;
        }

        instance.interstitialContinuation = continuation;
        IsFullScreenAdShowing = true;
        instance.interstitialAd.ShowAd("completed_match");
        return true;
    }

    public static bool TryShowRewarded(
        Action<bool> continuation,
        string placement = "double_coins"
    )
    {
        if (!IsRewardedReady || instance.rewardedContinuation != null)
        {
            return false;
        }

        instance.rewardReceived = false;
        instance.rewardedContinuation = continuation;
        IsFullScreenAdShowing = true;
        instance.rewardedAd.ShowAd(placement);
        return true;
    }

    private void OnInitializationSucceeded(LevelPlayConfiguration configuration)
    {
        IsInitialized = true;

        if (launchTestSuite)
        {
            PlayerPrefs.SetInt(TestSuiteShownKey, 1);
            PlayerPrefs.Save();
            LevelPlay.LaunchTestSuite();
            return;
        }

        CreateInterstitial();
        CreateRewarded();
        Debug.Log("LevelPlay çocuk modu ile başlatıldı.");
    }

    private static void OnInitializationFailed(LevelPlayInitError error)
    {
        IsInitialized = false;
        Debug.LogWarning($"LevelPlay başlatılamadı: {error}");
    }

    private void CreateInterstitial()
    {
        interstitialAd = new LevelPlayInterstitialAd(InterstitialAdUnitId);
        interstitialAd.OnAdClosed += OnInterstitialClosed;
        interstitialAd.OnAdDisplayed += OnInterstitialDisplayed;
        interstitialAd.OnAdDisplayFailed += OnInterstitialDisplayFailed;
        interstitialAd.OnAdLoadFailed += error =>
            Debug.LogWarning($"Interstitial yüklenemedi: {error}");
        interstitialAd.LoadAd();
    }

    private void CreateRewarded()
    {
        rewardedAd = new LevelPlayRewardedAd(RewardedAdUnitId);
        rewardedAd.OnAdRewarded += OnRewardReceived;
        rewardedAd.OnAdClosed += OnRewardedClosed;
        rewardedAd.OnAdDisplayFailed += OnRewardedDisplayFailed;
        rewardedAd.OnAdLoadFailed += error =>
            Debug.LogWarning($"Ödüllü reklam yüklenemedi: {error}");
        rewardedAd.LoadAd();
    }

    private static void OnInterstitialDisplayed(LevelPlayAdInfo adInfo)
    {
        AdFrequencyPolicy.RecordInterstitialShown(DateTime.UtcNow);
    }

    private void OnInterstitialClosed(LevelPlayAdInfo adInfo)
    {
        IsFullScreenAdShowing = false;
        Action continuation = interstitialContinuation;
        interstitialContinuation = null;
        interstitialAd.LoadAd();
        continuation?.Invoke();
    }

    private void OnInterstitialDisplayFailed(
        LevelPlayAdInfo adInfo,
        LevelPlayAdError error
    )
    {
        Debug.LogWarning($"Interstitial gösterilemedi: {error}");
        OnInterstitialClosed(adInfo);
    }

    private void OnRewardReceived(
        LevelPlayAdInfo adInfo,
        LevelPlayReward reward
    )
    {
        rewardReceived = true;
    }

    private void OnRewardedClosed(LevelPlayAdInfo adInfo)
    {
        IsFullScreenAdShowing = false;
        Action<bool> continuation = rewardedContinuation;
        bool earned = rewardReceived;
        rewardedContinuation = null;
        rewardReceived = false;
        rewardedAd.LoadAd();
        continuation?.Invoke(earned);
    }

    private void OnRewardedDisplayFailed(
        LevelPlayAdInfo adInfo,
        LevelPlayAdError error
    )
    {
        Debug.LogWarning($"Ödüllü reklam gösterilemedi: {error}");
        OnRewardedClosed(adInfo);
    }

    private void OnDestroy()
    {
        if (instance != this)
        {
            return;
        }

        LevelPlay.OnInitSuccess -= OnInitializationSucceeded;
        LevelPlay.OnInitFailed -= OnInitializationFailed;
        interstitialAd?.DestroyAd();
        rewardedAd?.Dispose();
        instance = null;
        IsInitialized = false;
        IsFullScreenAdShowing = false;
    }
}
