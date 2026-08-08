using UnityEngine;

public static class ChampionProgress
{
    private const string SelectedKey = "BlockArena.Champions.Selected";
    private const string OwnedPrefix = "BlockArena.Champions.Owned.";

    public static ChampionTheme Selected => ChampionCatalog.Get(
        (ChampionId)Mathf.Clamp(
            PlayerPrefs.GetInt(SelectedKey, (int)ChampionId.Classic),
            0,
            ChampionCatalog.All.Length - 1
        )
    );

    public static bool IsUnlocked(ChampionTheme theme)
    {
        if (theme == null)
        {
            return false;
        }

        switch (theme.UnlockKind)
        {
            case ChampionUnlockKind.Free:
                return true;
            case ChampionUnlockKind.Stars:
                return EconomyProgress.TotalStars >= theme.UnlockAmount;
            case ChampionUnlockKind.Coins:
                return PlayerPrefs.GetInt(OwnedPrefix + theme.Id, 0) == 1;
            default:
                return false;
        }
    }

    public static bool TryUnlock(ChampionTheme theme)
    {
        if (theme == null)
        {
            return false;
        }

        if (IsUnlocked(theme))
        {
            return true;
        }

        if (theme.UnlockKind != ChampionUnlockKind.Coins ||
            !EconomyProgress.TrySpendCoins(theme.UnlockAmount))
        {
            return false;
        }

        PlayerPrefs.SetInt(OwnedPrefix + theme.Id, 1);
        PlayerPrefs.Save();
        return true;
    }

    public static bool TrySelect(ChampionTheme theme)
    {
        if (!IsUnlocked(theme))
        {
            return false;
        }

        PlayerPrefs.SetInt(SelectedKey, (int)theme.Id);
        PlayerPrefs.Save();
        return true;
    }
}
