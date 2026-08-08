using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private const string DifficultyKey = "BlockArena.Difficulty";

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible
    }

    [SerializeField] private Difficulty difficulty = Difficulty.Medium;
    [Header("Yapay Zekâ Ayarları")]
    [SerializeField] private float thinkingDelay = 0.6f;

    public Difficulty CurrentDifficulty => difficulty;

    private void Awake()
    {
        int savedDifficulty = PlayerPrefs.GetInt(
            DifficultyKey,
            (int)difficulty
        );

        difficulty = (Difficulty)Mathf.Clamp(
            savedDifficulty,
            (int)Difficulty.Easy,
            (int)Difficulty.Impossible
        );
    }

    public IEnumerator PlayTurn(
        BoardManager boardManager,
        GridMovement enemyPlayer
    )
    {
        // Rakip düşünüyormuş gibi kısa süre bekler.
        yield return new WaitForSeconds(thinkingDelay);

        List<Tile> validMovementTiles =
            boardManager.GetValidMovementTiles(enemyPlayer);

        // Rakibin hareket edebileceği kare yoksa insan kazanır.
        if (validMovementTiles.Count == 0)
        {
            boardManager.HumanWins();
            yield break;
        }

        Tile selectedMovementTile =
            SelectMovementTile(boardManager, validMovementTiles);

        enemyPlayer.MoveTo(
    selectedMovementTile.X,
    selectedMovementTile.Z
);

while (enemyPlayer.IsMoving)
{
    yield return null;
}

        boardManager.BeginEnemyObstaclePhase();

        yield return new WaitForSeconds(thinkingDelay);

        List<Tile> validObstacleTiles =
            boardManager.GetValidObstacleTiles();

        if (validObstacleTiles.Count > 0)
        {
            Tile selectedObstacleTile =
                SelectObstacleTile(boardManager, validObstacleTiles);

            boardManager.PlaceEnemyObstacle(
                selectedObstacleTile
            );
        }

        yield return new WaitForSeconds(thinkingDelay);

        boardManager.CompleteEnemyTurn();
    }

    private Tile SelectMovementTile(
        BoardManager boardManager,
        List<Tile> options
    )
    {
        if (difficulty == Difficulty.Easy)
        {
            return options[Random.Range(0, options.Count)];
        }

        if (difficulty == Difficulty.Medium)
        {
            return AIPlanner.SelectHighestScoring(
                options,
                boardManager.CountEnemyMovesAfterMovingTo
            );
        }

        int humanWeight =
            difficulty == Difficulty.Impossible ? 3 : 1;

        if (difficulty == Difficulty.Impossible)
        {
            return AIPlanner.SelectHighestScoring(
                options,
                boardManager.ScoreImpossibleEnemyMove
            );
        }

        return AIPlanner.SelectHighestScoring(
            options,
            tile =>
                boardManager.CountEnemyReachableAreaAfterMovingTo(tile) * 2 -
                boardManager.CountHumanReachableAreaAfterEnemyMovesTo(tile) *
                humanWeight
        );
    }

    private Tile SelectObstacleTile(
        BoardManager boardManager,
        List<Tile> options
    )
    {
        if (difficulty == Difficulty.Easy)
        {
            return options[Random.Range(0, options.Count)];
        }

        if (difficulty == Difficulty.Medium)
        {
            return AIPlanner.SelectLowestScoring(
                options,
                boardManager.CountHumanMovesAfterBlocking
            );
        }

        if (difficulty == Difficulty.Hard)
        {
            return AIPlanner.SelectHighestScoring(
                options,
                tile =>
                    boardManager.CountEnemyMovesAfterBlocking(tile) * 2 -
                    boardManager.CountHumanMovesAfterBlocking(tile) * 3
            );
        }

        return AIPlanner.SelectHighestScoring(
            options,
            boardManager.ScoreImpossibleEnemyObstacle
        );
    }
}
