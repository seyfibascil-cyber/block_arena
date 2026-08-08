using UnityEngine;

public readonly struct WorldTheme
{
    public WorldTheme(
        int number,
        string name,
        Color tileA,
        Color tileB,
        Color blockedTile,
        Color obstacle,
        Color human,
        Color enemy,
        Color background,
        float metallic,
        float smoothness
    )
    {
        Number = number;
        Name = name;
        TileA = tileA;
        TileB = tileB;
        BlockedTile = blockedTile;
        Obstacle = obstacle;
        Human = human;
        Enemy = enemy;
        Background = background;
        Metallic = metallic;
        Smoothness = smoothness;
    }

    public int Number { get; }
    public string Name { get; }
    public Color TileA { get; }
    public Color TileB { get; }
    public Color BlockedTile { get; }
    public Color Obstacle { get; }
    public Color Human { get; }
    public Color Enemy { get; }
    public Color Background { get; }
    public float Metallic { get; }
    public float Smoothness { get; }
}

public static class WorldThemeCatalog
{
    public const int LevelsPerWorld = 30;

    public static int GetWorldNumber(int level)
    {
        int safeLevel = Mathf.Clamp(level, 1, GameProgression.MaximumLevel);
        return ((safeLevel - 1) / LevelsPerWorld) + 1;
    }

    public static WorldTheme GetForCurrentGame()
    {
        GameProgression.GameMode mode =
            (GameProgression.GameMode)PlayerPrefs.GetInt(
                GameProgression.GameModeKey,
                (int)GameProgression.GameMode.Standard
            );
        int level = mode == GameProgression.GameMode.Levels
            ? PlayerPrefs.GetInt(GameProgression.SelectedLevelKey, 1)
            : Mathf.Max(1, GameProgression.UnlockedLevel);

        return GetForLevel(level);
    }

    public static WorldTheme GetForLevel(int level)
    {
        switch (GetWorldNumber(level))
        {
            case 2:
                return new WorldTheme(
                    2, "ORMAN",
                    Hex("#7FAE68"), Hex("#A8C686"), Hex("#40563A"),
                    Hex("#6B4931"), Hex("#FFD166"), Hex("#8D4E36"),
                    Hex("#78966A"), 0.05f, 0.25f
                );
            case 3:
                return new WorldTheme(
                    3, "BUZ DÜNYASI",
                    Hex("#BDEBFF"), Hex("#E5F7FF"), Hex("#608AA3"),
                    Hex("#66C7E8"), Hex("#F8D66D"), Hex("#7657D5"),
                    Hex("#91BFD6"), 0.25f, 0.8f
                );
            case 4:
                return new WorldTheme(
                    4, "LAV ARENASI",
                    Hex("#4A302D"), Hex("#664039"), Hex("#261B1B"),
                    Hex("#E0522D"), Hex("#FFD166"), Hex("#DA1E37"),
                    Hex("#39211E"), 0.15f, 0.35f
                );
            case 5:
                return new WorldTheme(
                    5, "UZAY ARENASI",
                    Hex("#182346"), Hex("#273764"), Hex("#080D20"),
                    Hex("#8D5CFF"), Hex("#00E5FF"), Hex("#FF3D9A"),
                    Hex("#070B1C"), 0.65f, 0.85f
                );
            default:
                return new WorldTheme(
                    1, "BAŞLANGIÇ ARENASI",
                    Hex("#D9D9D2"), Hex("#BFC7C9"), Hex("#585858"),
                    Hex("#57392F"), Hex("#14B8C4"), Hex("#E52D3D"),
                    Hex("#829CB7"), 0.05f, 0.3f
                );
        }
    }

    public static void ApplyToRenderers(
        GameObject target,
        Color color,
        float metallic,
        float smoothness
    )
    {
        if (target == null)
        {
            return;
        }

        foreach (Renderer renderer in target.GetComponentsInChildren<Renderer>())
        {
            foreach (Material material in renderer.materials)
            {
                material.color = color;
                if (material.HasProperty("_Metallic"))
                {
                    material.SetFloat("_Metallic", metallic);
                }
                if (material.HasProperty("_Smoothness"))
                {
                    material.SetFloat(
                        "_Smoothness",
                        Mathf.Min(smoothness, 0.35f)
                    );
                }
                if (material.HasProperty("_SpecularHighlights"))
                {
                    material.SetFloat("_SpecularHighlights", 0f);
                }
                if (material.HasProperty("_EnvironmentReflections"))
                {
                    material.SetFloat("_EnvironmentReflections", 0f);
                }
            }
        }
    }

    private static Color Hex(string value)
    {
        return ColorUtility.TryParseHtmlString(value, out Color color)
            ? color
            : Color.white;
    }
}
