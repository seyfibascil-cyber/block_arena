using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [Header("Yapay Zekâ Ayarları")]
    [SerializeField] private float thinkingDelay = 0.6f;

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

        int randomMovementIndex =
            Random.Range(0, validMovementTiles.Count);

        Tile selectedMovementTile =
            validMovementTiles[randomMovementIndex];

        enemyPlayer.MoveTo(
    selectedMovementTile.X,
    selectedMovementTile.Z
);

while (enemyPlayer.IsMoving)
{
    yield return null;
}

boardManager.BeginEnemyObstaclePhase();

        boardManager.BeginEnemyObstaclePhase();

        yield return new WaitForSeconds(thinkingDelay);

        List<Tile> validObstacleTiles =
            boardManager.GetValidObstacleTiles();

        if (validObstacleTiles.Count > 0)
        {
            int randomObstacleIndex =
                Random.Range(0, validObstacleTiles.Count);

            Tile selectedObstacleTile =
                validObstacleTiles[randomObstacleIndex];

            boardManager.PlaceEnemyObstacle(
                selectedObstacleTile
            );
        }

        yield return new WaitForSeconds(thinkingDelay);

        boardManager.CompleteEnemyTurn();
    }
}