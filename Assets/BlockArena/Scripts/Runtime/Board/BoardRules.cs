using System;
using System.Collections.Generic;

public static class BoardRules
{
    public static List<BoardPosition> GetValidMovementPositions(
        int boardSize,
        BoardPosition currentPosition,
        Func<int, int, bool> isBlocked,
        Func<int, int, bool> isOccupied
    )
    {
        ValidateArguments(boardSize, isBlocked, isOccupied);

        List<BoardPosition> validPositions =
            new List<BoardPosition>();

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
                    currentPosition.X + xDirection;

                int targetZ =
                    currentPosition.Z + zDirection;

                if (!IsInsideBoard(
                        boardSize,
                        targetX,
                        targetZ))
                {
                    continue;
                }

                if (isBlocked(targetX, targetZ) ||
                    isOccupied(targetX, targetZ))
                {
                    continue;
                }

                validPositions.Add(
                    new BoardPosition(targetX, targetZ)
                );
            }
        }

        return validPositions;
    }

    public static List<BoardPosition> GetValidObstaclePositions(
        int boardSize,
        Func<int, int, bool> isBlocked,
        Func<int, int, bool> isOccupied
    )
    {
        ValidateArguments(boardSize, isBlocked, isOccupied);

        List<BoardPosition> validPositions =
            new List<BoardPosition>();

        for (int x = 0; x < boardSize; x++)
        {
            for (int z = 0; z < boardSize; z++)
            {
                if (isBlocked(x, z) ||
                    isOccupied(x, z))
                {
                    continue;
                }

                validPositions.Add(
                    new BoardPosition(x, z)
                );
            }
        }

        return validPositions;
    }

    public static bool IsInsideBoard(
        int boardSize,
        int x,
        int z
    )
    {
        return x >= 0 &&
               x < boardSize &&
               z >= 0 &&
               z < boardSize;
    }

    public static int CountReachablePositions(
        int boardSize,
        BoardPosition startPosition,
        Func<int, int, bool> isBlocked,
        Func<int, int, bool> isOccupied
    )
    {
        ValidateArguments(boardSize, isBlocked, isOccupied);

        bool[,] visited = new bool[boardSize, boardSize];
        Queue<BoardPosition> pending = new Queue<BoardPosition>();
        pending.Enqueue(startPosition);
        visited[startPosition.X, startPosition.Z] = true;

        int reachableCount = 0;

        while (pending.Count > 0)
        {
            BoardPosition current = pending.Dequeue();

            foreach (BoardPosition next in GetValidMovementPositions(
                         boardSize,
                         current,
                         isBlocked,
                         isOccupied
                     ))
            {
                if (visited[next.X, next.Z])
                {
                    continue;
                }

                visited[next.X, next.Z] = true;
                pending.Enqueue(next);
                reachableCount++;
            }
        }

        return reachableCount;
    }

    private static void ValidateArguments(
        int boardSize,
        Func<int, int, bool> isBlocked,
        Func<int, int, bool> isOccupied
    )
    {
        if (boardSize <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(boardSize),
                "Board size must be greater than zero."
            );
        }

        if (isBlocked == null)
        {
            throw new ArgumentNullException(
                nameof(isBlocked)
            );
        }

        if (isOccupied == null)
        {
            throw new ArgumentNullException(
                nameof(isOccupied)
            );
        }
    }
}
