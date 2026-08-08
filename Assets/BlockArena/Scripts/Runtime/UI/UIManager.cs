using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Arayüz Elemanları")]
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text resultText;

    private string difficultyLabel = "";
    private string matchLabel = "";
    private int roundNumber = 1;
    private Button nextLevelButton;
    private bool gameOverVisible;
    private bool humanWon;
    private bool leaveConfirmationVisible;
    private bool tutorialVisible;
    private bool matchCompletionRecorded;
    private GameObject turnDisplayBackground;

    public bool IsBoardInputBlocked =>
        tutorialVisible ||
        leaveConfirmationVisible ||
        gameOverVisible ||
        LevelPlayAdService.IsFullScreenAdShowing;

    private void Start()
    {
        difficultyLabel = T("medium");
        matchLabel = difficultyLabel;
        ConfigureTurnDisplay();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        tutorialVisible = PlayerPrefs.GetInt(
            GameProgression.TutorialSeenKey,
            0
        ) == 0;

        if (tutorialVisible)
        {
            Time.timeScale = 0f;
        }
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape) ||
            LevelPlayAdService.IsFullScreenAdShowing ||
            tutorialVisible)
        {
            return;
        }

        if (leaveConfirmationVisible)
        {
            leaveConfirmationVisible = false;
            Time.timeScale = 1f;
            return;
        }

        leaveConfirmationVisible = true;
        Time.timeScale = 0f;
    }

    private void ConfigureTurnDisplay()
    {
        if (turnText == null)
        {
            return;
        }

        RectTransform textRect = turnText.rectTransform;
        Canvas canvas = turnText.GetComponentInParent<Canvas>();
        float canvasScale = canvas != null
            ? Mathf.Max(canvas.scaleFactor, 0.01f)
            : 1f;
        float safeTopInset =
            (Screen.height - Screen.safeArea.yMax) / canvasScale;
        float safeY = -72f - safeTopInset;
        textRect.anchorMin = new Vector2(0.5f, 1f);
        textRect.anchorMax = new Vector2(0.5f, 1f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = new Vector2(0f, safeY);
        textRect.sizeDelta = new Vector2(820f, 116f);

        turnText.fontSize = 38f;
        turnText.enableAutoSizing = true;
        turnText.fontSizeMin = 24f;
        turnText.fontSizeMax = 38f;
        turnText.alignment = TextAlignmentOptions.Center;

        GameObject background = new GameObject(
            "TurnDisplayBackground",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );

        RectTransform backgroundRect =
            background.GetComponent<RectTransform>();

        backgroundRect.SetParent(textRect.parent, false);
        backgroundRect.anchorMin = new Vector2(0.5f, 1f);
        backgroundRect.anchorMax = new Vector2(0.5f, 1f);
        backgroundRect.pivot = new Vector2(0.5f, 0.5f);
        backgroundRect.anchoredPosition = new Vector2(0f, safeY);
        backgroundRect.sizeDelta = new Vector2(880f, 132f);
        backgroundRect.SetSiblingIndex(textRect.GetSiblingIndex());

        Image backgroundImage = background.GetComponent<Image>();
        backgroundImage.color = new Color(0.04f, 0.07f, 0.12f, 0.88f);
        backgroundImage.raycastTarget = false;
        turnDisplayBackground = background;
    }

    public void SetMatchInfo(
        AIController.Difficulty difficulty,
        int currentRound
    )
    {
        difficultyLabel = GetDifficultyLabel(difficulty);
        GameProgression.GameMode mode =
            (GameProgression.GameMode)PlayerPrefs.GetInt(
                GameProgression.GameModeKey,
                (int)GameProgression.GameMode.Standard
            );
        int levelNumber = PlayerPrefs.GetInt(
            GameProgression.SelectedLevelKey,
            0
        );
        if (mode == GameProgression.GameMode.Levels)
        {
            matchLabel = $"{T("level")} {levelNumber}";
        }
        else if (mode == GameProgression.GameMode.Pvp &&
                 PlayerPrefs.GetInt(
                     "BlockArena.PvpOpponentIsBot",
                     0
                 ) == 1)
        {
            matchLabel = $"PVP  •  BOT  •  {difficultyLabel}";
        }
        else
        {
            matchLabel = difficultyLabel;
        }
        roundNumber = Mathf.Max(1, currentRound);
    }

    public void ShowHumanMovement()
    {
        SetTurnTextWithMatchInfo(T("your_turn"));
    }

    public void ShowHumanObstacle()
    {
        SetTurnTextWithMatchInfo(T("place_obstacle"));
    }

    public void ShowEnemyTurn()
    {
        SetTurnTextWithMatchInfo(T("rival_thinking"));
    }

    public void ShowHumanWin()
    {
        humanWon = true;
        SetTurnText(T("you_won"));
        ShowGameOver(T("you_won"));
    }

    public void ShowEnemyWin()
    {
        humanWon = false;
        SetTurnText(T("you_lost"));
        ShowGameOver(T("you_lost"));

        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(false);
        }
    }

    public void RestartGame()
    {
        ContinueAfterPossibleAd(
            () => SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            )
        );
    }

    public void GoToMainMenu()
    {
        ContinueAfterPossibleAd(
            () => SceneManager.LoadScene("MainMenu")
        );
    }

    public void GoToNextLevel()
    {
        int currentLevel = PlayerPrefs.GetInt(
            GameProgression.SelectedLevelKey,
            1
        );

        ContinueAfterPossibleAd(() =>
        {
            GameProgression.StartLevel(currentLevel + 1);
            SceneManager.LoadScene("Game");
        });
    }

    private void ConfigureNextLevelButton()
    {
        if (gameOverPanel == null)
        {
            return;
        }

        Button restartButton = null;
        Button mainMenuButton = null;

        foreach (Button button in
                 gameOverPanel.GetComponentsInChildren<Button>(true))
        {
            if (button.name == "RestartButton")
            {
                restartButton = button;
            }
            else if (button.name == "MainMenuButton")
            {
                mainMenuButton = button;
            }
        }

        if (restartButton == null)
        {
            return;
        }

        nextLevelButton = Instantiate(
            restartButton,
            restartButton.transform.parent
        );
        nextLevelButton.name = "NextLevelButton";
        nextLevelButton.onClick.RemoveAllListeners();
        nextLevelButton.onClick.AddListener(GoToNextLevel);

        RectTransform nextRect =
            nextLevelButton.GetComponent<RectTransform>();
        nextRect.anchoredPosition = new Vector2(0f, 48f);
        nextRect.sizeDelta = new Vector2(
            nextRect.sizeDelta.x,
            78f
        );

        RectTransform restartRect =
            restartButton.GetComponent<RectTransform>();
        restartRect.anchoredPosition = new Vector2(0f, -58f);
        restartRect.sizeDelta = new Vector2(
            restartRect.sizeDelta.x,
            78f
        );

        if (mainMenuButton != null)
        {
            RectTransform mainMenuRect =
                mainMenuButton.GetComponent<RectTransform>();
            mainMenuRect.anchoredPosition = new Vector2(0f, -164f);
            mainMenuRect.sizeDelta = new Vector2(
                mainMenuRect.sizeDelta.x,
                78f
            );
        }

        if (resultText != null)
        {
            RectTransform resultRect = resultText.rectTransform;
            resultRect.anchoredPosition = new Vector2(0f, 190f);
            resultRect.sizeDelta = new Vector2(600f, 125f);
            resultText.enableAutoSizing = true;
            resultText.fontSizeMin = 25f;
            resultText.fontSizeMax = 44f;
            resultText.alignment = TextAlignmentOptions.Center;
        }

        TMP_Text label =
            nextLevelButton.GetComponentInChildren<TMP_Text>();

        if (label != null)
        {
            label.text = T("next_level");
        }

        nextLevelButton.gameObject.SetActive(false);
    }

    private void ShowNextLevelButton()
    {
        if (nextLevelButton == null)
        {
            return;
        }

        GameProgression.GameMode mode =
            (GameProgression.GameMode)PlayerPrefs.GetInt(
                GameProgression.GameModeKey,
                (int)GameProgression.GameMode.Standard
            );
        int currentLevel = PlayerPrefs.GetInt(
            GameProgression.SelectedLevelKey,
            0
        );

        nextLevelButton.gameObject.SetActive(
            mode == GameProgression.GameMode.Levels &&
            currentLevel > 0 &&
            currentLevel < GameProgression.MaximumLevel
        );
    }

    private void SetTurnTextWithMatchInfo(string message)
    {
        SetTurnText(
            $"{message}\n{matchLabel}"
        );
    }

    private void SetTurnText(string message)
    {
        if (turnText == null)
        {
            Debug.LogError("UIManager içindeki Turn Text alanı boş.");
            return;
        }

        turnText.text = message;
    }

    private void ShowGameOver(string result)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        gameOverVisible = true;

        if (turnText != null)
        {
            turnText.gameObject.SetActive(false);
        }

        if (turnDisplayBackground != null)
        {
            turnDisplayBackground.SetActive(false);
        }

        if (!matchCompletionRecorded)
        {
            matchCompletionRecorded = true;
            AdFrequencyPolicy.RecordCompletedMatch();
            GameProgression.GameMode mode =
                (GameProgression.GameMode)PlayerPrefs.GetInt(
                    GameProgression.GameModeKey,
                    (int)GameProgression.GameMode.Standard
                );
            DailyMissionProgress.RecordCompletedMatch(
                humanWon,
                mode == GameProgression.GameMode.Levels
            );
        }
    }

    private static void ContinueAfterPossibleAd(System.Action continuation)
    {
        InterstitialAdController.ContinueAfterPossibleAd(continuation);
    }

    private void OnGUI()
    {
        if (LevelPlayAdService.IsFullScreenAdShowing)
        {
            return;
        }

        GUI.depth = -1000;

        if (tutorialVisible)
        {
            DrawTutorial();
            return;
        }

        if (leaveConfirmationVisible)
        {
            DrawLeaveConfirmation();
            return;
        }

        if (!gameOverVisible)
        {
            DrawLeaveGameButton();

            if (leaveConfirmationVisible)
            {
                DrawLeaveConfirmation();
                return;
            }
        }

        if (!gameOverVisible)
        {
            return;
        }

        bool showNext = humanWon && IsNextLevelAvailable();
        int buttonCount = showNext ? 3 : 2;
        Rect safeArea = Screen.safeArea;
        float width = Mathf.Min(620f, safeArea.width - 40f);
        float height = 205f + buttonCount * 82f;
        float safeTop = Screen.height - safeArea.yMax;

        Color previousColor = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.68f);
        GUI.DrawTexture(
            new Rect(0f, 0f, Screen.width, Screen.height),
            Texture2D.whiteTexture
        );
        GUI.color = previousColor;

        Rect panel = new Rect(
            safeArea.x + (safeArea.width - width) * 0.5f,
            safeTop + (safeArea.height - height) * 0.5f,
            width,
            height
        );

        GUI.color = new Color(0.04f, 0.06f, 0.09f, 0.97f);
        GUI.Box(panel, GUIContent.none);
        GUI.color = previousColor;

        GUIStyle resultStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 29,
            fontStyle = FontStyle.Bold
        };
        resultStyle.normal.textColor = Color.white;

        string result = humanWon ? T("you_won") : T("you_lost");
        string reward = "";
        if (humanWon && EconomyProgress.LastCoinsEarned > 0)
        {
            reward = $"\n+{EconomyProgress.LastCoinsEarned} {T("coin")}";
            if (EconomyProgress.LastStarsEarned > 0)
            {
                reward += $"  •  +{EconomyProgress.LastStarsEarned} {T("star")}";
            }
        }
        GUI.Label(
            new Rect(panel.x + 20f, panel.y + 18f, panel.width - 40f, 125f),
            $"{result}\n{matchLabel}{reward}",
            resultStyle
        );

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };
        float nextY = panel.y + 150f;

        if (showNext)
        {
            if (DrawGameOverButton(
                    panel,
                    ref nextY,
                    T("next_level"),
                    buttonStyle
                ))
            {
                GoToNextLevel();
            }
        }

        if (DrawGameOverButton(
                panel,
                ref nextY,
                T("retry"),
                buttonStyle
            ))
        {
            RestartGame();
        }

        if (DrawGameOverButton(
                panel,
                ref nextY,
                T("main_menu"),
                buttonStyle
            ))
        {
            GoToMainMenu();
        }
    }

    private void DrawTutorial()
    {
        Rect safeArea = Screen.safeArea;
        float width = Mathf.Min(560f, safeArea.width - 40f);
        float height = 500f;
        float safeTop = Screen.height - safeArea.yMax;
        Rect panel = new Rect(
            safeArea.x + (safeArea.width - width) * 0.5f,
            safeTop + (safeArea.height - height) * 0.5f,
            width,
            height
        );

        Color previousColor = GUI.color;
        GUI.color = new Color(0.04f, 0.06f, 0.09f, 0.98f);
        GUI.Box(panel, GUIContent.none);
        GUI.color = previousColor;

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 28,
            fontStyle = FontStyle.Bold
        };
        titleStyle.normal.textColor = Color.white;

        GUI.Label(
            new Rect(panel.x + 20f, panel.y + 20f, panel.width - 40f, 55f),
            T("how_to_play"),
            titleStyle
        );

        GUIStyle textStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.UpperLeft,
            fontSize = 20,
            wordWrap = true,
            richText = true
        };
        textStyle.normal.textColor = Color.white;

        GUI.Label(
            new Rect(panel.x + 38f, panel.y + 95f, panel.width - 76f, 285f),
            "<color=#48E969>" + T("move") + ":</color> " + T("move_help") + "\n\n" +
            "<color=#F05249>" + T("place_obstacle") + ":</color> " + T("obstacle_help") + "\n\n" +
            T("trap_help"),
            textStyle
        );

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };

        if (GUI.Button(
                new Rect(panel.x + 35f, panel.yMax - 88f, panel.width - 70f, 62f),
                T("understood"),
                buttonStyle
            ))
        {
            tutorialVisible = false;
            PlayerPrefs.SetInt(GameProgression.TutorialSeenKey, 1);
            PlayerPrefs.Save();
            Time.timeScale = 1f;
        }
    }

    private void DrawLeaveGameButton()
    {
        GameProgression.GameMode mode =
            (GameProgression.GameMode)PlayerPrefs.GetInt(
                GameProgression.GameModeKey,
                (int)GameProgression.GameMode.Standard
            );
        bool levelMode = mode == GameProgression.GameMode.Levels;
        Rect safeArea = Screen.safeArea;
        float width = Mathf.Min(230f, safeArea.width * 0.42f);
        float safeBottom = Screen.height - safeArea.y;
        Rect buttonRect = new Rect(
            safeArea.x + 14f,
            safeBottom - 68f,
            width,
            54f
        );
        GUIStyle style = new GUIStyle(GUI.skin.button)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold
        };

        if (GUI.Button(
                buttonRect,
                levelMode ? T("return_levels") : T("main_menu"),
                style
            ))
        {
            leaveConfirmationVisible = true;
            Time.timeScale = 0f;
        }
    }

    private void DrawLeaveConfirmation()
    {
        Rect safeArea = Screen.safeArea;
        float width = Mathf.Min(480f, safeArea.width - 40f);
        float height = 300f;
        float safeTop = Screen.height - safeArea.yMax;
        Rect panel = new Rect(
            safeArea.x + (safeArea.width - width) * 0.5f,
            safeTop + (safeArea.height - height) * 0.5f,
            width,
            height
        );

        Color previousColor = GUI.color;
        GUI.color = new Color(0.04f, 0.06f, 0.09f, 0.98f);
        GUI.Box(panel, GUIContent.none);
        GUI.color = previousColor;

        GUIStyle messageStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 23,
            fontStyle = FontStyle.Bold,
            wordWrap = true
        };
        messageStyle.normal.textColor = Color.white;

        GUI.Label(
            new Rect(panel.x + 25f, panel.y + 24f, panel.width - 50f, 90f),
            T("leave_prompt"),
            messageStyle
        );

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 18,
            fontStyle = FontStyle.Bold
        };

        if (GUI.Button(
                new Rect(panel.x + 28f, panel.y + 130f, panel.width - 56f, 58f),
                T("yes_leave"),
                buttonStyle
            ))
        {
            ConfirmLeaveGame();
        }

        if (GUI.Button(
                new Rect(panel.x + 28f, panel.y + 208f, panel.width - 56f, 58f),
                T("cancel"),
                buttonStyle
            ))
        {
            leaveConfirmationVisible = false;
            Time.timeScale = 1f;
        }
    }

    private void ConfirmLeaveGame()
    {
        GameProgression.GameMode mode =
            (GameProgression.GameMode)PlayerPrefs.GetInt(
                GameProgression.GameModeKey,
                (int)GameProgression.GameMode.Standard
            );

        if (mode == GameProgression.GameMode.Levels)
        {
            PlayerPrefs.SetInt(GameProgression.OpenLevelsMenuKey, 1);
            PlayerPrefs.Save();
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDisable()
    {
        if (leaveConfirmationVisible || tutorialVisible)
        {
            Time.timeScale = 1f;
        }
    }

    private static bool DrawGameOverButton(
        Rect panel,
        ref float y,
        string label,
        GUIStyle style
    )
    {
        bool clicked = GUI.Button(
            new Rect(panel.x + 30f, y, panel.width - 60f, 62f),
            label,
            style
        );

        y += 76f;
        return clicked;
    }

    private static bool IsNextLevelAvailable()
    {
        GameProgression.GameMode mode =
            (GameProgression.GameMode)PlayerPrefs.GetInt(
                GameProgression.GameModeKey,
                (int)GameProgression.GameMode.Standard
            );
        int currentLevel = PlayerPrefs.GetInt(
            GameProgression.SelectedLevelKey,
            0
        );

        return mode == GameProgression.GameMode.Levels &&
               currentLevel > 0 &&
               currentLevel < GameProgression.MaximumLevel;
    }

    private static string GetDifficultyLabel(
        AIController.Difficulty difficulty
    )
    {
        switch (difficulty)
        {
            case AIController.Difficulty.Easy:
                return T("easy");
            case AIController.Difficulty.Hard:
                return T("hard");
            case AIController.Difficulty.Impossible:
                return T("impossible");
            default:
                return T("medium");
        }
    }

    private static string T(string key)
    {
        return BlockArenaLocalization.Text(key);
    }

    private static string GetLocalizedArenaName(int number)
    {
        switch (number)
        {
            case 2: return T("arena_forest");
            case 3: return T("arena_ice");
            case 4: return T("arena_lava");
            case 5: return T("arena_space");
            default: return T("arena_start");
        }
    }
}
