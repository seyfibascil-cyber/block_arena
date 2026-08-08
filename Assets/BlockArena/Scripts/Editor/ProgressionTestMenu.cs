using UnityEditor;
using UnityEngine;

public static class ProgressionTestMenu
{
    private const string UnlockedLevelKey = "BlockArena.UnlockedLevel";
    private const string CompletedLevelKey = "BlockArena.CompletedLevel";

    [MenuItem("Block Arena/Test/Tüm Bölümleri Aç")]
    private static void UnlockAllLevels()
    {
        PlayerPrefs.SetInt(UnlockedLevelKey, GameProgression.MaximumLevel);
        PlayerPrefs.Save();
        Debug.Log(
            "Test için 150 bölümün tamamı açıldı. " +
            "Ana menüyü yeniden açarak bölümleri deneyebilirsin."
        );
    }

    [MenuItem("Block Arena/Test/Bölüm İlerlemesini Sıfırla")]
    private static void ResetLevelProgress()
    {
        PlayerPrefs.SetInt(UnlockedLevelKey, 1);
        PlayerPrefs.SetInt(CompletedLevelKey, 0);
        PlayerPrefs.SetInt(GameProgression.SelectedLevelKey, 1);
        PlayerPrefs.Save();
        Debug.Log("Bölüm ilerlemesi test için sıfırlandı.");
    }

    [MenuItem("Block Arena/Test/5000 Test Jetonu Ekle")]
    private static void AddTestCoins()
    {
        EconomyProgress.GrantCoins(5000);
        Debug.Log(
            $"5000 test jetonu eklendi. Toplam: {EconomyProgress.Coins}"
        );
    }

    [MenuItem("Block Arena/Test/300 Test Y\u0131ld\u0131z\u0131 Ekle")]
    private static void AddTestStars()
    {
        for (int level = 1; level <= 100; level++)
        {
            PlayerPrefs.SetInt(
                "BlockArena.Economy.LevelStars." + level,
                EconomyProgress.MaximumStarsPerLevel
            );
        }

        PlayerPrefs.Save();
        Debug.Log("300 test y\u0131ld\u0131z\u0131 eklendi. Y\u0131ld\u0131zla a\u00E7\u0131lan \u015Fampiyonlar test edilebilir.");
    }
}
