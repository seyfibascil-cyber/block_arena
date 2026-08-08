using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private enum MenuPage
    {
        Modes,
        Difficulties,
        Levels,
        Shop,
        Collection,
        Missions,
        Pvp,
        Help,
        Settings,
        Languages,
        Information
    }

    private static readonly string[] DifficultyLabels =
    {
        "KOLAY",
        "ORTA",
        "ZOR",
        "İMKÂNSIZ"
    };

    private MenuPage currentPage = MenuPage.Modes;
    private Vector2 levelScrollPosition;
    private bool onlineInitializationStarted;
    private CosmeticCategory shopCategory = CosmeticCategory.Character;
    private string shopMessage = "";
    private bool dailyRewardVisible;
    private Vector2 shopScrollPosition;
    private Vector2 collectionScrollPosition;
    private bool exitConfirmationVisible;
    private Texture2D cuboTexture;
    private Texture2D ninjaTexture;
    private Texture2D pirateTexture;
    private Texture2D selectedChampionTexture;
    private Texture2D menuBackgroundTexture;
    private Texture2D menuButtonPanelTexture;
    private Texture2D menuLogoTexture;
    private Texture2D illustratedMenuTexture;
    private Texture2D levelsMapTexture;
    private Texture2D dailyMissionsTexture;
    private Texture2D characterCollectionTexture;
    private Texture2D unifiedBackButtonTexture;
    private Texture2D settingsTexture;
    private Texture2D pvpTexture;
    private Texture2D howToPlayTexture;
    private Texture2D languageSelectionTexture;
    private Texture2D informationTexture;
    private Font menuDisplayFont;
    private Font readableTextFont;
    private static Texture2D sharedPanelTexture;
    private bool musicEnabled = true;
    private bool soundEnabled = true;
    private bool vibrationEnabled = true;
    private Vector2 languageScrollPosition;
    private string informationTitle = "";
    private string informationBody = "";
    private MenuPage helpReturnPage = MenuPage.Modes;
    private int levelPage;
    private static readonly string[] LanguageNames =
    {
        "T\u00FCrk\u00E7e", "English", "Espa\u00F1ol", "Deutsch", "Fran\u00E7ais",
        "Italiano", "\u0420\u0443\u0441\u0441\u043A\u0438\u0439", "\u0627\u0644\u0639\u0631\u0628\u064A\u0629", "\u65E5\u672C\u8A9E"
    };

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape) ||
            LevelPlayAdService.IsFullScreenAdShowing)
        {
            return;
        }

        if (dailyRewardVisible)
        {
            dailyRewardVisible = false;
            return;
        }

        if (exitConfirmationVisible)
        {
            exitConfirmationVisible = false;
            return;
        }

        if (currentPage != MenuPage.Modes)
        {
            currentPage = MenuPage.Modes;
            return;
        }

        exitConfirmationVisible = true;
    }

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        cuboTexture = Resources.Load<Texture2D>("BlockArena/Champions/ClassicBlock-v1");
        ninjaTexture = Resources.Load<Texture2D>("BlockArena/Champions/Ninja-v1");
        pirateTexture = Resources.Load<Texture2D>("BlockArena/Champions/Pirate-v1");
        selectedChampionTexture = Resources.Load<Texture2D>(
            "BlockArena/Champions/" + GetChampionResourceName(ChampionProgress.Selected.Id)
        );
        menuBackgroundTexture = Resources.Load<Texture2D>(
            "BlockArena/UI/MainMenuArenaBackground-v1"
        );
        menuButtonPanelTexture = Resources.Load<Texture2D>(
            "BlockArena/UI/MenuButtonPanel-v1"
        );
        menuLogoTexture = Resources.Load<Texture2D>("BlockArena/UI/BlockArenaLogo-v1");
        illustratedMenuTexture = Resources.Load<Texture2D>(
            "BlockArena/UI/MainMenuIllustratedBase-v3"
        );
        levelsMapTexture = Resources.Load<Texture2D>(
            "BlockArena/UI/LevelsMapBase-v2"
        );
        dailyMissionsTexture = Resources.Load<Texture2D>(
            "BlockArena/UI/DailyMissionsBase-v2"
        );
        characterCollectionTexture = Resources.Load<Texture2D>(
            "BlockArena/UI/CharacterCollectionBase-v1"
        );
        unifiedBackButtonTexture = Resources.Load<Texture2D>(
            "BlockArena/UI/Common/BackButton-v1"
        );
        settingsTexture = Resources.Load<Texture2D>(
            "BlockArena/UI/SettingsBase-v1"
        );
        pvpTexture = Resources.Load<Texture2D>(
            "BlockArena/UI/PvpMatchmakingBase-v1"
        );
        howToPlayTexture = Resources.Load<Texture2D>(
            "BlockArena/UI/HowToPlayBase-v1"
        );
        languageSelectionTexture = Resources.Load<Texture2D>(
            "BlockArena/UI/LanguageSelectionBase-v1"
        );
        informationTexture = Resources.Load<Texture2D>(
            "BlockArena/UI/InformationBase-v1"
        );
        menuDisplayFont = Resources.Load<Font>(
            "BlockArena/Fonts/LilitaOne-Regular"
        );
        readableTextFont = Resources.Load<Font>("BlockArena/Fonts/Fredoka");
        sharedPanelTexture = menuButtonPanelTexture;
        musicEnabled = PlayerPrefs.GetInt("BlockArena.Settings.Music", 1) == 1;
        soundEnabled = PlayerPrefs.GetInt("BlockArena.Settings.Sound", 1) == 1;
        vibrationEnabled = PlayerPrefs.GetInt("BlockArena.Settings.Vibration", 1) == 1;
        if (!PlayerPrefs.HasKey("BlockArena.Settings.Language"))
        {
            PlayerPrefs.SetInt(
                "BlockArena.Settings.Language",
                GetSystemLanguageIndex(Application.systemLanguage)
            );
            PlayerPrefs.Save();
        }
        dailyRewardVisible = DailyRewardProgress.IsAvailable(DateTime.UtcNow);

        if (PlayerPrefs.GetInt(
                GameProgression.OpenLevelsMenuKey,
                0
            ) == 1)
        {
            OpenLevelsPage();
            PlayerPrefs.DeleteKey(GameProgression.OpenLevelsMenuKey);
            PlayerPrefs.Save();
        }

        GameObject oldPlayButton = GameObject.Find("PlayButton");

        if (oldPlayButton != null)
        {
            oldPlayButton.SetActive(false);
        }

        GameObject oldTitle = GameObject.Find("TitleText");

        if (oldTitle != null)
        {
            oldTitle.SetActive(false);
        }
    }

    private void OnGUI()
    {
        if (LevelPlayAdService.IsFullScreenAdShowing)
        {
            return;
        }

        Rect safe = Screen.safeArea;
        DrawMenuBackground(
            new Rect(safe.x, Screen.height - safe.yMax, safe.width, safe.height),
            menuBackgroundTexture
        );

        GUIStyle titleStyle = CreateTitleStyle();
        GUIStyle buttonStyle = CreateButtonStyle();

        if (exitConfirmationVisible)
        {
            DrawExitConfirmation(buttonStyle);
            return;
        }

        if (dailyRewardVisible)
        {
            DrawDailyReward(titleStyle, buttonStyle);
            return;
        }

        switch (currentPage)
        {
            case MenuPage.Difficulties:
                DrawDifficultyPage(titleStyle, buttonStyle);
                break;
            case MenuPage.Levels:
                DrawLevelsPage(titleStyle, buttonStyle);
                break;
            case MenuPage.Pvp:
                DrawPvpPage(titleStyle, buttonStyle);
                break;
            case MenuPage.Shop:
                DrawShopPage(titleStyle, buttonStyle);
                break;
            case MenuPage.Collection:
                DrawChampionCollectionPage(titleStyle, buttonStyle);
                break;
            case MenuPage.Missions:
                DrawMissionsPage(titleStyle, buttonStyle);
                break;
            case MenuPage.Help:
                DrawHelpPage(titleStyle, buttonStyle);
                break;
            case MenuPage.Settings:
                DrawSettingsPage(titleStyle, buttonStyle);
                break;
            case MenuPage.Languages:
                DrawLanguagePage(titleStyle, buttonStyle);
                break;
            case MenuPage.Information:
                DrawInformationPage(titleStyle, buttonStyle);
                break;
            default:
                DrawModesPage(titleStyle, buttonStyle);
                break;
        }
    }

    private void DrawDailyReward(
        GUIStyle titleStyle,
        GUIStyle buttonStyle
    )
    {
        Rect panel = GetPanelRect(390f);
        DrawPanel(panel);
        DrawTitle(panel, "GÜNLÜK ÖDÜL", titleStyle);

        int nextReward = DailyRewardProgress.GetNextReward(DateTime.UtcNow);
        int nextDay = DailyRewardProgress.CurrentStreak >= 7
            ? 1
            : DailyRewardProgress.CurrentStreak + 1;

        GUIStyle rewardStyle = new GUIStyle(titleStyle)
        {
            fontSize = 23,
            wordWrap = true
        };
        rewardStyle.normal.textColor = new Color(1f, 0.83f, 0.25f);
        GUI.Label(
            new Rect(panel.x + 30f, panel.y + 86f, panel.width - 60f, 105f),
            $"{nextDay}. GÜN\n{nextReward} JETON",
            rewardStyle
        );

        if (GUI.Button(
                new Rect(panel.x + 30f, panel.y + 205f, panel.width - 60f, 64f),
                "ÖDÜLÜ AL",
                buttonStyle
            ))
        {
            DailyRewardProgress.Claim(DateTime.UtcNow);
            dailyRewardVisible = false;
        }

        if (GUI.Button(
                new Rect(panel.x + 30f, panel.y + 288f, panel.width - 60f, 54f),
                "SONRA AL",
                buttonStyle
            ))
        {
            dailyRewardVisible = false;
        }
    }

    public void PlayGame()
    {
        currentPage = MenuPage.Difficulties;
    }

    private void OpenLevelsPage()
    {
        currentPage = MenuPage.Levels;
        levelPage = Mathf.Clamp(
            (Mathf.Max(1, GameProgression.UnlockedLevel) - 1) / 13,
            0,
            Mathf.Max(
                0,
                Mathf.CeilToInt(GameProgression.MaximumLevel / 13f) - 1
            )
        );
    }

    private void DrawModesPage(GUIStyle titleStyle, GUIStyle buttonStyle)
    {
        if (illustratedMenuTexture == null)
        {
            DrawLegacyModesPage(titleStyle, buttonStyle);
            return;
        }

        Rect safe = Screen.safeArea;
        Rect available = new Rect(
            safe.x,
            Screen.height - safe.yMax,
            safe.width,
            safe.height
        );
        // Preserve the source aspect ratio. Free Aspect in the editor can be
        // landscape even though Android is locked to portrait; stretching here
        // would make every illustrated character look wide and flattened.
        float sourceAspect = illustratedMenuTexture.width /
            (float)illustratedMenuTexture.height;
        float availableAspect = available.width / available.height;
        float artWidth;
        float artHeight;
        if (availableAspect > sourceAspect)
        {
            artHeight = available.height;
            artWidth = artHeight * sourceAspect;
        }
        else
        {
            artWidth = available.width;
            artHeight = artWidth / sourceAspect;
        }

        Rect art = new Rect(
            available.x + (available.width - artWidth) * 0.5f,
            available.y + (available.height - artHeight) * 0.5f,
            artWidth,
            artHeight
        );

        Color old = GUI.color;
        GUI.color = new Color(0.035f, 0.035f, 0.08f);
        GUI.DrawTexture(available, Texture2D.whiteTexture);
        GUI.color = old;
        if (menuBackgroundTexture != null)
        {
            GUI.DrawTexture(available, menuBackgroundTexture, ScaleMode.ScaleAndCrop, true);
        }
        GUI.DrawTexture(art, illustratedMenuTexture, ScaleMode.StretchToFill, true);
        DrawUnifiedBackButton(art);

        // Alt mavi ayarlar panelinin tamamını kapsa. Karakter ve görev
        // kartlarının tıklama alanları bu panelin üstünde biter.
        Rect settingsPanel = NormalizedRect(art, 0.10f, 0.915f, 0.80f, 0.08f);

        GUIStyle large = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.075f, 22f, 42f))
        );
        GUIStyle medium = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.06f, 18f, 34f))
        );
        GUIStyle small = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.042f, 15f, 25f))
        );

        DrawOutlinedLabel(NormalizedRect(art, 0.245f, 0.292f, 0.51f, 0.075f), T("quick_play"), large);
        DrawOutlinedLabel(NormalizedRect(art, 0.39f, 0.465f, 0.22f, 0.055f), T("pvp"), large);
        DrawOutlinedLabel(NormalizedRect(art, 0.31f, 0.625f, 0.5f, 0.045f), T("levels"), medium);
        DrawOutlinedLabel(NormalizedRect(art, 0.055f, 0.77f, 0.43f, 0.04f), T("characters"), small);
        DrawOutlinedLabel(NormalizedRect(art, 0.55f, 0.77f, 0.39f, 0.04f), T("missions"), small);
        DrawOutlinedLabel(NormalizedRect(art, 0.31f, 0.95f, 0.38f, 0.035f), T("settings"), medium);

        GUIStyle balance = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.045f, 16f, 25f))
        );
        DrawOutlinedLabel(NormalizedRect(art, 0.305f, 0.197f, 0.125f, 0.035f), EconomyProgress.Coins.ToString(), balance);
        DrawOutlinedLabel(NormalizedRect(art, 0.605f, 0.197f, 0.125f, 0.035f), EconomyProgress.TotalStars.ToString(), balance);

        if (InvisibleButton(NormalizedRect(art, 0.025f, 0.018f, 0.13f, 0.08f)))
        {
            exitConfirmationVisible = true;
        }
        if (InvisibleButton(NormalizedRect(art, 0.84f, 0.018f, 0.13f, 0.08f)))
        {
            helpReturnPage = MenuPage.Modes;
            currentPage = MenuPage.Help;
        }
        if (InvisibleButton(NormalizedRect(art, 0.06f, 0.245f, 0.88f, 0.185f)))
        {
            currentPage = MenuPage.Difficulties;
        }
        if (InvisibleButton(NormalizedRect(art, 0.06f, 0.435f, 0.88f, 0.175f)))
        {
            currentPage = MenuPage.Pvp;
            BeginOnlineInitialization();
        }
        if (InvisibleButton(NormalizedRect(art, 0.06f, 0.615f, 0.88f, 0.15f)))
        {
            OpenLevelsPage();
        }
        if (InvisibleButton(NormalizedRect(art, 0.04f, 0.77f, 0.46f, 0.145f)))
        {
            currentPage = MenuPage.Collection;
            shopMessage = "";
        }
        if (InvisibleButton(NormalizedRect(art, 0.51f, 0.77f, 0.46f, 0.145f)))
        {
            currentPage = MenuPage.Missions;
        }
        if (InvisibleButton(settingsPanel))
        {
            currentPage = MenuPage.Settings;
        }
    }

    private static Rect NormalizedRect(
        Rect parent,
        float x,
        float y,
        float width,
        float height
    )
    {
        return new Rect(
            parent.x + parent.width * x,
            parent.y + parent.height * y,
            parent.width * width,
            parent.height * height
        );
    }

    private static void DrawLevelLock(Rect rect)
    {
        Color previous = GUI.color;
        GUI.color = new Color(0.27f, 0.22f, 0.18f, 1f);

        float unit = Mathf.Min(rect.width, rect.height);
        float bodyWidth = unit * 0.48f;
        float bodyHeight = unit * 0.36f;
        float bodyX = rect.center.x - bodyWidth * 0.5f;
        float bodyY = rect.y + rect.height * 0.49f;
        GUI.DrawTexture(
            new Rect(bodyX, bodyY, bodyWidth, bodyHeight),
            Texture2D.whiteTexture
        );

        float bar = Mathf.Max(3f, unit * 0.075f);
        float shackleWidth = unit * 0.34f;
        float shackleX = rect.center.x - shackleWidth * 0.5f;
        float shackleTop = rect.y + rect.height * 0.20f;
        float shackleBottom = bodyY + bar;
        GUI.DrawTexture(
            new Rect(shackleX, shackleTop, shackleWidth, bar),
            Texture2D.whiteTexture
        );
        GUI.DrawTexture(
            new Rect(shackleX, shackleTop, bar, shackleBottom - shackleTop),
            Texture2D.whiteTexture
        );
        GUI.DrawTexture(
            new Rect(
                shackleX + shackleWidth - bar,
                shackleTop,
                bar,
                shackleBottom - shackleTop
            ),
            Texture2D.whiteTexture
        );

        GUI.color = previous;
    }

    private Rect DrawIllustratedPage(Texture2D texture)
    {
        Rect safe = Screen.safeArea;
        Rect available = new Rect(
            safe.x,
            Screen.height - safe.yMax,
            safe.width,
            safe.height
        );
        float sourceAspect = texture.width / (float)texture.height;
        bool fitHeight = available.width / available.height > sourceAspect;
        float artWidth = fitHeight ? available.height * sourceAspect : available.width;
        float artHeight = fitHeight ? available.height : available.width / sourceAspect;
        Rect art = new Rect(
            available.x + (available.width - artWidth) * 0.5f,
            available.y + (available.height - artHeight) * 0.5f,
            artWidth,
            artHeight
        );

        if (menuBackgroundTexture != null)
        {
            GUI.DrawTexture(available, menuBackgroundTexture, ScaleMode.ScaleAndCrop, true);
        }
        GUI.DrawTexture(art, texture, ScaleMode.StretchToFill, true);
        return art;
    }

    private static bool InvisibleButton(Rect rect)
    {
        return GUI.Button(rect, GUIContent.none, GUIStyle.none);
    }

    private GUIStyle CreateOverlayLabelStyle(int fontSize)
    {
        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = fontSize,
            fontStyle = FontStyle.Bold,
            wordWrap = true,
            font = BlockArenaLocalization.CurrentLanguageIndex <= 1
                ? menuDisplayFont
                : readableTextFont != null
                    ? readableTextFont
                    : menuDisplayFont
        };
        SetStyleTextColor(style, new Color(1f, 0.97f, 0.82f, 1f));
        return style;
    }

    private static void SetStyleTextColor(GUIStyle style, Color color)
    {
        style.normal.textColor = color;
        style.hover.textColor = color;
        style.active.textColor = color;
        style.focused.textColor = color;
        style.onNormal.textColor = color;
        style.onHover.textColor = color;
        style.onActive.textColor = color;
        style.onFocused.textColor = color;
    }

    private static void DrawOutlinedLabel(Rect rect, string text, GUIStyle style)
    {
        style = CreateFittedOutlinedStyle(rect, text, style);
        Color original = style.normal.textColor;

        float outline = Mathf.Clamp(style.fontSize * 0.085f, 2f, 4f);
        SetStyleTextColor(
            style,
            new Color(0.035f, 0.075f, 0.18f, 0.98f)
        );

        // A deep lower shadow gives the letters the raised, toy-like depth used
        // by the illustrated logo and the gold VS emblem.
        GUI.Label(
            new Rect(
                rect.x,
                rect.y + outline + 2f,
                rect.width,
                rect.height
            ),
            text,
            style
        );

        // Eight-direction outline remains crisp at different phone resolutions.
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                GUI.Label(
                    new Rect(
                        rect.x + x * outline,
                        rect.y + y * outline,
                        rect.width,
                        rect.height
                    ),
                    text,
                    style
                );
            }
        }

        SetStyleTextColor(style, original);
        GUI.Label(rect, text, style);

        // A small top highlight makes the cream face feel less flat without
        // baking language-specific text into the menu artwork.
        Color highlight = original;
        highlight.a = 0.32f;
        SetStyleTextColor(style, highlight);
        GUI.Label(
            new Rect(rect.x, rect.y - 1f, rect.width, rect.height),
            text,
            style
        );
        SetStyleTextColor(style, original);
    }

    private static GUIStyle CreateFittedOutlinedStyle(
        Rect rect,
        string text,
        GUIStyle source
    )
    {
        GUIStyle fitted = new GUIStyle(source)
        {
            wordWrap = false
        };
        int minimumSize = Mathf.Min(10, fitted.fontSize);
        float allowedWidth = Mathf.Max(1f, rect.width * 0.90f);
        float allowedHeight = Mathf.Max(1f, rect.height * 0.88f);

        while (fitted.fontSize > minimumSize)
        {
            Vector2 measured = fitted.CalcSize(new GUIContent(text));
            if (measured.x <= allowedWidth && measured.y <= allowedHeight)
            {
                break;
            }
            fitted.fontSize--;
        }

        return fitted;
    }

    private void DrawLegacyModesPage(
        GUIStyle titleStyle,
        GUIStyle buttonStyle
    )
    {
        Rect safeArea = Screen.safeArea;
        float safeTop = Screen.height - safeArea.yMax;
        Rect screen = new Rect(safeArea.x, safeTop, safeArea.width, safeArea.height);
        DrawMenuBackground(screen, menuBackgroundTexture);

        float width = Mathf.Min(560f, screen.width - 24f);
        Rect content = new Rect(
            screen.x + (screen.width - width) * 0.5f,
            screen.y + 10f,
            width,
            screen.height - 20f
        );

        GUIStyle roundButton = CreateRoundButtonStyle(new Color(0.12f, 0.48f, 0.9f));
        if (GUI.Button(new Rect(content.x, content.y, 48f, 48f), "\u2190", roundButton))
        {
            exitConfirmationVisible = true;
        }

        if (GUI.Button(new Rect(content.xMax - 48f, content.y, 48f, 48f), "?", roundButton))
        {
            helpReturnPage = MenuPage.Modes;
            currentPage = MenuPage.Help;
        }

        Rect logoRect = new Rect(content.x + 58f, content.y, content.width - 116f, 60f);
        if (menuLogoTexture != null)
        {
            GUI.DrawTexture(logoRect, menuLogoTexture, ScaleMode.ScaleToFit, true);
        }
        else
        {
            GUI.Label(logoRect, "BLOCK ARENA", titleStyle);
        }

        float balanceY = content.y + 66f;
        float balanceWidth = (content.width - 14f) * 0.5f;
        DrawBalanceCard(
            new Rect(content.x, balanceY, balanceWidth, 46f),
            "\u25C9",
            EconomyProgress.Coins,
            new Color(1f, 0.68f, 0.08f)
        );
        DrawBalanceCard(
            new Rect(content.x + balanceWidth + 14f, balanceY, balanceWidth, 46f),
            "\u2605",
            EconomyProgress.TotalStars,
            new Color(1f, 0.84f, 0.12f)
        );

        float startY = balanceY + 62f;
        float gap = 10f;
        float available = content.yMax - startY;
        float cardHeight = Mathf.Clamp((available - gap * 5f) / 6f, 62f, 96f);

        if (DrawIllustratedMenuButton(
                new Rect(content.x, startY, content.width, cardHeight),
                "HIZLI OYNA", "Yapay zekaya kar\u015F\u0131 hemen oyna",
                new Color(0.12f, 0.64f, 0.95f), cuboTexture, ninjaTexture,
                menuButtonPanelTexture))
        {
            currentPage = MenuPage.Difficulties;
        }

        startY += cardHeight + gap;
        if (DrawIllustratedMenuButton(
                new Rect(content.x, startY, content.width, cardHeight),
                "PVP", "Ger\u00E7ek rakip ara",
                new Color(0.48f, 0.28f, 0.86f), cuboTexture, pirateTexture,
                menuButtonPanelTexture))
        {
            currentPage = MenuPage.Pvp;
            BeginOnlineInitialization();
        }

        startY += cardHeight + gap;
        if (DrawIllustratedMenuButton(
                new Rect(content.x, startY, content.width, cardHeight),
                "B\u00D6L\u00DCMLER", "Y\u0131ld\u0131zlar\u0131 topla, arenalar\u0131 a\u00E7",
                new Color(0.16f, 0.72f, 0.42f), selectedChampionTexture, null,
                menuButtonPanelTexture))
        {
            OpenLevelsPage();
        }

        startY += cardHeight + gap;
        if (DrawIllustratedMenuButton(
                new Rect(content.x, startY, content.width, cardHeight),
                "KARAKTERLER", "Karakterini se\u00E7 ve yenilerini a\u00E7",
                new Color(0.94f, 0.48f, 0.18f), ninjaTexture, selectedChampionTexture,
                menuButtonPanelTexture))
        {
            currentPage = MenuPage.Collection;
            shopMessage = "";
        }

        startY += cardHeight + gap;
        if (DrawIllustratedMenuButton(
                new Rect(content.x, startY, content.width, cardHeight),
                "G\u00DCNL\u00DCK G\u00D6REVLER", "G\u00F6revleri tamamla, jeton kazan",
                new Color(0.94f, 0.66f, 0.12f), cuboTexture, null,
                menuButtonPanelTexture))
        {
            currentPage = MenuPage.Missions;
        }

        startY += cardHeight + gap;
        if (DrawIllustratedMenuButton(
                new Rect(content.x, startY, content.width, cardHeight),
                "AYARLAR", "Ses, dil, hesap ve yard\u0131m",
                new Color(0.3f, 0.48f, 0.68f), null, null,
                menuButtonPanelTexture))
        {
            currentPage = MenuPage.Settings;
        }
    }

    private void DrawExitConfirmation(GUIStyle buttonStyle)
    {
        Rect panel = GetPanelRect(300f);
        DrawPanel(panel);

        GUIStyle messageStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 23,
            fontStyle = FontStyle.Bold,
            wordWrap = true,
            font = readableTextFont
        };
        messageStyle.normal.textColor = Color.white;
        GUIStyle localizedButtonStyle = new GUIStyle(buttonStyle)
        {
            font = readableTextFont
        };

        GUI.Label(
            new Rect(panel.x + 25f, panel.y + 20f, panel.width - 50f, 90f),
            T("leave_prompt"),
            messageStyle
        );

        if (GUI.Button(
                new Rect(panel.x + 28f, panel.y + 128f, panel.width - 56f, 58f),
                T("yes_leave"),
                localizedButtonStyle
            ))
        {
            Application.Quit();
        }

        if (GUI.Button(
                new Rect(panel.x + 28f, panel.y + 207f, panel.width - 56f, 58f),
                T("cancel"),
                localizedButtonStyle
            ))
        {
            exitConfirmationVisible = false;
        }
    }

    private static void DrawMenuBackground(Rect screen, Texture2D background)
    {
        if (background != null)
        {
            GUI.DrawTexture(screen, background, ScaleMode.ScaleAndCrop, true);
            return;
        }

        Color previous = GUI.color;
        GUI.color = new Color(0.48f, 0.74f, 0.94f);
        GUI.DrawTexture(screen, Texture2D.whiteTexture);

        GUI.color = new Color(0.86f, 0.94f, 1f, 0.6f);
        GUI.DrawTexture(
            new Rect(screen.x, screen.y + screen.height * 0.63f, screen.width, screen.height * 0.37f),
            Texture2D.whiteTexture
        );
        GUI.color = previous;
    }

    private static void DrawBalanceCard(Rect rect, string icon, int value, Color accent)
    {
        Color previous = GUI.color;
        GUI.color = new Color(0.05f, 0.18f, 0.34f, 0.94f);
        GUI.Box(rect, GUIContent.none);
        GUI.color = previous;

        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };
        style.normal.textColor = accent;
        GUI.Label(rect, icon + "  " + value, style);
    }

    private static bool DrawIllustratedMenuButton(
        Rect rect,
        string title,
        string subtitle,
        Color color,
        Texture2D leftCharacter,
        Texture2D rightCharacter,
        Texture2D panelTexture
    )
    {
        if (panelTexture != null)
        {
            GUI.DrawTexture(rect, panelTexture, ScaleMode.StretchToFill, true);
        }
        else
        {
            Color previous = GUI.color;
            GUI.color = color;
            GUI.Box(rect, GUIContent.none);
            GUI.color = previous;
        }

        bool clicked = GUI.Button(rect, GUIContent.none, GUIStyle.none);

        float artWidth = Mathf.Min(rect.height * 1.05f, rect.width * 0.23f);
        Rect leftArt = new Rect(rect.x + 7f, rect.y + 3f, artWidth, rect.height - 6f);
        Rect rightArt = new Rect(rect.xMax - artWidth - 7f, rect.y + 3f, artWidth, rect.height - 6f);
        DrawCharacterTexture(leftArt, leftCharacter);
        DrawCharacterTexture(rightArt, rightCharacter);

        float textLeft = leftCharacter != null ? leftArt.xMax + 5f : rect.x + 20f;
        float textRight = rightCharacter != null ? rightArt.x - 5f : rect.xMax - 20f;
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.LowerCenter,
            fontSize = Mathf.RoundToInt(Mathf.Clamp(rect.height * 0.28f, 18f, 27f)),
            fontStyle = FontStyle.Bold
        };
        titleStyle.normal.textColor = Color.white;
        GUIStyle subtitleStyle = new GUIStyle(titleStyle)
        {
            alignment = TextAnchor.UpperCenter,
            fontSize = Mathf.RoundToInt(Mathf.Clamp(rect.height * 0.16f, 12f, 16f)),
            fontStyle = FontStyle.Normal
        };
        subtitleStyle.normal.textColor = new Color(0.94f, 0.98f, 1f);

        float textWidth = Mathf.Max(80f, textRight - textLeft);
        GUI.Label(new Rect(textLeft, rect.y + 7f, textWidth, rect.height * 0.46f), title, titleStyle);
        GUI.Label(new Rect(textLeft, rect.y + rect.height * 0.52f, textWidth, rect.height * 0.35f), subtitle, subtitleStyle);
        return clicked;
    }

    private static void DrawCharacterTexture(Rect rect, Texture2D texture)
    {
        if (texture == null)
        {
            return;
        }

        GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit, true);
    }

    private static GUIStyle CreateRoundButtonStyle(Color color)
    {
        GUIStyle style = new GUIStyle(GUI.skin.button)
        {
            fontSize = 28,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        style.normal.textColor = Color.white;
        return style;
    }

    private static string GetChampionResourceName(ChampionId id)
    {
        switch (id)
        {
            case ChampionId.Ninja: return "Ninja-v1";
            case ChampionId.Pirate: return "Pirate-v1";
            case ChampionId.Astronaut: return "Astronaut-v1";
            case ChampionId.Robot: return "Robot-v1";
            case ChampionId.Wizard: return "Wizard-v1";
            case ChampionId.Dinosaur: return "Dinosaur-v1";
            case ChampionId.Bear: return "Bear-v1";
            default: return "ClassicBlock-v1";
        }
    }

    private static string GetChampionShortName(ChampionId id)
    {
        switch (id)
        {
            case ChampionId.Ninja: return "NINO";
            case ChampionId.Pirate: return "CAPPY";
            case ChampionId.Astronaut: return "NOVA";
            case ChampionId.Robot: return "B\u0130P";
            case ChampionId.Wizard: return "MORA";
            case ChampionId.Dinosaur: return "SEY\u0130R";
            case ChampionId.Bear: return "BALBO";
            default: return "CUBO";
        }
    }

    private void DrawSettingsPage(GUIStyle titleStyle, GUIStyle buttonStyle)
    {
        if (settingsTexture == null)
        {
            DrawLegacySettingsPage(titleStyle, buttonStyle);
            return;
        }

        Rect safe = Screen.safeArea;
        Rect available = new Rect(
            safe.x,
            Screen.height - safe.yMax,
            safe.width,
            safe.height
        );
        float sourceAspect = settingsTexture.width / (float)settingsTexture.height;
        bool fitHeight = available.width / available.height > sourceAspect;
        float artWidth = fitHeight
            ? available.height * sourceAspect
            : available.width;
        float artHeight = fitHeight
            ? available.height
            : available.width / sourceAspect;
        Rect art = new Rect(
            available.x + (available.width - artWidth) * 0.5f,
            available.y + (available.height - artHeight) * 0.5f,
            artWidth,
            artHeight
        );

        GUI.DrawTexture(available, menuBackgroundTexture, ScaleMode.ScaleAndCrop, true);
        GUI.DrawTexture(art, settingsTexture, ScaleMode.StretchToFill, true);
        DrawUnifiedBackButton(art);

        GUIStyle header = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.065f, 22f, 38f))
        );
        GUIStyle rowStyle = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.038f, 14f, 22f))
        );
        GUIStyle smallStyle = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.030f, 12f, 18f))
        );

        DrawOutlinedLabel(NormalizedRect(art, 0.20f, 0.105f, 0.60f, 0.09f), T("settings"), header);

        float[] toggleY = { 0.250f, 0.342f, 0.433f };
        string[] toggleLabels = { T("music"), T("sound_effects"), T("vibration") };
        bool[] toggleValues = { musicEnabled, soundEnabled, vibrationEnabled };
        for (int index = 0; index < toggleY.Length; index++)
        {
            float y = toggleY[index];
            DrawOutlinedLabel(NormalizedRect(art, 0.27f, y, 0.38f, 0.06f), toggleLabels[index], rowStyle);
            DrawOutlinedLabel(
                NormalizedRect(art, 0.70f, y + 0.003f, 0.14f, 0.055f),
                toggleValues[index] ? T("on") : T("off"),
                smallStyle
            );
            if (InvisibleButton(NormalizedRect(art, 0.13f, y - 0.015f, 0.74f, 0.085f)))
            {
                toggleValues[index] = !toggleValues[index];
                SaveSettingToggle(index, toggleValues[index]);
            }
        }
        musicEnabled = toggleValues[0];
        soundEnabled = toggleValues[1];
        vibrationEnabled = toggleValues[2];

        DrawOutlinedLabel(NormalizedRect(art, 0.27f, 0.535f, 0.40f, 0.055f), "GOOGLE PLAY GAMES", rowStyle);
        DrawOutlinedLabel(NormalizedRect(art, 0.67f, 0.535f, 0.17f, 0.055f), T("connect"), smallStyle);
        DrawOutlinedLabel(NormalizedRect(art, 0.27f, 0.620f, 0.32f, 0.055f), T("language"), rowStyle);
        DrawOutlinedLabel(NormalizedRect(art, 0.62f, 0.620f, 0.23f, 0.055f), GetSelectedLanguageName(), smallStyle);
        if (InvisibleButton(NormalizedRect(art, 0.13f, 0.602f, 0.74f, 0.08f)))
        {
            currentPage = MenuPage.Languages;
        }

        string[,] gridLabels =
        {
            { T("how_to_play"), T("parent_guide") },
            { T("support"), T("credits") },
            { T("privacy"), T("terms") }
        };
        float[] gridY = { 0.715f, 0.805f, 0.895f };
        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 2; column++)
            {
                float x = column == 0 ? 0.12f : 0.52f;
                Rect hit = NormalizedRect(art, x, gridY[row] - 0.01f, 0.36f, 0.08f);
                DrawOutlinedLabel(
                    NormalizedRect(art, x + 0.08f, gridY[row], 0.27f, 0.06f),
                    gridLabels[row, column],
                    smallStyle
                );
                if (InvisibleButton(hit))
                {
                    OpenSettingsInformation(row, column);
                }
            }
        }

        if (InvisibleButton(NormalizedRect(art, 0.018f, 0.014f, 0.145f, 0.085f)))
        {
            currentPage = MenuPage.Modes;
        }
    }

    private void SaveSettingToggle(int index, bool value)
    {
        string key = index == 0
            ? "BlockArena.Settings.Music"
            : index == 1
                ? "BlockArena.Settings.Sound"
                : "BlockArena.Settings.Vibration";
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
        GameAudio.PlayButton();
        GameAudio.ApplySettings();
    }

    private void OpenSettingsInformation(int row, int column)
    {
        if (row == 0 && column == 0)
        {
            helpReturnPage = MenuPage.Settings;
            currentPage = MenuPage.Help;
            return;
        }
        if (row == 0) { OpenInformation(T("parent_guide"), T("parent_body")); return; }
        if (row == 1 && column == 0) { OpenInformation(T("support"), T("support_body")); return; }
        if (row == 1)
        {
            OpenInformation(
                T("credits"),
                "Seyir Ba\u015Fc\u0131l\nSeyfi Ba\u015Fc\u0131l"
            );
            return;
        }
        if (column == 0) { OpenInformation(T("privacy"), T("privacy_body")); return; }
        OpenInformation(T("terms"), T("terms_body"));
    }

    private void DrawLegacySettingsPage(GUIStyle titleStyle, GUIStyle buttonStyle)
    {
        Rect panel = GetPanelRect(740f);
        DrawPanel(panel);
        DrawTitle(panel, "AYARLAR", titleStyle);

        musicEnabled = DrawSettingToggle(panel, 0, "\u266B  M\u00DCZ\u0130K", musicEnabled);
        soundEnabled = DrawSettingToggle(panel, 1, "\u25B6  SES EFEKTLER\u0130", soundEnabled);
        vibrationEnabled = DrawSettingToggle(panel, 2, "\u223F  T\u0130TRE\u015E\u0130M", vibrationEnabled);

        if (GUI.Button(
                new Rect(panel.x + 30f, panel.y + 300f, panel.width - 60f, 54f),
                "GOOGLE PLAY GAMES     BA\u011ELAN",
                buttonStyle))
        {
        }

        if (GUI.Button(
                new Rect(panel.x + 30f, panel.y + 368f, panel.width - 60f, 54f),
                "D\u0130L     " + GetSelectedLanguageName() + "  >",
                buttonStyle))
        {
            currentPage = MenuPage.Languages;
        }

        if (GUI.Button(
                new Rect(panel.x + 30f, panel.y + 436f, (panel.width - 70f) * 0.5f, 44f),
                "NASIL OYNANIR?",
                buttonStyle))
        {
            helpReturnPage = MenuPage.Settings;
            currentPage = MenuPage.Help;
        }

        float half = (panel.width - 70f) * 0.5f;
        float right = panel.x + 40f + half;
        if (GUI.Button(new Rect(right, panel.y + 436f, half, 44f), "EBEVEYN KILAVUZU", buttonStyle))
        {
            OpenInformation(
                "EBEVEYN KILAVUZU",
                "Block Arena sohbet i\u00E7ermez. PVP modunda \u00F6nce ger\u00E7ek oyuncu aran\u0131r; bulunamazsa bot oldu\u011Fu a\u00E7\u0131k\u00E7a belirtilir. Reklamlar oyun aralar\u0131nda g\u00F6sterilir. \u00D6d\u00FCll\u00FC reklamlar iste\u011Fe ba\u011Fl\u0131d\u0131r."
            );
        }
        if (GUI.Button(new Rect(panel.x + 30f, panel.y + 490f, half, 44f), "YARDIM VE DESTEK", buttonStyle))
        {
            OpenInformation("YARDIM VE DESTEK", "Bir sorun ya\u015Farsan\u0131z uygulama s\u00FCr\u00FCm\u00FCn\u00FC ve cihaz modelinizi destek mesaj\u0131n\u0131za ekleyin.");
        }
        if (GUI.Button(new Rect(right, panel.y + 490f, half, 44f), "HAZIRLAYANLAR", buttonStyle))
        {
            OpenInformation(
                "HAZIRLAYANLAR",
                "Seyir Ba\u015Fc\u0131l\nSeyfi Ba\u015Fc\u0131l"
            );
        }
        if (GUI.Button(new Rect(panel.x + 30f, panel.y + 544f, half, 44f), "G\u0130ZL\u0130L\u0130K", buttonStyle))
        {
            OpenInformation("G\u0130ZL\u0130L\u0130K", "Oyun ilerlemesi cihazda saklan\u0131r. Reklam ve \u00E7evrimi\u00E7i hizmetlerin veri kullan\u0131m\u0131 yay\u0131nlanacak gizlilik politikas\u0131nda ayr\u0131nt\u0131l\u0131 olarak a\u00E7\u0131klan\u0131r.");
        }
        if (GUI.Button(new Rect(right, panel.y + 544f, half, 44f), "KULLANIM \u015EARTLARI", buttonStyle))
        {
            OpenInformation("KULLANIM \u015EARTLARI", "Block Arena e\u011Flence amac\u0131yla sunulur. Hile, hizmeti bozma ve ba\u015Fkalar\u0131n\u0131n oyun deneyimine zarar verme yasakt\u0131r.");
        }

        GUI.Label(
            new Rect(panel.x, panel.yMax - 105f, panel.width, 28f),
            "S\u00DCR\u00DCM " + Application.version,
            new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                normal = { textColor = new Color(0.75f, 0.84f, 0.94f) }
            }
        );
        DrawBackButton(panel, buttonStyle);
    }

    private void OpenInformation(string title, string body)
    {
        informationTitle = title;
        informationBody = body;
        currentPage = MenuPage.Information;
    }

    private void DrawInformationPage(GUIStyle titleStyle, GUIStyle buttonStyle)
    {
        if (informationTexture == null)
        {
            DrawLegacyInformationPage(titleStyle, buttonStyle);
            return;
        }

        Rect art = DrawIllustratedPage(informationTexture);
        DrawUnifiedBackButton(art);

        GUIStyle header = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.058f, 20f, 34f))
        );
        DrawOutlinedLabel(
            NormalizedRect(art, 0.20f, 0.095f, 0.60f, 0.10f),
            informationTitle,
            header
        );

        bool isCredits = informationTitle == T("credits");
        GUIStyle body = new GUIStyle(GUI.skin.label)
        {
            alignment = isCredits ? TextAnchor.UpperCenter : TextAnchor.UpperLeft,
            font = readableTextFont,
            fontSize = Mathf.RoundToInt(Mathf.Clamp(
                art.width * (isCredits ? 0.052f : 0.037f),
                isCredits ? 21f : 15f,
                isCredits ? 31f : 22f
            )),
            fontStyle = FontStyle.Bold,
            wordWrap = true,
            padding = new RectOffset(8, 8, 8, 8)
        };
        Color bodyColor = isCredits
            ? new Color(1f, 0.88f, 0.48f)
            : new Color(0.08f, 0.13f, 0.22f);
        body.normal.textColor = bodyColor;
        body.hover.textColor = bodyColor;
        body.active.textColor = bodyColor;
        body.focused.textColor = bodyColor;
        body.onNormal.textColor = bodyColor;
        body.onHover.textColor = bodyColor;
        body.onActive.textColor = bodyColor;
        body.onFocused.textColor = bodyColor;
        Rect bodyRect = NormalizedRect(
            art,
            0.13f,
            isCredits ? 0.36f : 0.325f,
            0.74f,
            isCredits ? 0.28f : 0.49f
        );
        if (isCredits)
        {
            DrawOutlinedLabel(bodyRect, informationBody, body);
        }
        else
        {
            GUI.Label(bodyRect, informationBody, body);
        }

        GUIStyle back = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.052f, 18f, 30f))
        );
        DrawOutlinedLabel(NormalizedRect(art, 0.30f, 0.906f, 0.40f, 0.055f), T("back"), back);

        if (InvisibleButton(NormalizedRect(art, 0.018f, 0.014f, 0.145f, 0.085f)) ||
            InvisibleButton(NormalizedRect(art, 0.24f, 0.89f, 0.52f, 0.09f)))
        {
            currentPage = MenuPage.Settings;
        }
    }

    private void DrawLegacyInformationPage(GUIStyle titleStyle, GUIStyle buttonStyle)
    {
        Rect panel = GetPanelRect(620f);
        DrawPanel(panel);
        DrawTitle(panel, informationTitle, titleStyle);

        GUIStyle bodyStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.UpperLeft,
            fontSize = 19,
            wordWrap = true,
            padding = new RectOffset(18, 18, 18, 18)
        };
        bodyStyle.normal.textColor = Color.white;
        GUI.Label(
            new Rect(panel.x + 32f, panel.y + 92f, panel.width - 64f, panel.height - 190f),
            informationBody,
            bodyStyle
        );

        if (GUI.Button(
                new Rect(panel.x + 30f, panel.yMax - 72f, panel.width - 60f, 52f),
                "GER\u0130",
                buttonStyle))
        {
            currentPage = MenuPage.Settings;
        }
    }

    private void DrawLanguagePage(GUIStyle titleStyle, GUIStyle buttonStyle)
    {
        if (languageSelectionTexture == null)
        {
            DrawLegacyLanguagePage(titleStyle, buttonStyle);
            return;
        }

        Rect art = DrawIllustratedPage(languageSelectionTexture);
        DrawUnifiedBackButton(art);
        GUIStyle header = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.07f, 22f, 40f))
        );
        DrawOutlinedLabel(NormalizedRect(art, 0.24f, 0.09f, 0.52f, 0.10f), T("language"), header);

        int selected = Mathf.Clamp(
            PlayerPrefs.GetInt("BlockArena.Settings.Language", 0),
            0,
            LanguageNames.Length - 1
        );
        GUIStyle languageStyle = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.045f, 17f, 27f))
        );
        languageStyle.font = readableTextFont != null ? readableTextFont : menuDisplayFont;
        GUIStyle markerStyle = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.038f, 14f, 23f))
        );
        float firstY = 0.238f;
        float step = 0.0672f;
        for (int index = 0; index < LanguageNames.Length; index++)
        {
            float y = firstY + index * step;
            DrawOutlinedLabel(
                NormalizedRect(art, 0.27f, y, 0.48f, 0.045f),
                LanguageNames[index],
                languageStyle
            );
            if (index == selected)
            {
                DrawOutlinedLabel(
                    NormalizedRect(art, 0.755f, y, 0.075f, 0.045f),
                    "OK",
                    markerStyle
                );
            }
            if (InvisibleButton(NormalizedRect(art, 0.15f, y - 0.012f, 0.70f, 0.063f)))
            {
                PlayerPrefs.SetInt("BlockArena.Settings.Language", index);
                PlayerPrefs.Save();
                currentPage = MenuPage.Settings;
            }
        }

        GUIStyle back = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.052f, 18f, 30f))
        );
        DrawOutlinedLabel(NormalizedRect(art, 0.30f, 0.872f, 0.40f, 0.055f), T("back"), back);
        if (InvisibleButton(NormalizedRect(art, 0.018f, 0.014f, 0.145f, 0.085f)) ||
            InvisibleButton(NormalizedRect(art, 0.23f, 0.87f, 0.54f, 0.10f)))
        {
            currentPage = MenuPage.Settings;
        }
    }

    private void DrawLegacyLanguagePage(GUIStyle titleStyle, GUIStyle buttonStyle)
    {
        Rect panel = GetPanelRect(650f);
        DrawPanel(panel);
        DrawTitle(panel, "D\u0130L", titleStyle);

        Rect view = new Rect(
            panel.x + 28f,
            panel.y + 82f,
            panel.width - 56f,
            panel.height - 170f
        );
        languageScrollPosition = GUI.BeginScrollView(
            view,
            languageScrollPosition,
            new Rect(0f, 0f, view.width - 18f, LanguageNames.Length * 62f)
        );

        int selected = Mathf.Clamp(
            PlayerPrefs.GetInt("BlockArena.Settings.Language", 0),
            0,
            LanguageNames.Length - 1
        );
        for (int index = 0; index < LanguageNames.Length; index++)
        {
            string marker = index == selected ? "  \u2713" : "";
            if (GUI.Button(
                    new Rect(4f, index * 62f, view.width - 30f, 52f),
                    LanguageNames[index] + marker,
                    buttonStyle))
            {
                PlayerPrefs.SetInt("BlockArena.Settings.Language", index);
                PlayerPrefs.Save();
                currentPage = MenuPage.Settings;
            }
        }
        GUI.EndScrollView();

        if (GUI.Button(
                new Rect(panel.x + 30f, panel.yMax - 72f, panel.width - 60f, 52f),
                "GER\u0130",
                buttonStyle))
        {
            currentPage = MenuPage.Settings;
        }
    }

    private static string GetSelectedLanguageName()
    {
        int selected = Mathf.Clamp(
            PlayerPrefs.GetInt("BlockArena.Settings.Language", 0),
            0,
            LanguageNames.Length - 1
        );
        return LanguageNames[selected];
    }

    private static int GetSystemLanguageIndex(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.Turkish: return 0;
            case SystemLanguage.Spanish: return 2;
            case SystemLanguage.German: return 3;
            case SystemLanguage.French: return 4;
            case SystemLanguage.Italian: return 5;
            case SystemLanguage.Russian: return 6;
            case SystemLanguage.Arabic: return 7;
            case SystemLanguage.Japanese: return 8;
            default: return 1;
        }
    }

    private bool DrawSettingToggle(Rect panel, int index, string label, bool value)
    {
        Rect row = new Rect(panel.x + 30f, panel.y + 88f + index * 68f, panel.width - 60f, 54f);
        GUIStyle style = new GUIStyle(CreateButtonStyle())
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(18, 18, 0, 0)
        };
        if (GUI.Button(row, label + (value ? "                 A\u00C7IK" : "                 KAPALI"), style))
        {
            value = !value;
            string key = index == 0
                ? "BlockArena.Settings.Music"
                : index == 1
                    ? "BlockArena.Settings.Sound"
                    : "BlockArena.Settings.Vibration";
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            PlayerPrefs.Save();
            GameAudio.PlayButton();
            GameAudio.ApplySettings();
        }

        return value;
    }

    private void DrawMissionsPage(GUIStyle titleStyle, GUIStyle buttonStyle)
    {
        if (dailyMissionsTexture == null)
        {
            DrawLegacyMissionsPage(titleStyle, buttonStyle);
            return;
        }

        Rect safe = Screen.safeArea;
        Rect available = new Rect(
            safe.x,
            Screen.height - safe.yMax,
            safe.width,
            safe.height
        );
        float sourceAspect = dailyMissionsTexture.width /
            (float)dailyMissionsTexture.height;
        bool fitHeight = available.width / available.height > sourceAspect;
        float artWidth = fitHeight
            ? available.height * sourceAspect
            : available.width;
        float artHeight = fitHeight
            ? available.height
            : available.width / sourceAspect;
        Rect art = new Rect(
            available.x + (available.width - artWidth) * 0.5f,
            available.y + (available.height - artHeight) * 0.5f,
            artWidth,
            artHeight
        );

        if (menuBackgroundTexture != null)
        {
            GUI.DrawTexture(
                available,
                menuBackgroundTexture,
                ScaleMode.ScaleAndCrop,
                true
            );
        }
        GUI.DrawTexture(art, dailyMissionsTexture, ScaleMode.StretchToFill, true);
        DrawUnifiedBackButton(art);

        GUIStyle header = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.065f, 21f, 38f))
        );
        GUIStyle missionTitle = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.044f, 18f, 30f))
        );
        GUIStyle detail = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.040f, 18f, 27f))
        );

        DrawOutlinedLabel(
            NormalizedRect(art, 0.18f, 0.075f, 0.64f, 0.10f),
            T("missions"),
            header
        );
        DrawOutlinedLabel(
            NormalizedRect(art, 0.78f, 0.026f, 0.16f, 0.04f),
            EconomyProgress.Coins.ToString(),
            detail
        );

        DailyMissionState[] missions =
            DailyMissionProgress.GetMissions(DateTime.UtcNow);
        float[] cardY = { 0.222f, 0.344f, 0.466f, 0.588f };
        for (int index = 0; index < missions.Length; index++)
        {
            DailyMissionState mission = missions[index];
            float y = cardY[index];
            string status = mission.Claimed
                ? T("claimed")
                : mission.IsComplete
                    ? T("claim")
                    : $"{mission.Progress}/{mission.Target}";

            DrawOutlinedLabel(
                NormalizedRect(art, 0.265f, y + 0.006f, 0.42f, 0.045f),
                GetLocalizedMissionTitle(mission.Type),
                missionTitle
            );
            DrawOutlinedLabel(
                NormalizedRect(art, 0.735f, y + 0.045f, 0.15f, 0.038f),
                mission.Reward.ToString(),
                detail
            );

            Rect progressTrack = NormalizedRect(
                art,
                0.292f,
                y + 0.083f,
                0.365f,
                0.014f
            );
            float ratio = Mathf.Clamp01(
                mission.Progress / (float)Mathf.Max(1, mission.Target)
            );
            if (ratio > 0f)
            {
                Color previous = GUI.color;
                GUI.color = mission.Claimed
                    ? new Color(0.40f, 0.95f, 0.50f)
                    : new Color(0.15f, 0.88f, 1f);
                GUI.DrawTexture(
                    new Rect(
                        progressTrack.x,
                        progressTrack.y,
                        progressTrack.width * ratio,
                        progressTrack.height
                    ),
                    Texture2D.whiteTexture
                );
                GUI.color = previous;
            }
            DrawOutlinedLabel(
                NormalizedRect(art, 0.39f, y + 0.071f, 0.18f, 0.034f),
                status,
                detail
            );

            Rect missionHit = NormalizedRect(
                art,
                0.08f,
                y - 0.002f,
                0.84f,
                0.11f
            );
            if (mission.IsComplete && !mission.Claimed &&
                InvisibleButton(missionHit))
            {
                DailyMissionProgress.TryClaim(mission.Type, DateTime.UtcNow);
            }
        }

        int claimedCount = 0;
        foreach (DailyMissionState mission in missions)
        {
            if (mission.Claimed)
            {
                claimedCount++;
            }
        }
        DrawOutlinedLabel(
            NormalizedRect(art, 0.105f, 0.754f, 0.29f, 0.05f),
            T("mission_chest"),
            detail
        );
        DrawOutlinedLabel(
            NormalizedRect(art, 0.63f, 0.754f, 0.23f, 0.05f),
            "150",
            detail
        );
        for (int index = 0; index < 4; index++)
        {
            if (index < claimedCount)
            {
                DrawOutlinedLabel(
                    NormalizedRect(
                        art,
                        0.276f + index * 0.132f,
                        0.858f,
                        0.06f,
                        0.035f
                    ),
                    "\u2713",
                    detail
                );
            }
        }

        bool bonusReady = DailyMissionProgress.CanClaimDailyBonus(DateTime.UtcNow);
        DrawOutlinedLabel(
            NormalizedRect(art, 0.30f, 0.80f, 0.40f, 0.045f),
            bonusReady ? T("claim_chest") : $"{claimedCount}/4",
            missionTitle
        );
        if (bonusReady && InvisibleButton(
                NormalizedRect(art, 0.07f, 0.72f, 0.86f, 0.17f)
            ))
        {
            DailyMissionProgress.TryClaimDailyBonus(DateTime.UtcNow);
        }

        DrawOutlinedLabel(
            NormalizedRect(art, 0.24f, 0.925f, 0.52f, 0.045f),
            T("back"),
            header
        );
        bool lowerBack = InvisibleButton(
            NormalizedRect(art, 0.19f, 0.91f, 0.62f, 0.075f)
        );
        bool upperBack = InvisibleButton(
            NormalizedRect(art, 0.02f, 0.02f, 0.15f, 0.09f)
        );
        if (lowerBack || upperBack)
        {
            currentPage = MenuPage.Modes;
        }
    }

    private void DrawLegacyMissionsPage(
        GUIStyle titleStyle,
        GUIStyle buttonStyle
    )
    {
        Rect panel = GetPanelRect(540f);
        DrawPanel(panel);
        DrawTitle(panel, "GÜNLÜK GÖREVLER", titleStyle);
        bool bonusReady = DailyMissionProgress.CanClaimDailyBonus(DateTime.UtcNow);
        GUI.enabled = bonusReady;
        if (GUI.Button(
                new Rect(panel.x + 58f, panel.y + 70f, panel.width - 116f, 42f),
                bonusReady ? "GÜNLÜK SANDIK  •  150 JETON  •  AL" : "TÜM GÖREVLER  •  150 JETON",
                buttonStyle))
        {
            DailyMissionProgress.TryClaimDailyBonus(DateTime.UtcNow);
        }
        GUI.enabled = true;

        DailyMissionState[] missions =
            DailyMissionProgress.GetMissions(DateTime.UtcNow);
        for (int index = 0; index < missions.Length; index++)
        {
            DailyMissionState mission = missions[index];
            string state = mission.Claimed
                ? "ALINDI"
                : mission.IsComplete
                    ? $"{mission.Reward} JETONU AL"
                    : $"{mission.Progress}/{mission.Target}  •  {mission.Reward} JETON";

            GUI.enabled = mission.IsComplete && !mission.Claimed;
            if (GUI.Button(
                    new Rect(
                        panel.x + 112f,
                        panel.y + 124f + index * 82f,
                        panel.width - 142f,
                        64f
                    ),
                    $"{mission.Title}\n{state}",
                    buttonStyle
                ))
            {
                DailyMissionProgress.TryClaim(mission.Type, DateTime.UtcNow);
            }
            GUI.enabled = true;

            Texture2D missionArt = index == 0
                ? cuboTexture
                : index == 1
                    ? ninjaTexture
                    : index == 2
                        ? pirateTexture
                        : selectedChampionTexture;
            DrawCharacterTexture(
                new Rect(panel.x + 36f, panel.y + 120f + index * 82f, 72f, 72f),
                missionArt
            );
        }

        DrawBackButton(panel, buttonStyle);
    }

    private void DrawChampionCollectionPage(
        GUIStyle titleStyle,
        GUIStyle buttonStyle
    )
    {
        if (characterCollectionTexture == null)
        {
            DrawLegacyChampionCollectionPage(titleStyle, buttonStyle);
            return;
        }

        Rect safe = Screen.safeArea;
        Rect available = new Rect(
            safe.x,
            Screen.height - safe.yMax,
            safe.width,
            safe.height
        );
        float sourceAspect = characterCollectionTexture.width /
            (float)characterCollectionTexture.height;
        bool fitHeight = available.width / available.height > sourceAspect;
        float artWidth = fitHeight
            ? available.height * sourceAspect
            : available.width;
        float artHeight = fitHeight
            ? available.height
            : available.width / sourceAspect;
        Rect art = new Rect(
            available.x + (available.width - artWidth) * 0.5f,
            available.y + (available.height - artHeight) * 0.5f,
            artWidth,
            artHeight
        );

        if (menuBackgroundTexture != null)
        {
            GUI.DrawTexture(
                available,
                menuBackgroundTexture,
                ScaleMode.ScaleAndCrop,
                true
            );
        }
        GUI.DrawTexture(
            art,
            characterCollectionTexture,
            ScaleMode.StretchToFill,
            true
        );
        DrawUnifiedBackButton(art);

        GUIStyle header = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.06f, 20f, 35f))
        );
        GUIStyle nameStyle = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.043f, 18f, 29f))
        );
        GUIStyle priceStyle = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.040f, 17f, 27f))
        );

        DrawOutlinedLabel(
            NormalizedRect(art, 0.23f, 0.035f, 0.54f, 0.06f),
            T("characters"),
            header
        );
        DrawOutlinedLabel(
            NormalizedRect(art, 0.79f, 0.029f, 0.15f, 0.04f),
            EconomyProgress.Coins.ToString(),
            priceStyle
        );
        Rect rewardedCoinsButton = NormalizedRect(
            art, 0.67f, 0.078f, 0.29f, 0.045f
        );
        DrawOutlinedLabel(
            rewardedCoinsButton,
            "\u25B6  +100 " + T("coin"),
            priceStyle
        );
        if (InvisibleButton(rewardedCoinsButton))
        {
            TryShowCoinRewardAd();
        }

        float[] columns = { 0.195f, 0.515f };
        float[] rows = { 0.135f, 0.348f, 0.561f, 0.768f };
        for (int index = 0; index < ChampionCatalog.All.Length; index++)
        {
            ChampionTheme champion = ChampionCatalog.All[index];
            int column = index % 2;
            int row = index / 2;
            float x = columns[column];
            float y = rows[row];
            float lowerRowTextOffset = row == 0
                ? 0f
                : row == 1
                    ? -0.006f
                    : -0.012f;
            bool unlocked = ChampionProgress.IsUnlocked(champion);
            bool selected = ChampionProgress.Selected.Id == champion.Id;

            Texture2D portrait = Resources.Load<Texture2D>(
                "BlockArena/Champions/" + GetChampionResourceName(champion.Id)
            );
            Color previous = GUI.color;
            GUI.color = unlocked
                ? Color.white
                : new Color(0.28f, 0.28f, 0.32f, 0.72f);
            DrawCharacterTexture(
                NormalizedRect(art, x + 0.008f, y + 0.006f, 0.269f, 0.125f),
                portrait
            );
            GUI.color = previous;

            DrawOutlinedLabel(
                NormalizedRect(
                    art,
                    x + 0.018f,
                    y + 0.126f + lowerRowTextOffset,
                    0.249f,
                    0.028f
                ),
                GetChampionShortName(champion.Id),
                nameStyle
            );

            string action;
            if (selected)
            {
                action = "\u2713 " + T("selected");
            }
            else if (unlocked)
            {
                action = T("select");
            }
            else if (champion.UnlockKind == ChampionUnlockKind.Stars)
            {
                action = "\u2605 " + champion.UnlockAmount;
            }
            else
            {
                action = champion.UnlockAmount.ToString();
            }
            DrawOutlinedLabel(
                NormalizedRect(
                    art,
                    x + 0.075f,
                    y + 0.157f + lowerRowTextOffset,
                    0.17f,
                    0.03f
                ),
                action,
                priceStyle
            );

            Rect hit = NormalizedRect(art, x, y, 0.285f, 0.20f);
            if (!InvisibleButton(hit) || selected)
            {
                continue;
            }

            if (!unlocked && !ChampionProgress.TryUnlock(champion))
            {
                shopMessage = T("not_enough_coins");
                continue;
            }

            ChampionProgress.TrySelect(champion);
            selectedChampionTexture = portrait;
            shopMessage = GetChampionShortName(champion.Id) +
                " " + T("selected");
        }

        if (!string.IsNullOrEmpty(shopMessage))
        {
            DrawOutlinedLabel(
                NormalizedRect(art, 0.25f, 0.105f, 0.50f, 0.035f),
                shopMessage,
                priceStyle
            );
        }

        if (InvisibleButton(NormalizedRect(art, 0.025f, 0.02f, 0.15f, 0.08f)))
        {
            currentPage = MenuPage.Modes;
            shopMessage = "";
        }
    }

    private void DrawLegacyChampionCollectionPage(
        GUIStyle titleStyle,
        GUIStyle buttonStyle
    )
    {
        Rect panel = GetPanelRect(650f);
        DrawPanel(panel);
        DrawTitle(panel, "\u015EAMP\u0130YONLAR", titleStyle);
        DrawEconomyBalance(panel);

        Rect view = new Rect(
            panel.x + 24f,
            panel.y + 100f,
            panel.width - 48f,
            panel.height - 220f
        );
        collectionScrollPosition = GUI.BeginScrollView(
            view,
            collectionScrollPosition,
            new Rect(0f, 0f, view.width - 20f, 4f * 162f)
        );

        for (int index = 0; index < ChampionCatalog.All.Length; index++)
        {
            ChampionTheme champion = ChampionCatalog.All[index];
            bool unlocked = ChampionProgress.IsUnlocked(champion);
            bool selected = ChampionProgress.Selected.Id == champion.Id;
            float cardWidth = (view.width - 36f) * 0.5f;
            int column = index % 2;
            int row = index / 2;
            Rect card = new Rect(
                4f + column * (cardWidth + 12f),
                row * 162f,
                cardWidth,
                150f
            );

            if (sharedPanelTexture != null)
            {
                GUI.DrawTexture(card, sharedPanelTexture, ScaleMode.StretchToFill, true);
            }

            string action = selected
                ? "SE\u00C7\u0130L\u0130"
                : unlocked
                    ? "SE\u00C7"
                    : GetChampionUnlockLabel(champion);

            if (GUI.Button(card, GUIContent.none, GUIStyle.none))
            {
                if (selected)
                {
                    continue;
                }

                if (!unlocked && !ChampionProgress.TryUnlock(champion))
                {
                    shopMessage = champion.UnlockKind == ChampionUnlockKind.Coins
                        ? "YETERL\u0130 JETONUN YOK"
                        : "DAHA FAZLA YILDIZ KAZANMALISIN";
                    continue;
                }

                ChampionProgress.TrySelect(champion);
                selectedChampionTexture = Resources.Load<Texture2D>(
                    "BlockArena/Champions/" + GetChampionResourceName(champion.Id)
                );
                shopMessage = GetChampionShortName(champion.Id) +
                    " SE\u00C7\u0130LD\u0130";
            }

            Texture2D portrait = Resources.Load<Texture2D>(
                "BlockArena/Champions/" + GetChampionResourceName(champion.Id)
            );
            Color portraitColor = GUI.color;
            GUI.color = unlocked ? Color.white : new Color(0.38f, 0.38f, 0.38f, 0.82f);
            DrawCharacterTexture(
                new Rect(card.x + 18f, card.y + 5f, card.width - 36f, 88f),
                portrait
            );
            GUI.color = portraitColor;

            GUIStyle cardLabel = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                wordWrap = true
            };
            cardLabel.normal.textColor = unlocked
                ? Color.white
                : new Color(0.72f, 0.74f, 0.78f);
            GUI.Label(
                new Rect(card.x + 10f, card.y + 91f, card.width - 20f, 52f),
                GetChampionShortName(champion.Id) + "\n" + action,
                cardLabel
            );
        }
        GUI.EndScrollView();

        GUIStyle messageStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16,
            wordWrap = true
        };
        messageStyle.normal.textColor = Color.white;
        GUI.Label(
            new Rect(panel.x + 25f, panel.yMax - 125f, panel.width - 50f, 42f),
            string.IsNullOrEmpty(shopMessage)
                ? "KARAKTER\u0130N\u0130 SE\u00C7, KEND\u0130 ENGEL\u0130YLE ARENAYA \u00C7IK"
                : shopMessage,
            messageStyle
        );

        if (GUI.Button(
                new Rect(panel.x + 30f, panel.yMax - 178f, panel.width - 60f, 44f),
                "\u25B6  REKLAM \u0130ZLE  •  +100 JETON",
                buttonStyle
            ))
        {
            TryShowCoinRewardAd();
        }

        DrawBackButton(panel, buttonStyle);
    }

    private void TryShowCoinRewardAd()
    {
        shopMessage = "REKLAM HAZIRLANIYOR...";
        if (!LevelPlayAdService.TryShowRewarded(
                earned =>
                {
                    if (earned)
                    {
                        EconomyProgress.GrantCoins(100);
                        shopMessage = "+100 JETON KAZANDIN";
                    }
                    else
                    {
                        shopMessage = "ÖDÜL İÇİN REKLAMI TAMAMLA";
                    }
                },
                "shop_100_coins"
            ))
        {
            shopMessage = "REKLAM ŞU ANDA HAZIR DEĞİL";
        }
    }

    private static string GetChampionUnlockLabel(ChampionTheme champion)
    {
        switch (champion.UnlockKind)
        {
            case ChampionUnlockKind.Stars:
                return champion.UnlockAmount + " YILDIZDA A\u00C7ILIR";
            case ChampionUnlockKind.Coins:
                return champion.UnlockAmount + " JETONLA A\u00C7";
            default:
                return "SE\u00C7";
        }
    }

    private void DrawLegacyCollectionPage(
        GUIStyle titleStyle,
        GUIStyle buttonStyle
    )
    {
        Rect panel = GetPanelRect(650f);
        DrawPanel(panel);
        DrawTitle(panel, "KOLEKSİYONUM", titleStyle);

        string[] categoryLabels = { "KARAKTER", "TAHTA", "ENGEL", "EFEKT" };
        for (int index = 0; index < categoryLabels.Length; index++)
        {
            if (GUI.Button(
                    new Rect(
                        panel.x + 20f + index * ((panel.width - 40f) / 4f),
                        panel.y + 86f,
                        (panel.width - 58f) / 4f,
                        48f
                    ),
                    categoryLabels[index],
                    buttonStyle
                ))
            {
                shopCategory = (CosmeticCategory)index;
                shopMessage = "";
            }
        }

        int ownedCount = 0;
        foreach (CosmeticItem item in CosmeticProgress.Items)
        {
            if (item.Category == shopCategory && CosmeticProgress.IsOwned(item))
            {
                ownedCount++;
            }
        }

        Rect collectionView = new Rect(
            panel.x + 24f,
            panel.y + 148f,
            panel.width - 48f,
            panel.height - 285f
        );
        collectionScrollPosition = GUI.BeginScrollView(
            collectionView,
            collectionScrollPosition,
            new Rect(0f, 0f, collectionView.width - 20f, ownedCount * 68f)
        );

        int ownedIndex = 0;
        foreach (CosmeticItem item in CosmeticProgress.Items)
        {
            if (item.Category != shopCategory ||
                !CosmeticProgress.IsOwned(item))
            {
                continue;
            }

            bool equipped = CosmeticProgress.IsEquipped(item);
            if (GUI.Button(
                    new Rect(
                        4f,
                        ownedIndex * 68f,
                        collectionView.width - 32f,
                        56f
                    ),
                    $"{item.Name}  •  {(equipped ? "KULLANILIYOR" : "KULLAN")}",
                    buttonStyle
                ))
            {
                CosmeticProgress.Equip(item);
                shopMessage = $"{item.Name} KULLANILIYOR";
            }

            ownedIndex++;
        }
        GUI.EndScrollView();

        GUIStyle messageStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16,
            wordWrap = true
        };
        messageStyle.normal.textColor = Color.white;
        GUI.Label(
            new Rect(panel.x + 25f, panel.yMax - 125f, panel.width - 50f, 42f),
            string.IsNullOrEmpty(shopMessage)
                ? "SATIN ALDIĞIN GÖRÜNÜMLERİ BURADAN DEĞİŞTİREBİLİRSİN"
                : shopMessage,
            messageStyle
        );

        DrawBackButton(panel, buttonStyle);
    }

    private void DrawShopPage(GUIStyle titleStyle, GUIStyle buttonStyle)
    {
        Rect panel = GetPanelRect(650f);
        DrawPanel(panel);
        DrawTitle(panel, "MAĞAZA", titleStyle);
        DrawEconomyBalance(panel);

        string[] categoryLabels = { "KARAKTER", "TAHTA", "ENGEL", "EFEKT" };
        for (int index = 0; index < categoryLabels.Length; index++)
        {
            if (GUI.Button(
                    new Rect(
                        panel.x + 20f + index * ((panel.width - 40f) / 4f),
                        panel.y + 96f,
                        (panel.width - 58f) / 4f,
                        48f
                    ),
                    categoryLabels[index],
                    buttonStyle
                ))
            {
                shopCategory = (CosmeticCategory)index;
                shopMessage = "";
            }
        }

        int itemCount = 0;
        foreach (CosmeticItem item in CosmeticProgress.Items)
        {
            if (item.Category == shopCategory)
            {
                itemCount++;
            }
        }

        Rect shopView = new Rect(
            panel.x + 24f,
            panel.y + 158f,
            panel.width - 48f,
            panel.height - 292f
        );
        shopScrollPosition = GUI.BeginScrollView(
            shopView,
            shopScrollPosition,
            new Rect(0f, 0f, shopView.width - 20f, itemCount * 68f)
        );

        int itemIndex = 0;
        foreach (CosmeticItem item in CosmeticProgress.Items)
        {
            if (item.Category != shopCategory)
            {
                continue;
            }

            string state;
            if (!CosmeticProgress.IsLevelUnlocked(item))
            {
                state = $"BÖLÜM {item.RequiredLevel} GEREKLİ";
            }
            else if (CosmeticProgress.IsEquipped(item))
            {
                state = "SEÇİLİ";
            }
            else if (CosmeticProgress.IsOwned(item))
            {
                state = "SEÇ";
            }
            else
            {
                state = $"{item.Price} JETON";
            }

            GUI.enabled = CosmeticProgress.IsLevelUnlocked(item);
            if (GUI.Button(
                    new Rect(
                        4f,
                        itemIndex * 68f,
                        shopView.width - 32f,
                        56f
                    ),
                    $"{item.Name}  •  {state}",
                    buttonStyle
                ))
            {
                HandleShopItem(item);
            }
            GUI.enabled = true;
            itemIndex++;
        }
        GUI.EndScrollView();

        GUIStyle messageStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16
        };
        messageStyle.normal.textColor = Color.white;
        GUI.Label(
            new Rect(panel.x + 25f, panel.yMax - 122f, panel.width - 50f, 38f),
            shopMessage,
            messageStyle
        );
        DrawBackButton(panel, buttonStyle);
    }

    private void HandleShopItem(CosmeticItem item)
    {
        if (CosmeticProgress.IsOwned(item))
        {
            CosmeticProgress.Equip(item);
            shopMessage = $"{item.Name} SEÇİLDİ";
            return;
        }

        if (CosmeticProgress.TryPurchase(item))
        {
            CosmeticProgress.Equip(item);
            shopMessage = $"{item.Name} SATIN ALINDI VE SEÇİLDİ";
        }
        else
        {
            shopMessage = "YETERLİ JETONUN YOK";
        }
    }

    private void DrawDifficultyPage(
        GUIStyle titleStyle,
        GUIStyle buttonStyle
    )
    {
        Rect panel = GetPanelRect(580f);
        DrawPanel(panel);
        DrawTitle(panel, T("choose_difficulty"), titleStyle);

        for (int index = 0;
             index < DifficultyLabels.Length;
             index++)
        {
            if (DrawButton(
                    panel,
                    index,
                    GetLocalizedDifficultyLabel((AIController.Difficulty)index),
                    buttonStyle
                ))
            {
                GameProgression.StartStandardGame(
                    (AIController.Difficulty)index
                );
                SceneManager.LoadScene("Game");
            }
        }

        DrawBackButton(panel, buttonStyle);
    }

    private void DrawLevelsPage(GUIStyle titleStyle, GUIStyle buttonStyle)
    {
        if (levelsMapTexture == null)
        {
            DrawLegacyLevelsPage(titleStyle, buttonStyle);
            return;
        }

        Rect safe = Screen.safeArea;
        Rect available = new Rect(
            safe.x,
            Screen.height - safe.yMax,
            safe.width,
            safe.height
        );
        float sourceAspect = levelsMapTexture.width /
            (float)levelsMapTexture.height;
        bool fitHeight = available.width / available.height > sourceAspect;
        float artWidth = fitHeight
            ? available.height * sourceAspect
            : available.width;
        float artHeight = fitHeight
            ? available.height
            : available.width / sourceAspect;
        Rect art = new Rect(
            available.x + (available.width - artWidth) * 0.5f,
            available.y + (available.height - artHeight) * 0.5f,
            artWidth,
            artHeight
        );

        if (menuBackgroundTexture != null)
        {
            GUI.DrawTexture(
                available,
                menuBackgroundTexture,
                ScaleMode.ScaleAndCrop,
                true
            );
        }
        GUI.DrawTexture(art, levelsMapTexture, ScaleMode.StretchToFill, true);
        DrawUnifiedBackButton(art);

        const int levelsPerPage = 13;
        int pageCount = Mathf.CeilToInt(
            GameProgression.MaximumLevel / (float)levelsPerPage
        );
        levelPage = Mathf.Clamp(levelPage, 0, Mathf.Max(0, pageCount - 1));
        int firstLevel = levelPage * levelsPerPage + 1;
        int lastLevel = Mathf.Min(
            firstLevel + levelsPerPage - 1,
            GameProgression.MaximumLevel
        );

        GUIStyle header = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.062f, 20f, 36f))
        );
        GUIStyle node = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.052f, 17f, 29f))
        );
        GUIStyle stars = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.043f, 20f, 31f))
        );
        SetStyleTextColor(stars, new Color(1f, 0.76f, 0.04f, 1f));
        GUIStyle counter = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.038f, 14f, 21f))
        );

        DrawOutlinedLabel(
            NormalizedRect(art, 0.23f, 0.055f, 0.54f, 0.105f),
            $"{T("levels")}\n{firstLevel}-{lastLevel}",
            header
        );
        DrawOutlinedLabel(
            NormalizedRect(art, 0.80f, 0.035f, 0.13f, 0.04f),
            EconomyProgress.Coins.ToString(),
            counter
        );

        Vector2[] centers =
        {
            new Vector2(0.2895f, 0.2487f), new Vector2(0.4956f, 0.2489f),
            new Vector2(0.7032f, 0.2494f), new Vector2(0.2906f, 0.3579f),
            new Vector2(0.5080f, 0.3580f), new Vector2(0.7260f, 0.3580f),
            new Vector2(0.2749f, 0.4814f), new Vector2(0.5081f, 0.4814f),
            new Vector2(0.7421f, 0.4814f), new Vector2(0.2532f, 0.6299f),
            new Vector2(0.5068f, 0.6299f), new Vector2(0.7620f, 0.6299f),
            new Vector2(0.4360f, 0.7705f)
        };
        float[] starRows =
        {
            0.302f, 0.302f, 0.302f,
            0.415f, 0.415f, 0.415f,
            0.553f, 0.553f, 0.553f,
            0.709f, 0.709f, 0.709f,
            0.839f
        };

        for (int index = 0; index < levelsPerPage; index++)
        {
            int level = firstLevel + index;
            if (level > GameProgression.MaximumLevel)
            {
                break;
            }

            Vector2 center = centers[index];
            bool unlocked = level <= GameProgression.UnlockedLevel;
            bool current = level == GameProgression.UnlockedLevel;
            Color oldNodeColor = node.normal.textColor;
            node.normal.textColor = !unlocked
                ? new Color(0.48f, 0.5f, 0.56f)
                : current
                    ? new Color(0.55f, 1f, 1f)
                    : new Color(1f, 0.95f, 0.68f);
            Rect levelNodeRect = NormalizedRect(
                art,
                center.x - 0.075f,
                center.y - 0.035f,
                0.15f,
                0.07f
            );
            if (unlocked)
            {
                DrawOutlinedLabel(levelNodeRect, level.ToString(), node);
            }
            else
            {
                DrawLevelLock(levelNodeRect);
            }
            node.normal.textColor = oldNodeColor;

            int earnedStars = EconomyProgress.GetLevelStars(level);
            for (int starIndex = 0; starIndex < earnedStars; starIndex++)
            {
                float starX = center.x + (starIndex - 1) * 0.036f;
                DrawOutlinedLabel(
                    NormalizedRect(
                        art,
                        starX - 0.025f,
                        starRows[index] - 0.021f,
                        0.05f,
                        0.042f
                    ),
                    "\u2605",
                    stars
                );
            }

            Rect hit = NormalizedRect(
                art,
                center.x - 0.09f,
                center.y - 0.05f,
                0.18f,
                0.10f
            );
            if (unlocked && InvisibleButton(hit))
            {
                GameProgression.StartLevel(level);
                SceneManager.LoadScene("Game");
            }
        }

        if (levelPage > 0)
        {
            DrawOutlinedLabel(
                NormalizedRect(art, 0.25f, 0.15f, 0.10f, 0.05f),
                "\u25C0",
                header
            );
            if (InvisibleButton(NormalizedRect(art, 0.20f, 0.13f, 0.20f, 0.09f)))
            {
                levelPage--;
            }
        }
        if (levelPage < pageCount - 1)
        {
            DrawOutlinedLabel(
                NormalizedRect(art, 0.65f, 0.15f, 0.10f, 0.05f),
                "\u25B6",
                header
            );
            if (InvisibleButton(NormalizedRect(art, 0.60f, 0.13f, 0.20f, 0.09f)))
            {
                levelPage++;
            }
        }

        DrawOutlinedLabel(
            NormalizedRect(art, 0.20f, 0.865f, 0.60f, 0.055f),
            T("back"),
            header
        );
        bool bottomBack = InvisibleButton(
            NormalizedRect(art, 0.15f, 0.90f, 0.70f, 0.085f)
        );
        bool topBack = InvisibleButton(
            NormalizedRect(art, 0.02f, 0.025f, 0.15f, 0.09f)
        );
        if (bottomBack || topBack)
        {
            currentPage = MenuPage.Modes;
        }
    }

    private void DrawLegacyLevelsPage(
        GUIStyle titleStyle,
        GUIStyle buttonStyle
    )
    {
        Rect safeArea = Screen.safeArea;
        float width = Mathf.Min(820f, safeArea.width - 30f);
        float height = Mathf.Min(1250f, safeArea.height - 40f);
        Rect panel = new Rect(
            safeArea.x + (safeArea.width - width) * 0.5f,
            Screen.height - safeArea.yMax +
            (safeArea.height - height) * 0.5f,
            width,
            height
        );

        DrawPanel(panel);
        DrawTitle(
            panel,
            "BÖLÜMLER",
            titleStyle
        );

        Rect viewRect = new Rect(
            panel.x + 24f,
            panel.y + 78f,
            panel.width - 48f,
            panel.height - 168f
        );
        int columns = Mathf.Max(3, Mathf.FloorToInt(viewRect.width / 120f));
        int rows = Mathf.CeilToInt(
            GameProgression.MaximumLevel / (float)columns
        );
        Rect contentRect = new Rect(
            0f,
            0f,
            viewRect.width - 20f,
            rows * 86f
        );

        levelScrollPosition = GUI.BeginScrollView(
            viewRect,
            levelScrollPosition,
            contentRect
        );

        GUIStyle completedStyle = new GUIStyle(buttonStyle);
        Color completedColor = new Color(0.3f, 1f, 0.45f);
        completedStyle.normal.textColor = completedColor;
        completedStyle.hover.textColor = completedColor;
        completedStyle.active.textColor = completedColor;
        completedStyle.focused.textColor = completedColor;
        GUIStyle currentStyle = new GUIStyle(buttonStyle);
        currentStyle.normal.textColor = new Color(0.18f, 0.95f, 1f);
        currentStyle.hover.textColor = currentStyle.normal.textColor;
        currentStyle.active.textColor = Color.white;

        for (int level = 1;
             level <= GameProgression.MaximumLevel;
             level++)
        {
            int zeroBased = level - 1;
            int column = zeroBased % columns;
            int row = zeroBased / columns;
            bool unlocked = level <= GameProgression.UnlockedLevel;
            bool completed =
                level <= GameProgression.HighestCompletedLevel;
            bool current = level == GameProgression.UnlockedLevel;

            GUI.enabled = unlocked;
            int stars = EconomyProgress.GetLevelStars(level);
            string label = stars > 0
                ? $"{level}\n{new string('★', stars)}"
                : unlocked
                    ? level.ToString()
                    : level + "\nK\u0130L\u0130TL\u0130";

            if (GUI.Button(
                    new Rect(column * 116f, row * 86f, 104f, 70f),
                    label,
                    completed ? completedStyle : current ? currentStyle : buttonStyle
                ))
            {
                GameProgression.StartLevel(level);
                SceneManager.LoadScene("Game");
            }

            if (current)
            {
                DrawCharacterTexture(
                    new Rect(column * 116f + 70f, row * 86f - 8f, 46f, 50f),
                    selectedChampionTexture
                );
            }
        }

        GUI.enabled = true;
        GUI.EndScrollView();

        if (GUI.Button(
                new Rect(
                    panel.x + 30f,
                    panel.yMax - 76f,
                    panel.width - 60f,
                    54f
                ),
                T("back"),
                buttonStyle
            ))
        {
            currentPage = MenuPage.Modes;
        }
    }

    private static void DrawEconomyBalance(Rect panel)
    {
        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 17,
            fontStyle = FontStyle.Bold
        };
        style.normal.textColor = new Color(1f, 0.83f, 0.25f);

        GUI.Label(
            new Rect(panel.x + 20f, panel.y + 58f, panel.width - 40f, 34f),
            $"{EconomyProgress.Coins} JETON  •  {EconomyProgress.TotalStars} YILDIZ",
            style
        );
    }

    private void DrawPvpPage(
        GUIStyle titleStyle,
        GUIStyle buttonStyle
    )
    {
        if (pvpTexture == null)
        {
            DrawLegacyPvpPage(titleStyle, buttonStyle);
            return;
        }

        Rect safe = Screen.safeArea;
        Rect available = new Rect(
            safe.x,
            Screen.height - safe.yMax,
            safe.width,
            safe.height
        );
        float sourceAspect = pvpTexture.width / (float)pvpTexture.height;
        bool fitHeight = available.width / available.height > sourceAspect;
        float artWidth = fitHeight
            ? available.height * sourceAspect
            : available.width;
        float artHeight = fitHeight
            ? available.height
            : available.width / sourceAspect;
        Rect art = new Rect(
            available.x + (available.width - artWidth) * 0.5f,
            available.y + (available.height - artHeight) * 0.5f,
            artWidth,
            artHeight
        );

        if (menuBackgroundTexture != null)
        {
            GUI.DrawTexture(available, menuBackgroundTexture, ScaleMode.ScaleAndCrop, true);
        }
        GUI.DrawTexture(art, pvpTexture, ScaleMode.StretchToFill, true);
        DrawUnifiedBackButton(art);

        GUIStyle header = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.075f, 24f, 42f))
        );
        GUIStyle statusStyle = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.045f, 16f, 27f))
        );
        GUIStyle actionStyle = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.050f, 17f, 30f))
        );

        DrawOutlinedLabel(NormalizedRect(art, 0.22f, 0.105f, 0.56f, 0.085f), "PVP", header);

        DrawCharacterTexture(
            NormalizedRect(art, 0.075f, 0.315f, 0.33f, 0.235f),
            selectedChampionTexture
        );
        Texture2D opponentPreview = OnlineMatchmaking.State ==
            OnlineMatchmaking.SearchState.BotReady
                ? ninjaTexture
                : pirateTexture;
        Color oldColor = GUI.color;
        GUI.color = OnlineMatchmaking.State == OnlineMatchmaking.SearchState.Searching
            ? new Color(0.32f, 0.38f, 0.50f, 0.62f)
            : Color.white;
        DrawCharacterTexture(
            NormalizedRect(art, 0.595f, 0.315f, 0.33f, 0.235f),
            opponentPreview
        );
        GUI.color = oldColor;

        string status = GetIllustratedPvpStatus();
        DrawOutlinedLabel(
            NormalizedRect(art, 0.09f, 0.635f, 0.82f, 0.095f),
            status,
            statusStyle
        );

        OnlineMatchmaking.SearchState state = OnlineMatchmaking.State;
        string primaryAction = "";
        string secondaryAction = "";
        Action primary = null;
        Action secondary = null;

        if (OnlineServices.State == OnlineServices.ConnectionState.Failed)
        {
            primaryAction = T("try_again");
            primary = RetryOnlineInitialization;
        }
        else if (OnlineServices.State == OnlineServices.ConnectionState.Ready)
        {
            if (state == OnlineMatchmaking.SearchState.Searching)
            {
                primaryAction = T("cancel_search");
                primary = CancelMatchmaking;
            }
            else if (state == OnlineMatchmaking.SearchState.BotReady)
            {
                primaryAction = T("play_bot");
                primary = StartPvpBotMatch;
                secondaryAction = T("search_again");
                secondary = StartMatchmaking;
            }
            else if (state != OnlineMatchmaking.SearchState.HumanFound)
            {
                primaryAction = T("search_player");
                primary = StartMatchmaking;
            }
        }

        if (!string.IsNullOrEmpty(primaryAction))
        {
            DrawOutlinedLabel(
                NormalizedRect(art, 0.18f, 0.772f, 0.64f, 0.065f),
                primaryAction,
                actionStyle
            );
            if (InvisibleButton(NormalizedRect(art, 0.16f, 0.765f, 0.68f, 0.10f)))
            {
                primary?.Invoke();
            }
        }
        if (!string.IsNullOrEmpty(secondaryAction))
        {
            DrawOutlinedLabel(
                NormalizedRect(art, 0.18f, 0.875f, 0.64f, 0.065f),
                secondaryAction,
                actionStyle
            );
            if (InvisibleButton(NormalizedRect(art, 0.16f, 0.87f, 0.68f, 0.10f)))
            {
                secondary?.Invoke();
            }
        }

        if (InvisibleButton(NormalizedRect(art, 0.018f, 0.014f, 0.145f, 0.085f)))
        {
            CancelMatchmakingAndGoBack();
        }
    }

    private static string GetIllustratedPvpStatus()
    {
        if (OnlineServices.State == OnlineServices.ConnectionState.Failed)
        {
            return T("connection_failed");
        }
        if (OnlineServices.State != OnlineServices.ConnectionState.Ready)
        {
            return T("connecting");
        }

        switch (OnlineMatchmaking.State)
        {
            case OnlineMatchmaking.SearchState.Searching:
                return T("searching") + "\n" +
                    OnlineMatchmaking.SecondsRemaining + " " + T("seconds");
            case OnlineMatchmaking.SearchState.HumanFound:
                return T("player_found") + "\n" +
                    OnlineNetworkRuntime.GetRoleLabel();
            case OnlineMatchmaking.SearchState.BotReady:
                return T("bot_ready");
            case OnlineMatchmaking.SearchState.Failed:
                return T("match_failed");
            default:
                return T("real_rival");
        }
    }

    private void StartPvpBotMatch()
    {
        GameProgression.StartPvpBotGame();
        SceneManager.LoadScene("Game");
    }

    private void DrawLegacyPvpPage(
        GUIStyle titleStyle,
        GUIStyle buttonStyle
    )
    {
        Rect panel = GetPanelRect(520f);
        DrawPanel(panel);
        DrawTitle(panel, "PVP", titleStyle);

        GUIStyle messageStyle = new GUIStyle(titleStyle)
        {
            fontSize = 21,
            wordWrap = true
        };

        GUI.Label(
            new Rect(panel.x + 30f, panel.y + 82f, panel.width - 60f, 125f),
            GetPvpStatusMessage(),
            messageStyle
        );

        DrawCharacterTexture(
            new Rect(panel.x + 28f, panel.y + 112f, 82f, 92f),
            selectedChampionTexture
        );
        Texture2D opponentPreview = OnlineMatchmaking.State ==
            OnlineMatchmaking.SearchState.BotReady
                ? ninjaTexture
                : pirateTexture;
        Color opponentColor = GUI.color;
        GUI.color = OnlineMatchmaking.State == OnlineMatchmaking.SearchState.Searching
            ? new Color(0.35f, 0.45f, 0.6f, 0.65f)
            : Color.white;
        DrawCharacterTexture(
            new Rect(panel.xMax - 110f, panel.y + 112f, 82f, 92f),
            opponentPreview
        );
        GUI.color = opponentColor;

        if (OnlineServices.State == OnlineServices.ConnectionState.Failed &&
            GUI.Button(
                new Rect(panel.x + 30f, panel.y + 215f, panel.width - 60f, 58f),
                "TEKRAR DENE",
                buttonStyle
            ))
        {
            RetryOnlineInitialization();
        }

        if (OnlineServices.State == OnlineServices.ConnectionState.Ready)
        {
            DrawMatchmakingControls(panel, buttonStyle);
        }

        if (GUI.Button(
                new Rect(
                    panel.x + 30f,
                    panel.yMax - 72f,
                    panel.width - 60f,
                    52f
                ),
                "GERİ",
                buttonStyle
            ))
        {
            CancelMatchmakingAndGoBack();
        }
    }

    private static string GetPvpStatusMessage()
    {
        if (OnlineServices.State != OnlineServices.ConnectionState.Ready)
        {
            return $"İNTERNET ÜZERİNDEN PVP\n{OnlineServices.StatusMessage}";
        }

        string status = OnlineMatchmaking.StatusMessage;

        if (OnlineMatchmaking.State ==
            OnlineMatchmaking.SearchState.Searching)
        {
            status += $"\n{OnlineMatchmaking.SecondsRemaining} SANİYE";
        }

        return $"İNTERNET ÜZERİNDEN PVP\n{status}";
    }

    private void DrawMatchmakingControls(
        Rect panel,
        GUIStyle buttonStyle
    )
    {
        OnlineMatchmaking.SearchState state = OnlineMatchmaking.State;

        if (state == OnlineMatchmaking.SearchState.Searching)
        {
            if (GUI.Button(
                    new Rect(panel.x + 30f, panel.y + 230f, panel.width - 60f, 62f),
                    "ARAMAYI İPTAL ET",
                    buttonStyle
                ))
            {
                CancelMatchmaking();
            }

            return;
        }

        if (state == OnlineMatchmaking.SearchState.BotReady)
        {
            if (GUI.Button(
                    new Rect(panel.x + 30f, panel.y + 215f, panel.width - 60f, 62f),
                    "BOTLA OYNA (İMKÂNSIZ)",
                    buttonStyle
                ))
            {
                GameProgression.StartPvpBotGame();
                SceneManager.LoadScene("Game");
            }

            if (GUI.Button(
                    new Rect(panel.x + 30f, panel.y + 295f, panel.width - 60f, 62f),
                    "TEKRAR RAKİP ARA",
                    buttonStyle
                ))
            {
                StartMatchmaking();
            }

            return;
        }

        if (state == OnlineMatchmaking.SearchState.HumanFound)
        {
            GUI.Label(
                new Rect(panel.x + 30f, panel.y + 215f, panel.width - 60f, 70f),
                $"OYUNCU OTURUMA BAĞLANDI\n" +
                OnlineNetworkRuntime.GetRoleLabel(),
                CreateTitleStyle()
            );
            return;
        }

        if (GUI.Button(
                new Rect(panel.x + 30f, panel.y + 215f, panel.width - 60f, 62f),
                "RAKİP ARA",
                buttonStyle
            ))
        {
            StartMatchmaking();
        }
    }

    private async void StartMatchmaking()
    {
        await OnlineMatchmaking.StartSearchAsync();
    }

    private async void CancelMatchmaking()
    {
        await OnlineMatchmaking.CancelSearchAsync();
    }

    private async void CancelMatchmakingAndGoBack()
    {
        await OnlineMatchmaking.CancelSearchAsync();
        currentPage = MenuPage.Modes;
    }

    private async void BeginOnlineInitialization()
    {
        if (onlineInitializationStarted)
        {
            return;
        }

        onlineInitializationStarted = true;
        await OnlineServices.InitializeAsync();
    }

    private async void RetryOnlineInitialization()
    {
        await OnlineServices.RetryAsync();
    }

    private void DrawHelpPage(
        GUIStyle titleStyle,
        GUIStyle buttonStyle
    )
    {
        if (howToPlayTexture == null)
        {
            DrawLegacyHelpPage(titleStyle, buttonStyle);
            return;
        }

        Rect art = DrawIllustratedPage(howToPlayTexture);
        DrawUnifiedBackButton(art);
        GUIStyle header = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.062f, 21f, 36f))
        );
        GUIStyle stepTitle = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.043f, 16f, 25f))
        );
        GUIStyle stepBody = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            font = readableTextFont,
            fontSize = Mathf.RoundToInt(Mathf.Clamp(art.width * 0.031f, 13f, 19f)),
            fontStyle = FontStyle.Bold,
            wordWrap = true
        };
        stepBody.normal.textColor = Color.white;

        DrawOutlinedLabel(NormalizedRect(art, 0.20f, 0.095f, 0.60f, 0.10f), T("how_to_play"), header);
        string[] titles = { T("move"), T("place_obstacle"), T("trap_rival") };
        string[] bodies =
        {
            T("move_help"),
            T("obstacle_help"),
            T("trap_help")
        };
        float[] yValues = { 0.315f, 0.535f, 0.735f };
        for (int index = 0; index < titles.Length; index++)
        {
            DrawOutlinedLabel(
                NormalizedRect(art, 0.39f, yValues[index], 0.50f, 0.052f),
                titles[index],
                stepTitle
            );
            GUI.Label(
                NormalizedRect(art, 0.39f, yValues[index] + 0.035f, 0.50f, 0.075f),
                bodies[index],
                stepBody
            );
        }

        GUIStyle back = CreateOverlayLabelStyle(
            Mathf.RoundToInt(Mathf.Clamp(art.width * 0.052f, 18f, 30f))
        );
        DrawOutlinedLabel(NormalizedRect(art, 0.30f, 0.906f, 0.40f, 0.05f), T("back"), back);
        if (InvisibleButton(NormalizedRect(art, 0.018f, 0.014f, 0.145f, 0.085f)) ||
            InvisibleButton(NormalizedRect(art, 0.23f, 0.90f, 0.54f, 0.085f)))
        {
            currentPage = helpReturnPage;
        }
    }

    private void DrawLegacyHelpPage(
        GUIStyle titleStyle,
        GUIStyle buttonStyle
    )
    {
        Rect panel = GetPanelRect(580f);
        DrawPanel(panel);
        DrawTitle(panel, "NASIL OYNANIR?", titleStyle);

        GUIStyle textStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.UpperLeft,
            fontSize = 20,
            wordWrap = true,
            richText = true
        };
        textStyle.normal.textColor = Color.white;

        GUI.Label(
            new Rect(panel.x + 38f, panel.y + 95f, panel.width - 76f, 360f),
            "<color=#48E969>YEŞİL KARE:</color> Karakterini hareket ettir.\n\n" +
            "<color=#F05249>KIRMIZI KARE:</color> Bir engel yerleştir.\n\n" +
            "Her tur önce hareket et, sonra engel koy.\n\n" +
            "Rakibini hareket edemeyecek şekilde kapatarak kazan.",
            textStyle
        );

        DrawBackButton(panel, buttonStyle);
    }

    private void DrawUnifiedBackButton(Rect art)
    {
        if (unifiedBackButtonTexture == null)
        {
            return;
        }

        GUI.DrawTextureWithTexCoords(
            NormalizedRect(art, 0.018f, 0.014f, 0.145f, 0.085f),
            unifiedBackButtonTexture,
            new Rect(0.11f, 0.10f, 0.78f, 0.80f),
            true
        );
    }

    private void DrawBackButton(Rect panel, GUIStyle buttonStyle)
    {
        if (GUI.Button(
                new Rect(
                    panel.x + 30f,
                    panel.yMax - 72f,
                    panel.width - 60f,
                    52f
                ),
                T("back"),
                buttonStyle
            ))
        {
            currentPage = MenuPage.Modes;
        }
    }

    private static bool DrawButton(
        Rect panel,
        int index,
        string label,
        GUIStyle style
    )
    {
        return GUI.Button(
            new Rect(
                panel.x + 30f,
                panel.y + 90f + index * 92f,
                panel.width - 60f,
                70f
            ),
            label,
            style
        );
    }

    private static bool DrawModeButton(
        Rect panel,
        int index,
        string label,
        GUIStyle style
    )
    {
        return GUI.Button(
            new Rect(
                panel.x + 30f,
                panel.y + 96f + index * 58f,
                panel.width - 60f,
                48f
            ),
            label,
            style
        );
    }

    private static Rect GetPanelRect(float height)
    {
        Rect safeArea = Screen.safeArea;
        float width = Mathf.Min(480f, safeArea.width - 40f);
        height = Mathf.Min(height, safeArea.height - 30f);
        float safeTop = Screen.height - safeArea.yMax;

        return new Rect(
            safeArea.x + (safeArea.width - width) * 0.5f,
            safeTop + (safeArea.height - height) * 0.5f,
            width,
            height
        );
    }

    private static void DrawPanel(Rect panel)
    {
        if (sharedPanelTexture != null)
        {
            GUI.DrawTexture(panel, sharedPanelTexture, ScaleMode.StretchToFill, true);
            return;
        }

        Color previousColor = GUI.color;
        GUI.color = new Color(0.07f, 0.11f, 0.18f, 0.96f);
        GUI.Box(panel, GUIContent.none);
        GUI.color = previousColor;
    }

    private static void DrawTitle(
        Rect panel,
        string title,
        GUIStyle style
    )
    {
        GUI.Label(
            new Rect(panel.x, panel.y + 18f, panel.width, 52f),
            title,
            style
        );
    }

    private static GUIStyle CreateTitleStyle()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 26,
            fontStyle = FontStyle.Bold
        };
        style.normal.textColor = Color.white;
        return style;
    }

    private static GUIStyle CreateButtonStyle()
    {
        return new GUIStyle(GUI.skin.button)
        {
            fontSize = 21,
            fontStyle = FontStyle.Bold
        };
    }

    private static string T(string key)
    {
        return BlockArenaLocalization.Text(key);
    }

    private static string GetLocalizedMissionTitle(DailyMissionType type)
    {
        switch (type)
        {
            case DailyMissionType.PlayMatches: return T("mission_play");
            case DailyMissionType.WinMatches: return T("mission_win");
            case DailyMissionType.WinCampaignMatch: return T("mission_level");
            case DailyMissionType.PlaceObstacles: return T("mission_obstacle");
            default: return type.ToString();
        }
    }

    private static string GetLocalizedDifficultyLabel(AIController.Difficulty difficulty)
    {
        switch (difficulty)
        {
            case AIController.Difficulty.Easy: return T("easy");
            case AIController.Difficulty.Hard: return T("hard");
            case AIController.Difficulty.Impossible: return T("impossible");
            default: return T("medium");
        }
    }
}
