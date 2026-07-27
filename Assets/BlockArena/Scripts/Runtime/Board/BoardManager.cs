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

    private IEnumerator Start()
    {
        CreateBoard();

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

        StartHumanMovementPhase();
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
                tiles[x, z] = tile;
            }
        }
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

        PlaceObstacle(clickedTile);

        StartEnemyTurn();
    }

    private void StartHumanMovementPhase()
    {
        if (gameEnded)
        {
            return;
        }

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

        PlaceObstacle(tile);
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

        int characterX = character.CurrentX;
        int characterZ = character.CurrentZ;

        for (int xDirection = -1;
             xDirection <= 1;
             xDirection++)
        {
            for (int zDirection = -1;
                 zDirection <= 1;
                 zDirection++)
            {
                if (xDirection == 0 &&
                    zDirection == 0)
                {
                    continue;
                }

                int targetX =
                    characterX + xDirection;

                int targetZ =
                    characterZ + zDirection;

                if (!IsInsideBoard(
                        targetX,
                        targetZ))
                {
                    continue;
                }

                Tile targetTile =
                    tiles[targetX, targetZ];

                if (targetTile == null)
                {
                    continue;
                }

                if (targetTile.IsBlocked)
                {
                    continue;
                }

                if (IsTileOccupied(
                        targetX,
                        targetZ))
                {
                    continue;
                }

                validTiles.Add(targetTile);
            }
        }

        return validTiles;
    }

    public List<Tile> GetValidObstacleTiles()
    {
        List<Tile> validTiles =
            new List<Tile>();

        for (int x = 0; x < boardSize; x++)
        {
            for (int z = 0;
                 z < boardSize;
                 z++)
            {
                Tile tile = tiles[x, z];

                if (tile == null)
                {
                    continue;
                }

                if (tile.IsBlocked)
                {
                    continue;
                }

                if (IsTileOccupied(x, z))
                {
                    continue;
                }

                validTiles.Add(tile);
            }
        }

        return validTiles;
    }

    private void PlaceObstacle(Tile tile)
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

        Instantiate(
            obstaclePrefab,
            obstaclePosition,
            Quaternion.identity
        );

        tile.SetBlocked(true);

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

    private bool IsInsideBoard(int x, int z)
    {
        return x >= 0 &&
               x < boardSize &&
               z >= 0 &&
               z < boardSize;
    }

    public void HumanWins()
{
    uiManager.ShowHumanWin();

    EndGame(
        "Kazandın! Rakip hareket edemiyor."
    );
}

    public void EnemyWins()
{
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
