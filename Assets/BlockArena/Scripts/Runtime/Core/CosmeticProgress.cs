using UnityEngine;

public enum CosmeticCategory
{
    Character,
    Board,
    Obstacle,
    Effect
}

public sealed class CosmeticItem
{
    public CosmeticItem(
        string id,
        string name,
        CosmeticCategory category,
        int price,
        int requiredLevel,
        Color color,
        bool hasColorOverride = true
    )
    {
        Id = id;
        Name = name;
        Category = category;
        Price = price;
        RequiredLevel = requiredLevel;
        Color = color;
        HasColorOverride = hasColorOverride;
    }

    public string Id { get; }
    public string Name { get; }
    public CosmeticCategory Category { get; }
    public int Price { get; }
    public int RequiredLevel { get; }
    public Color Color { get; }
    public bool HasColorOverride { get; }
}

public static class CosmeticProgress
{
    private const string OwnedPrefix = "BlockArena.Cosmetics.Owned.";
    private const string EquippedPrefix = "BlockArena.Cosmetics.Equipped.";

    public static readonly CosmeticItem[] Items =
    {
        new CosmeticItem("character_default", "DÜNYA KARAKTERİ", CosmeticCategory.Character, 0, 1, Color.white, false),
        new CosmeticItem("character_ocean", "OKYANUS", CosmeticCategory.Character, 400, 15, Hex("#16C7D9")),
        new CosmeticItem("character_rose", "GÜL", CosmeticCategory.Character, 650, 30, Hex("#FF6FAE")),
        new CosmeticItem("character_gold", "ALTIN", CosmeticCategory.Character, 1200, 60, Hex("#FFD447")),
        new CosmeticItem("character_shadow", "GÖLGE", CosmeticCategory.Character, 1600, 76, Hex("#302C4D")),
        new CosmeticItem("character_lava", "LAV", CosmeticCategory.Character, 2200, 91, Hex("#FF542E")),
        new CosmeticItem("character_neon", "NEON", CosmeticCategory.Character, 3000, 121, Hex("#6CFF65")),
        new CosmeticItem("character_champion", "ARENA ŞAMPİYONU", CosmeticCategory.Character, 0, 150, Hex("#FFF0A3")),

        new CosmeticItem("board_default", "DÜNYA TAHTASI", CosmeticCategory.Board, 0, 1, Color.white, false),
        new CosmeticItem("board_midnight", "GECE MAVİSİ", CosmeticCategory.Board, 900, 31, Hex("#243B67")),
        new CosmeticItem("board_candy", "ŞEKER", CosmeticCategory.Board, 1300, 46, Hex("#D96A9E")),
        new CosmeticItem("board_royal", "KRALİYET MORU", CosmeticCategory.Board, 2000, 76, Hex("#633A91")),
        new CosmeticItem("board_obsidian", "OBSİDYEN", CosmeticCategory.Board, 3000, 101, Hex("#242229")),
        new CosmeticItem("board_galaxy", "GALAKSİ", CosmeticCategory.Board, 4500, 121, Hex("#252B70")),

        new CosmeticItem("obstacle_default", "DÜNYA ENGELİ", CosmeticCategory.Obstacle, 0, 1, Color.white, false),
        new CosmeticItem("obstacle_moss", "YOSUNLU TAŞ", CosmeticCategory.Obstacle, 600, 31, Hex("#668C4A")),
        new CosmeticItem("obstacle_crystal", "KRİSTAL", CosmeticCategory.Obstacle, 900, 61, Hex("#69D9FF")),
        new CosmeticItem("obstacle_magma", "MAGMA", CosmeticCategory.Obstacle, 1800, 91, Hex("#FF542E")),
        new CosmeticItem("obstacle_gold", "ALTIN BLOK", CosmeticCategory.Obstacle, 2500, 106, Hex("#F4C542")),
        new CosmeticItem("obstacle_energy", "ENERJİ", CosmeticCategory.Obstacle, 3500, 121, Hex("#B15CFF")),
        new CosmeticItem("obstacle_void", "BOŞLUK", CosmeticCategory.Obstacle, 5000, 141, Hex("#16142E")),

        new CosmeticItem("effect_default", "EFEKT YOK", CosmeticCategory.Effect, 0, 1, Color.white, false),
        new CosmeticItem("effect_aqua", "SU İZİ", CosmeticCategory.Effect, 1000, 31, Hex("#32D7FF")),
        new CosmeticItem("effect_ice", "BUZ İZİ", CosmeticCategory.Effect, 1800, 61, Hex("#BDEBFF")),
        new CosmeticItem("effect_flame", "ATEŞ İZİ", CosmeticCategory.Effect, 2800, 91, Hex("#FF6538")),
        new CosmeticItem("effect_neon", "NEON İZİ", CosmeticCategory.Effect, 4500, 121, Hex("#8CFF67"))
    };

    public static bool IsOwned(CosmeticItem item)
    {
        return item.Price == 0 || PlayerPrefs.GetInt(OwnedPrefix + item.Id, 0) == 1;
    }

    public static bool IsLevelUnlocked(CosmeticItem item)
    {
        return GameProgression.UnlockedLevel >= item.RequiredLevel;
    }

    public static bool TryPurchase(CosmeticItem item)
    {
        if (item == null || IsOwned(item) || !IsLevelUnlocked(item) ||
            !EconomyProgress.TrySpendCoins(item.Price))
        {
            return false;
        }

        PlayerPrefs.SetInt(OwnedPrefix + item.Id, 1);
        PlayerPrefs.Save();
        return true;
    }

    public static void Equip(CosmeticItem item)
    {
        if (item == null || !IsOwned(item))
        {
            return;
        }

        PlayerPrefs.SetString(EquippedPrefix + item.Category, item.Id);
        PlayerPrefs.Save();
    }

    public static bool IsEquipped(CosmeticItem item)
    {
        return GetEquipped(item.Category).Id == item.Id;
    }

    public static CosmeticItem GetEquipped(CosmeticCategory category)
    {
        string defaultId = GetDefaultId(category);
        string savedId = PlayerPrefs.GetString(
            EquippedPrefix + category,
            defaultId
        );

        foreach (CosmeticItem item in Items)
        {
            if (item.Category == category && item.Id == savedId && IsOwned(item))
            {
                return item;
            }
        }

        foreach (CosmeticItem item in Items)
        {
            if (item.Id == defaultId)
            {
                return item;
            }
        }

        return Items[0];
    }

    private static string GetDefaultId(CosmeticCategory category)
    {
        switch (category)
        {
            case CosmeticCategory.Character:
                return "character_default";
            case CosmeticCategory.Board:
                return "board_default";
            case CosmeticCategory.Obstacle:
                return "obstacle_default";
            default:
                return "effect_default";
        }
    }

    private static Color Hex(string value)
    {
        ColorUtility.TryParseHtmlString(value, out Color color);
        return color;
    }
}
