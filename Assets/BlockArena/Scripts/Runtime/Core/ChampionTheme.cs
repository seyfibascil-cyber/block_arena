using UnityEngine;

public enum ChampionId
{
    Classic,
    Ninja,
    Pirate,
    Astronaut,
    Robot,
    Wizard,
    Dinosaur,
    Bear
}

public enum ChampionUnlockKind
{
    Free,
    Stars,
    Coins
}

public enum ChampionObstacleStyle
{
    StoneBlock,
    NinjaStar,
    ShipWheel,
    MoonRock,
    EnergyBarrier,
    RuneCrystal,
    DinosaurEgg,
    HoneyBarrel
}

public sealed class ChampionTheme
{
    public ChampionTheme(
        ChampionId id,
        string displayName,
        ChampionUnlockKind unlockKind,
        int unlockAmount,
        Color primaryColor,
        Color accentColor,
        ChampionObstacleStyle obstacleStyle
    )
    {
        Id = id;
        DisplayName = displayName;
        UnlockKind = unlockKind;
        UnlockAmount = Mathf.Max(0, unlockAmount);
        PrimaryColor = primaryColor;
        AccentColor = accentColor;
        ObstacleStyle = obstacleStyle;
    }

    public ChampionId Id { get; }
    public string DisplayName { get; }
    public ChampionUnlockKind UnlockKind { get; }
    public int UnlockAmount { get; }
    public Color PrimaryColor { get; }
    public Color AccentColor { get; }
    public ChampionObstacleStyle ObstacleStyle { get; }
}

public static class ChampionCatalog
{
    public static readonly ChampionTheme[] All =
    {
        Theme(ChampionId.Classic, "CUBO  •  KLAS\u0130K BLOK", ChampionUnlockKind.Free, 0, "#159DFF", "#78DCFF", ChampionObstacleStyle.StoneBlock),
        Theme(ChampionId.Ninja, "NINO  •  N\u0130NJA", ChampionUnlockKind.Coins, 500, "#242C78", "#8D55FF", ChampionObstacleStyle.NinjaStar),
        Theme(ChampionId.Pirate, "CAPPY  •  KORSAN", ChampionUnlockKind.Coins, 1000, "#16BFC4", "#F0A52B", ChampionObstacleStyle.ShipWheel),
        Theme(ChampionId.Astronaut, "NOVA  •  ASTRONOT", ChampionUnlockKind.Coins, 1500, "#F1F4FA", "#287DFF", ChampionObstacleStyle.MoonRock),
        Theme(ChampionId.Robot, "B\u0130P  •  ROBOT", ChampionUnlockKind.Coins, 2000, "#287DFF", "#67F4FF", ChampionObstacleStyle.EnergyBarrier),
        Theme(ChampionId.Wizard, "MORA  •  B\u00DCY\u00DCC\u00DC", ChampionUnlockKind.Coins, 2500, "#6C3CCB", "#E6A7FF", ChampionObstacleStyle.RuneCrystal),
        Theme(ChampionId.Bear, "BALBO  •  AYI", ChampionUnlockKind.Coins, 3000, "#A96732", "#FFD15A", ChampionObstacleStyle.HoneyBarrel),
        Theme(ChampionId.Dinosaur, "SEY\u0130R  •  D\u0130NOZOR", ChampionUnlockKind.Coins, 4000, "#75BC3A", "#FF9F2D", ChampionObstacleStyle.DinosaurEgg)
    };

    public static ChampionTheme Get(ChampionId id)
    {
        foreach (ChampionTheme theme in All)
        {
            if (theme.Id == id)
            {
                return theme;
            }
        }

        return All[0];
    }

    private static ChampionTheme Theme(
        ChampionId id,
        string name,
        ChampionUnlockKind unlockKind,
        int amount,
        string primary,
        string accent,
        ChampionObstacleStyle obstacle
    )
    {
        return new ChampionTheme(
            id,
            name,
            unlockKind,
            amount,
            Hex(primary),
            Hex(accent),
            obstacle
        );
    }

    private static Color Hex(string value)
    {
        ColorUtility.TryParseHtmlString(value, out Color color);
        return color;
    }
}
