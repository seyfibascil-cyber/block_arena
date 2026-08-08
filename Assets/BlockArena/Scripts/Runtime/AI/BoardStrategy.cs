using System;
using System.Collections.Generic;

public static class BoardStrategy
{
    private const int WinningScore = 100000;

    public static int ScoreAfterBestEnemyObstacle(
        int boardSize,
        bool[,] blocked,
        BoardPosition human,
        BoardPosition enemy
    )
    {
        int bestScore = int.MinValue;

        for (int x = 0; x < boardSize; x++)
        {
            for (int z = 0; z < boardSize; z++)
            {
                BoardPosition obstacle = new BoardPosition(x, z);

                if (blocked[x, z] ||
                    obstacle.Equals(human) ||
                    obstacle.Equals(enemy))
                {
                    continue;
                }

                int score = ScoreEnemyObstacle(
                    boardSize,
                    blocked,
                    human,
                    enemy,
                    obstacle
                );

                if (score > bestScore)
                {
                    bestScore = score;
                }
            }
        }

        return bestScore == int.MinValue
            ? Evaluate(boardSize, blocked, human, enemy)
            : bestScore;
    }

    public static int ScoreEnemyObstacle(
        int boardSize,
        bool[,] blocked,
        BoardPosition human,
        BoardPosition enemy,
        BoardPosition obstacle
    )
    {
        blocked[obstacle.X, obstacle.Z] = true;

        List<BoardPosition> humanMoves = GetMoves(
            boardSize,
            blocked,
            human,
            enemy
        );

        if (humanMoves.Count == 0)
        {
            blocked[obstacle.X, obstacle.Z] = false;
            return WinningScore;
        }

        int worstResponseScore = int.MaxValue;

        foreach (BoardPosition humanMove in humanMoves)
        {
            int responseScore = Evaluate(
                boardSize,
                blocked,
                humanMove,
                enemy
            );

            if (responseScore < worstResponseScore)
            {
                worstResponseScore = responseScore;
            }
        }

        blocked[obstacle.X, obstacle.Z] = false;
        return worstResponseScore;
    }

    public static int Evaluate(
        int boardSize,
        bool[,] blocked,
        BoardPosition human,
        BoardPosition enemy
    )
    {
        int humanMobility = GetMoves(
            boardSize,
            blocked,
            human,
            enemy
        ).Count;
        int enemyMobility = GetMoves(
            boardSize,
            blocked,
            enemy,
            human
        ).Count;

        if (humanMobility == 0)
        {
            return WinningScore;
        }

        if (enemyMobility == 0)
        {
            return -WinningScore;
        }

        int[,] humanDistances = GetDistances(
            boardSize,
            blocked,
            human,
            enemy
        );
        int[,] enemyDistances = GetDistances(
            boardSize,
            blocked,
            enemy,
            human
        );

        int humanTerritory = 0;
        int enemyTerritory = 0;
        int humanReachable = 0;
        int enemyReachable = 0;

        for (int x = 0; x < boardSize; x++)
        {
            for (int z = 0; z < boardSize; z++)
            {
                if (blocked[x, z])
                {
                    continue;
                }

                int humanDistance = humanDistances[x, z];
                int enemyDistance = enemyDistances[x, z];

                if (humanDistance >= 0)
                {
                    humanReachable++;
                }

                if (enemyDistance >= 0)
                {
                    enemyReachable++;
                }

                if (enemyDistance >= 0 &&
                    (humanDistance < 0 || enemyDistance < humanDistance))
                {
                    enemyTerritory++;
                }
                else if (humanDistance >= 0 &&
                         (enemyDistance < 0 || humanDistance < enemyDistance))
                {
                    humanTerritory++;
                }
            }
        }

        // İmkânsız rakip için doğrudan baskıya öncelik ver: oyuncunun
        // seçeneklerini ve güvenli alanını hızla azalt, kendi kaçış alanını koru.
        return (enemyTerritory - humanTerritory) * 20 +
               (enemyMobility - humanMobility) * 80 +
               (enemyReachable - humanReachable) * 12 -
               humanMobility * 35 -
               humanReachable * 4;
    }

    private static List<BoardPosition> GetMoves(
        int boardSize,
        bool[,] blocked,
        BoardPosition position,
        BoardPosition opponent
    )
    {
        return BoardRules.GetValidMovementPositions(
            boardSize,
            position,
            (x, z) => blocked[x, z],
            (x, z) => opponent.X == x && opponent.Z == z
        );
    }

    private static int[,] GetDistances(
        int boardSize,
        bool[,] blocked,
        BoardPosition start,
        BoardPosition opponent
    )
    {
        int[,] distances = new int[boardSize, boardSize];

        for (int x = 0; x < boardSize; x++)
        {
            for (int z = 0; z < boardSize; z++)
            {
                distances[x, z] = -1;
            }
        }

        Queue<BoardPosition> pending = new Queue<BoardPosition>();
        distances[start.X, start.Z] = 0;
        pending.Enqueue(start);

        while (pending.Count > 0)
        {
            BoardPosition current = pending.Dequeue();

            foreach (BoardPosition next in GetMoves(
                         boardSize,
                         blocked,
                         current,
                         opponent
                     ))
            {
                if (distances[next.X, next.Z] >= 0)
                {
                    continue;
                }

                distances[next.X, next.Z] =
                    distances[current.X, current.Z] + 1;
                pending.Enqueue(next);
            }
        }

        return distances;
    }
}
