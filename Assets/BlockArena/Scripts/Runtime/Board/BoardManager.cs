using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Tahta Ayarları")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private int boardSize = 7;
    [SerializeField] private float tileSpacing = 1.1f;

    [Header("Engel Ayarları")]
    [SerializeField] private float obstacleHeight = 0.55f;

[Header("Yöneticiler")]
[SerializeField] private AIController aiController;
[SerializeField] private UIManager uiManager;

    private Tile[,] tiles;

    private GridMovement humanPlayer;
    private GridMovement enemyPlayer;
    private TurnManager turnManager;

    private bool gameEnded;
    private int roundNumber;
    private int lastTileClickFrame = -1;
    private LevelDefinition currentLevel;
    private ChampionTheme enemyChampion;
    private WorldTheme currentTheme;

    private IEnumerator Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        LoadLevelDefinition();
        ConfigureWorldTheme();
        CreateBoard();
        ConfigureCameraForBoard();

// GameManager'ın karakterleri oluşturmasını bekle.
for (int i = 0; i < 60; i++)
{
    FindCharacters();

    if (humanPlayer != null && enemyPlayer != null)
    {
        break;   
    }

    yield return null;
}

turnManager = FindAnyObjectByType<TurnManager>();

        if (humanPlayer == null)
        {
            Debug.LogError(
                "İnsan oyuncu bulunamadı. " +
                "Player prefabındaki Is Human Player kutusunu kontrol et."
            );

            yield break;
        }
if (uiManager == null)
{
    Debug.LogError(
        "BoardManager içindeki UI Manager alanı boş."
    );

    yield break;
}
        if (enemyPlayer == null)
        {
            Debug.LogError(
                "Rakip bulunamadı. " +
                "Enemy prefabındaki Is Human Player kutusu boş olmalı."
            );

            yield break;
        }

        if (turnManager == null)
        {
            Debug.LogError(
                "TurnManager bulunamadı."
            );

            yield break;
        }

        if (aiController == null)
        {
            Debug.LogError(
                "BoardManager içindeki AI Controller alanı boş."
            );

            yield break;
        }

        ApplyThemeToCharacters();

        PlaceStartingObstacles();

        uiManager.SetMatchInfo(
            aiController.CurrentDifficulty,
            1
        );

        StartHumanMovementPhase();
    }

    private void LoadLevelDefinition()
    {
        GameProgression.GameMode mode =
            (GameProgression.GameMode)PlayerPrefs.GetInt(
                GameProgression.GameModeKey,
                (int)GameProgression.GameMode.Standard
            );

        if (mode != GameProgression.GameMode.Levels)
        {
            return;
        }

        int levelNumber = PlayerPrefs.GetInt(
            GameProgression.SelectedLevelKey,
            1
        );

        currentLevel = LevelCatalog.GetLevel(levelNumber);
        boardSize = currentLevel.BoardSize;
    }

    private void PlaceStartingObstacles()
    {
        if (currentLevel == null)
        {
            return;
        }

        foreach (BoardPosition position in
                 currentLevel.StartingObstacles)
        {
            if (BoardRules.IsInsideBoard(
                    boardSize,
                    position.X,
                    position.Z
                ))
            {
                PlaceObstacle(
                    tiles[position.X, position.Z],
                    enemyChampion ?? ChampionCatalog.Get(ChampionId.Classic),
                    false
                );
            }
        }
    }

    private void Update()
    {
        if (uiManager == null || uiManager.IsBoardInputBlocked)
        {
            return;
        }

        Vector2 pointerPosition;

        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 0)
            {
                return;
            }

            Touch touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Began)
            {
                return;
            }

            pointerPosition = touch.position;
        }
        else
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            pointerPosition = Input.mousePosition;
        }

        Camera boardCamera = Camera.main;

        if (boardCamera == null)
        {
            return;
        }

        Tile assistedTile = FindNearestSelectableTile(
            boardCamera,
            pointerPosition
        );

        if (assistedTile != null)
        {
            OnTileClicked(assistedTile);
            return;
        }

        Ray ray = boardCamera.ScreenPointToRay(pointerPosition);
        RaycastHit[] hits = Physics.RaycastAll(
            ray,
            Mathf.Infinity,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore
        );

        Tile selectedTile = null;
        float nearestTileDistance = float.MaxValue;

        foreach (RaycastHit hit in hits)
        {
            Tile touchedTile = hit.collider.GetComponentInParent<Tile>();

            if (touchedTile != null && hit.distance < nearestTileDistance)
            {
                selectedTile = touchedTile;
                nearestTileDistance = hit.distance;
            }
        }

        if (selectedTile != null)
        {
            OnTileClicked(selectedTile);
        }
    }

    private Tile FindNearestSelectableTile(
        Camera boardCamera,
        Vector2 pointerPosition
    )
    {
        if (tiles == null || gameEnded)
        {
            return null;
        }

        float touchRadius = Mathf.Clamp(
            Screen.width * 0.085f,
            58f,
            115f
        );
        float bestDistanceSquared = touchRadius * touchRadius;
        Tile bestTile = null;

        for (int x = 0; x < boardSize; x++)
        {
            for (int z = 0; z < boardSize; z++)
            {
                Tile tile = tiles[x, z];
                if (tile == null ||
                    (!tile.IsMovementTarget && !tile.IsObstacleTarget))
                {
                    continue;
                }

                Vector3 screenPoint = boardCamera.WorldToScreenPoint(
                    tile.transform.position + Vector3.up * 0.12f
                );
                if (screenPoint.z <= 0f)
                {
                    continue;
                }

                Vector2 difference =
                    (Vector2)screenPoint - pointerPosition;
                float distanceSquared = difference.sqrMagnitude;

                if (distanceSquared < bestDistanceSquared)
                {
                    bestDistanceSquared = distanceSquared;
                    bestTile = tile;
                }
            }
        }

        return bestTile;
    }

    private void CreateBoard()
    {
        tiles = new Tile[boardSize, boardSize];

        for (int x = 0; x < boardSize; x++)
        {
            for (int z = 0; z < boardSize; z++)
            {
                Vector3 tilePosition = new Vector3(
                    x * tileSpacing,
                    0f,
                    z * tileSpacing
                );

                GameObject tileObject = Instantiate(
                    tilePrefab,
                    tilePosition,
                    Quaternion.identity,
                    transform
                );

                Tile tile =
                    tileObject.GetComponent<Tile>();

                if (tile == null)
                {
                    Debug.LogError(
                        "Tile prefabında Tile scripti yok."
                    );

                    continue;
                }

                tile.Initialize(this, x, z);
                tile.SetTheme(
                    (x + z) % 2 == 0
                        ? currentTheme.TileA
                        : currentTheme.TileB,
                    currentTheme.BlockedTile
                );
                tiles[x, z] = tile;
            }
        }
    }

    private void ConfigureWorldTheme()
    {
        currentTheme = WorldThemeCatalog.GetForCurrentGame();
        CosmeticItem boardCosmetic =
            CosmeticProgress.GetEquipped(CosmeticCategory.Board);
        CosmeticItem obstacleCosmetic =
            CosmeticProgress.GetEquipped(CosmeticCategory.Obstacle);

        if (boardCosmetic.HasColorOverride)
        {
            Color boardColor = boardCosmetic.Color;
            currentTheme = new WorldTheme(
                currentTheme.Number,
                currentTheme.Name,
                Color.Lerp(boardColor, Color.white, 0.16f),
                Color.Lerp(boardColor, Color.black, 0.13f),
                currentTheme.BlockedTile,
                currentTheme.Obstacle,
                currentTheme.Human,
                currentTheme.Enemy,
                currentTheme.Background,
                currentTheme.Metallic,
                currentTheme.Smoothness
            );
        }

        if (obstacleCosmetic.HasColorOverride)
        {
            currentTheme = new WorldTheme(
                currentTheme.Number,
                currentTheme.Name,
                currentTheme.TileA,
                currentTheme.TileB,
                currentTheme.BlockedTile,
                obstacleCosmetic.Color,
                currentTheme.Human,
                currentTheme.Enemy,
                currentTheme.Background,
                Mathf.Max(currentTheme.Metallic, 0.45f),
                Mathf.Max(currentTheme.Smoothness, 0.7f)
            );
        }
        Camera boardCamera = Camera.main;
        if (boardCamera != null)
        {
            boardCamera.clearFlags = CameraClearFlags.SolidColor;
            boardCamera.backgroundColor = currentTheme.Background;
        }

        RenderSettings.ambientLight = Color.Lerp(
            currentTheme.Background,
            Color.white,
            0.55f
        );
    }

    private void ApplyThemeToCharacters()
    {
        ChampionTheme selectedChampion = ChampionProgress.Selected;
        enemyChampion = ChooseEnemyChampion(selectedChampion);
        ApplyCharacterTheme(humanPlayer, selectedChampion.PrimaryColor, false);
        ApplyCharacterTheme(enemyPlayer, currentTheme.Enemy, true);
        ChampionVisualBuilder.BuildCharacter(
            humanPlayer != null ? humanPlayer.gameObject : null,
            selectedChampion,
            false
        );
        ChampionVisualBuilder.BuildCharacter(
            enemyPlayer != null ? enemyPlayer.gameObject : null,
            enemyChampion,
            true
        );
        ApplyMovementEffect(humanPlayer);
    }

    private ChampionTheme ChooseEnemyChampion(ChampionTheme playerChampion)
    {
        ChampionId[] availableEnemies =
        {
            ChampionId.Classic,
            ChampionId.Ninja,
            ChampionId.Pirate,
            ChampionId.Astronaut,
            ChampionId.Robot,
            ChampionId.Wizard,
            ChampionId.Dinosaur,
            ChampionId.Bear
        };

        int index = Random.Range(0, availableEnemies.Length);

        if (availableEnemies.Length > 1 &&
            availableEnemies[index] == playerChampion.Id)
        {
            index = (index + 1) % availableEnemies.Length;
        }

        return ChampionCatalog.Get(availableEnemies[index]);
    }

    private static void ApplyMovementEffect(GridMovement character)
    {
        if (character == null)
        {
            return;
        }

        CosmeticItem effect =
            CosmeticProgress.GetEquipped(CosmeticCategory.Effect);
        TrailRenderer existing = character.GetComponent<TrailRenderer>();

        if (!effect.HasColorOverride)
        {
            if (existing != null)
            {
                existing.enabled = false;
            }
            return;
        }

        TrailRenderer trail = existing != null
            ? existing
            : character.gameObject.AddComponent<TrailRenderer>();
        trail.enabled = true;
        trail.time = 0.42f;
        trail.startWidth = 0.28f;
        trail.endWidth = 0.02f;
        trail.minVertexDistance = 0.04f;
        trail.startColor = new Color(
            effect.Color.r,
            effect.Color.g,
            effect.Color.b,
            0.9f
        );
        trail.endColor = new Color(
            effect.Color.r,
            effect.Color.g,
            effect.Color.b,
            0f
        );

        Shader shader = Shader.Find("Sprites/Default");
        if (shader != null)
        {
            trail.material = new Material(shader);
        }
    }

    private void ApplyCharacterTheme(
        GridMovement character,
        Color color,
        bool enemy
    )
    {
        if (character == null)
        {
            return;
        }

        WorldThemeCatalog.ApplyToRenderers(
            character.gameObject,
            color,
            currentTheme.Metallic,
            currentTheme.Smoothness
        );

        float widthMultiplier =
            0.82f + (currentTheme.Number - 1) * 0.015f;
        float heightMultiplier = enemy ? 1.04f : 1f;
        character.transform.localScale = new Vector3(
            widthMultiplier,
            heightMultiplier * 0.94f,
            widthMultiplier
        );
    }

    private void ConfigureCameraForBoard()
    {
        Camera boardCamera = Camera.main;

        if (boardCamera == null || boardCamera.orthographic)
        {
            return;
        }

        float boardWorldSize = (boardSize - 1) * tileSpacing;
        Vector3 boardCenter = new Vector3(
            boardWorldSize * 0.5f,
            0f,
            boardWorldSize * 0.5f
        );

        float verticalFieldOfView =
            boardCamera.fieldOfView * Mathf.Deg2Rad;
        float safeAspect = Mathf.Max(boardCamera.aspect, 0.4f);
        float horizontalFieldOfView = 2f * Mathf.Atan(
            Mathf.Tan(verticalFieldOfView * 0.5f) * safeAspect
        );

        float halfBoardWithMargin = boardWorldSize * 0.62f;
        float horizontalDistance =
            halfBoardWithMargin /
            Mathf.Tan(horizontalFieldOfView * 0.5f);
        float verticalDistance =
            halfBoardWithMargin /
            Mathf.Tan(verticalFieldOfView * 0.5f);
        float cameraDistance = Mathf.Max(
            horizontalDistance,
            verticalDistance
        );

        // Üst durum paneli ve alt çıkış düğmesi için tahtanın çevresinde
        // güvenli alan bırak. Yatay Game görünümünde daha fazla pay gerekir.
        float framingMultiplier = boardCamera.aspect > 1f
            ? 1.38f
            : Mathf.Lerp(1.18f, 1.28f, Mathf.InverseLerp(
                0.75f,
                0.45f,
                boardCamera.aspect
            ));
        cameraDistance *= framingMultiplier;

        // Engeller küçültüldüğü için rahat ama çok dik olmayan bir açı yeterli.
        Vector3 viewingDirection =
            new Vector3(0f, 1.55f, -0.62f).normalized;

        boardCamera.transform.position =
            boardCenter + viewingDirection * cameraDistance;
        boardCamera.transform.LookAt(boardCenter);
    }

    private void FindCharacters()
    {
        GridMovement[] characters =
            FindObjectsByType<GridMovement>(
                FindObjectsInactive.Exclude
            );

        foreach (GridMovement character in characters)
        {
            if (character.IsHumanPlayer)
            {
                humanPlayer = character;
            }
            else
            {
                enemyPlayer = character;
            }
        }
    }

    public void OnTileClicked(Tile clickedTile)
    {
        if (lastTileClickFrame == Time.frameCount)
        {
            return;
        }

        lastTileClickFrame = Time.frameCount;

        if (gameEnded)
        {
            return;
        }

        if (humanPlayer == null ||
            enemyPlayer == null ||
            turnManager == null)
        {
            return;
        }

        if (turnManager.CurrentTurnOwner !=
            TurnManager.TurnOwner.Human)
        {
            Debug.Log(
                "Şu anda rakibin sırası."
            );

            return;
        }

        if (turnManager.CurrentTurnPhase ==
            TurnManager.TurnPhase.Movement)
        {
            TryMoveHumanPlayer(clickedTile);
        }
        else
        {
            TryPlaceHumanObstacle(clickedTile);
        }
    }

    private void TryMoveHumanPlayer(
        Tile clickedTile
    )
    {
        if (!clickedTile.IsMovementTarget)
        {
            Debug.Log(
                "Bu kareye hareket edilemez."
            );

            return;
        }

        StartCoroutine(
            MoveHumanAndStartObstaclePhase(
                clickedTile
            )
        );
    }

    private IEnumerator MoveHumanAndStartObstaclePhase(
        Tile clickedTile
    )
    {
        ClearAllHighlights();

        humanPlayer.MoveTo(
            clickedTile.X,
            clickedTile.Z
        );

        while (humanPlayer.IsMoving)
        {
            yield return null;
        }

        StartHumanObstaclePhase();
    }

    private void TryPlaceHumanObstacle(
        Tile clickedTile
    )
    {
        if (!clickedTile.IsObstacleTarget)
        {
            Debug.Log(
                "Bu kareye engel yerleştirilemez."
            );

            return;
        }

        PlaceObstacle(clickedTile, ChampionProgress.Selected);
        DailyMissionProgress.RecordPlacedObstacle();

        StartEnemyTurn();
    }

    private void StartHumanMovementPhase()
    {
        if (gameEnded)
        {
            return;
        }

        roundNumber++;
        uiManager.SetMatchInfo(
            aiController.CurrentDifficulty,
            roundNumber
        );

        turnManager.StartHumanTurn();
        uiManager.ShowHumanMovement();
        ClearAllHighlights();

        List<Tile> validMovementTiles =
            GetValidMovementTiles(humanPlayer);

        if (validMovementTiles.Count == 0)
        {
            EnemyWins();
            return;
        }

        foreach (Tile tile in validMovementTiles)
        {
            tile.SetMovementTarget(true);
        }

        Debug.Log(
            "Senin sıran: Yeşil bir kare seç."
        );
    }

    private void StartHumanObstaclePhase()
    {
        turnManager.StartHumanObstaclePhase();
        uiManager.ShowHumanObstacle();
        ClearAllHighlights();

        List<Tile> validObstacleTiles =
            GetValidObstacleTiles();

        foreach (Tile tile in validObstacleTiles)
        {
            tile.SetObstacleTarget(true);
        }

        Debug.Log(
            "Kırmızı bir kareye engel koy."
        );
    }

    private void StartEnemyTurn()
    {
        turnManager.StartEnemyTurn();
        uiManager.ShowEnemyTurn();
        ClearAllHighlights();

        Debug.Log(
            "Rakip düşünüyor..."
        );

        StartCoroutine(
            aiController.PlayTurn(
                this,
                enemyPlayer
            )
        );
    }

    public void BeginEnemyObstaclePhase()
    {
        if (gameEnded)
        {
            return;
        }

        turnManager.StartEnemyObstaclePhase();

        Debug.Log(
            "Rakip engel yerleştiriyor..."
        );
    }

    public void PlaceEnemyObstacle(Tile tile)
    {
        if (gameEnded)
        {
            return;
        }

        PlaceObstacle(
            tile,
            enemyChampion ?? ChampionCatalog.Get(ChampionId.Classic)
        );
    }

    public void CompleteEnemyTurn()
    {
        if (gameEnded)
        {
            return;
        }

        StartHumanMovementPhase();
    }

    public List<Tile> GetValidMovementTiles(
        GridMovement character
    )
    {
        List<Tile> validTiles =
            new List<Tile>();

        List<BoardPosition> validPositions =
            BoardRules.GetValidMovementPositions(
                boardSize,
                new BoardPosition(
                    character.CurrentX,
                    character.CurrentZ
                ),
                IsTileBlocked,
                IsTileOccupied
            );

        foreach (BoardPosition position in validPositions)
        {
            validTiles.Add(
                tiles[position.X, position.Z]
            );
        }

        return validTiles;
    }

    public List<Tile> GetValidObstacleTiles()
    {
        List<Tile> validTiles =
            new List<Tile>();

        List<BoardPosition> validPositions =
            BoardRules.GetValidObstaclePositions(
                boardSize,
                IsTileBlocked,
                IsTileOccupied
            );

        foreach (BoardPosition position in validPositions)
        {
            validTiles.Add(
                tiles[position.X, position.Z]
            );
        }

        return validTiles;
    }

    public int CountEnemyMovesAfterMovingTo(Tile targetTile)
    {
        if (targetTile == null)
        {
            return 0;
        }

        return BoardRules.GetValidMovementPositions(
            boardSize,
            new BoardPosition(targetTile.X, targetTile.Z),
            IsTileBlocked,
            (x, z) =>
                humanPlayer != null &&
                humanPlayer.CurrentX == x &&
                humanPlayer.CurrentZ == z
        ).Count;
    }

    public int CountHumanMovesAfterBlocking(Tile obstacleTile)
    {
        if (obstacleTile == null || humanPlayer == null)
        {
            return 0;
        }

        return BoardRules.GetValidMovementPositions(
            boardSize,
            new BoardPosition(
                humanPlayer.CurrentX,
                humanPlayer.CurrentZ
            ),
            (x, z) =>
                IsTileBlocked(x, z) ||
                (x == obstacleTile.X && z == obstacleTile.Z),
            IsTileOccupied
        ).Count;
    }

    public int CountEnemyMovesAfterBlocking(Tile obstacleTile)
    {
        return CountMovesAfterBlocking(enemyPlayer, obstacleTile);
    }

    public int CountEnemyReachableAreaAfterMovingTo(Tile targetTile)
    {
        return CountReachableAreaAfterEnemyMovesTo(targetTile, true);
    }

    public int CountHumanReachableAreaAfterEnemyMovesTo(Tile targetTile)
    {
        return CountReachableAreaAfterEnemyMovesTo(targetTile, false);
    }

    public int CountEnemyReachableAreaAfterBlocking(Tile obstacleTile)
    {
        return CountReachableAreaAfterBlocking(enemyPlayer, obstacleTile);
    }

    public int CountHumanReachableAreaAfterBlocking(Tile obstacleTile)
    {
        return CountReachableAreaAfterBlocking(humanPlayer, obstacleTile);
    }

    public int ScoreImpossibleEnemyMove(Tile targetTile)
    {
        if (targetTile == null || humanPlayer == null)
        {
            return int.MinValue;
        }

        return BoardStrategy.ScoreAfterBestEnemyObstacle(
            boardSize,
            GetBlockedSnapshot(),
            new BoardPosition(humanPlayer.CurrentX, humanPlayer.CurrentZ),
            new BoardPosition(targetTile.X, targetTile.Z)
        );
    }

    public int ScoreImpossibleEnemyObstacle(Tile obstacleTile)
    {
        if (obstacleTile == null ||
            humanPlayer == null ||
            enemyPlayer == null)
        {
            return int.MinValue;
        }

        return BoardStrategy.ScoreEnemyObstacle(
            boardSize,
            GetBlockedSnapshot(),
            new BoardPosition(humanPlayer.CurrentX, humanPlayer.CurrentZ),
            new BoardPosition(enemyPlayer.CurrentX, enemyPlayer.CurrentZ),
            new BoardPosition(obstacleTile.X, obstacleTile.Z)
        );
    }

    private bool[,] GetBlockedSnapshot()
    {
        bool[,] blocked = new bool[boardSize, boardSize];

        for (int x = 0; x < boardSize; x++)
        {
            for (int z = 0; z < boardSize; z++)
            {
                blocked[x, z] = IsTileBlocked(x, z);
            }
        }

        return blocked;
    }

    private int CountMovesAfterBlocking(
        GridMovement character,
        Tile obstacleTile
    )
    {
        if (character == null || obstacleTile == null)
        {
            return 0;
        }

        return BoardRules.GetValidMovementPositions(
            boardSize,
            new BoardPosition(character.CurrentX, character.CurrentZ),
            (x, z) =>
                IsTileBlocked(x, z) ||
                (x == obstacleTile.X && z == obstacleTile.Z),
            IsTileOccupied
        ).Count;
    }

    private int CountReachableAreaAfterBlocking(
        GridMovement character,
        Tile obstacleTile
    )
    {
        if (character == null || obstacleTile == null)
        {
            return 0;
        }

        return BoardRules.CountReachablePositions(
            boardSize,
            new BoardPosition(character.CurrentX, character.CurrentZ),
            (x, z) =>
                IsTileBlocked(x, z) ||
                (x == obstacleTile.X && z == obstacleTile.Z),
            IsTileOccupied
        );
    }

    private int CountReachableAreaAfterEnemyMovesTo(
        Tile targetTile,
        bool countEnemyArea
    )
    {
        if (targetTile == null ||
            humanPlayer == null ||
            enemyPlayer == null)
        {
            return 0;
        }

        BoardPosition start = countEnemyArea
            ? new BoardPosition(targetTile.X, targetTile.Z)
            : new BoardPosition(humanPlayer.CurrentX, humanPlayer.CurrentZ);

        return BoardRules.CountReachablePositions(
            boardSize,
            start,
            IsTileBlocked,
            (x, z) => countEnemyArea
                ? humanPlayer.CurrentX == x && humanPlayer.CurrentZ == z
                : targetTile.X == x && targetTile.Z == z
        );
    }

    private bool IsTileBlocked(int x, int z)
    {
        Tile tile = tiles[x, z];

        return tile == null || tile.IsBlocked;
    }

    private void PlaceObstacle(
        Tile tile,
        ChampionTheme champion = null,
        bool playSound = true
    )
    {
        if (tile == null || tile.IsBlocked)
        {
            return;
        }

        if (IsTileOccupied(tile.X, tile.Z))
        {
            return;
        }

        if (obstaclePrefab == null)
        {
            Debug.LogError(
                "Obstacle Prefab alanı boş."
            );

            return;
        }

        Vector3 obstaclePosition =
            new Vector3(
                tile.X * tileSpacing,
                obstacleHeight,
                tile.Z * tileSpacing
            );

        GameObject obstacleObject = Instantiate(
            obstaclePrefab,
            obstaclePosition,
            Quaternion.identity
        );

        float rotation = (currentTheme.Number - 1) * 11.25f;
        obstacleObject.transform.Rotate(0f, rotation, 0f);
        float width = 0.72f - (currentTheme.Number - 1) * 0.015f;
        obstacleObject.transform.localScale = new Vector3(
            width,
            0.82f + (currentTheme.Number - 1) * 0.035f,
            width
        );
        WorldThemeCatalog.ApplyToRenderers(
            obstacleObject,
            currentTheme.Obstacle,
            currentTheme.Metallic,
            currentTheme.Smoothness
        );
        ChampionVisualBuilder.BuildObstacle(
            obstacleObject,
            champion ?? ChampionCatalog.Get(ChampionId.Classic)
        );

        tile.SetBlocked(true);
        if (playSound)
        {
            GameAudio.PlayObstacle();
        }

        Debug.Log(
            $"Engel yerleştirildi: " +
            $"X={tile.X}, Z={tile.Z}"
        );
    }

    private bool IsTileOccupied(int x, int z)
    {
        bool humanIsHere =
            humanPlayer != null &&
            humanPlayer.CurrentX == x &&
            humanPlayer.CurrentZ == z;

        bool enemyIsHere =
            enemyPlayer != null &&
            enemyPlayer.CurrentX == x &&
            enemyPlayer.CurrentZ == z;

        return humanIsHere || enemyIsHere;
    }

    private void ClearAllHighlights()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int z = 0;
                 z < boardSize;
                 z++)
            {
                if (tiles[x, z] != null)
                {
                    tiles[x, z]
                        .ClearHighlights();
                }
            }
        }
    }

    public void HumanWins()
{
    GameProgression.CompleteCurrentLevel();
    GameAudio.PlayWin();
    uiManager.ShowHumanWin();

    EndGame(
        "Kazandın! Rakip hareket edemiyor."
    );
}

    public void EnemyWins()
{
    EconomyProgress.RecordCurrentLevelDefeat();
    GameAudio.PlayLose();
    uiManager.ShowEnemyWin();

    EndGame(
        "Kaybettin! Hareket edebileceğin kare kalmadı."
    );
}

    private void EndGame(string message)
    {
        gameEnded = true;

        ClearAllHighlights();

        Debug.Log(message);
    }
}
