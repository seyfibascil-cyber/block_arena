using System;
using System.Collections.Generic;

public static class LevelCatalog
{
    public static LevelDefinition GetLevel(int levelNumber)
    {
        int safeLevel = Math.Max(
            1,
            Math.Min(levelNumber, GameProgression.MaximumLevel)
        );

        int boardSize = safeLevel <= 3
            ? 5
            : safeLevel <= 10 ? 6 : 7;

        return new LevelDefinition(
            safeLevel,
            boardSize,
            new BoardPosition(boardSize / 2, 0),
            new BoardPosition(boardSize / 2, boardSize - 1),
            GameProgression.GetDifficultyForLevel(safeLevel),
            GetStartingObstacles(safeLevel)
        );
    }

    private static IReadOnlyList<BoardPosition> GetStartingObstacles(
        int levelNumber
    )
    {
        switch (levelNumber)
        {
            case 1:
                return Positions();
            case 2:
                return Positions((1, 2));
            case 3:
                return Positions((1, 2), (3, 2));
            case 4:
                return Positions((1, 2), (4, 3));
            case 5:
                return Positions((1, 1), (4, 1), (2, 3));
            case 6:
                return Positions((1, 2), (4, 2), (2, 4), (3, 4));
            case 7:
                return Positions((0, 2), (5, 2), (2, 3), (3, 3));
            case 8:
                return Positions((1, 1), (4, 1), (1, 4), (4, 4));
            case 9:
                return Positions((1, 2), (2, 2), (3, 3), (4, 3));
            case 10:
                return Positions(
                    (0, 2),
                    (2, 1),
                    (3, 4),
                    (5, 3),
                    (1, 4)
                );
            default:
                return GenerateStartingObstacles(levelNumber);
        }
    }

    private static IReadOnlyList<BoardPosition> GenerateStartingObstacles(
        int levelNumber
    )
    {
        const int boardSize = 7;
        BoardPosition humanStart = new BoardPosition(3, 0);
        BoardPosition enemyStart = new BoardPosition(3, 6);
        BoardPosition humanEscape = new BoardPosition(3, 1);
        BoardPosition enemyEscape = new BoardPosition(3, 5);

        int obstacleCount = Math.Min(
            14,
            3 + (levelNumber - 11) / 9
        );

        List<BoardPosition> candidates =
            new List<BoardPosition>();

        for (int x = 0; x < boardSize; x++)
        {
            for (int z = 0; z < boardSize; z++)
            {
                BoardPosition position = new BoardPosition(x, z);

                if (position.Equals(humanStart) ||
                    position.Equals(enemyStart) ||
                    position.Equals(humanEscape) ||
                    position.Equals(enemyEscape))
                {
                    continue;
                }

                candidates.Add(position);
            }
        }

        Random random = new Random(levelNumber * 7919 + 17);

        for (int index = candidates.Count - 1;
             index > 0;
             index--)
        {
            int swapIndex = random.Next(index + 1);
            BoardPosition temporary = candidates[index];
            candidates[index] = candidates[swapIndex];
            candidates[swapIndex] = temporary;
        }

        return candidates.GetRange(0, obstacleCount);
    }

    private static IReadOnlyList<BoardPosition> Positions(
        params (int x, int z)[] coordinates
    )
    {
        List<BoardPosition> positions =
            new List<BoardPosition>(coordinates.Length);

        foreach ((int x, int z) coordinate in coordinates)
        {
            positions.Add(
                new BoardPosition(coordinate.x, coordinate.z)
            );
        }

        return positions;
    }
}
