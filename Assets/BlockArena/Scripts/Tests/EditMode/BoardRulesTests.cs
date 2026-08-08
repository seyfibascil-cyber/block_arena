using System.Collections.Generic;
using NUnit.Framework;

public class BoardRulesTests
{
    private const int BoardSize = 7;

    [Test]
    public void MovementFromCenter_ReturnsEightNeighbours()
    {
        List<BoardPosition> positions =
            GetMovementPositions(
                new BoardPosition(3, 3)
            );

        Assert.That(positions, Has.Count.EqualTo(8));
        Assert.That(
            positions,
            Does.Contain(new BoardPosition(2, 2))
        );
        Assert.That(
            positions,
            Does.Contain(new BoardPosition(4, 4))
        );
    }

    [Test]
    public void MovementFromCorner_StaysInsideBoard()
    {
        List<BoardPosition> positions =
            GetMovementPositions(
                new BoardPosition(0, 0)
            );

        Assert.That(positions, Has.Count.EqualTo(3));
        Assert.That(
            positions,
            Is.All.Matches<BoardPosition>(
                position =>
                    BoardRules.IsInsideBoard(
                        BoardSize,
                        position.X,
                        position.Z
                    )
            )
        );
    }

    [Test]
    public void Movement_ExcludesBlockedTile()
    {
        BoardPosition blockedPosition =
            new BoardPosition(3, 4);

        List<BoardPosition> positions =
            BoardRules.GetValidMovementPositions(
                BoardSize,
                new BoardPosition(3, 3),
                (x, z) =>
                    x == blockedPosition.X &&
                    z == blockedPosition.Z,
                (_, _) => false
            );

        Assert.That(
            positions.Contains(blockedPosition),
            Is.False
        );
        Assert.That(positions, Has.Count.EqualTo(7));
    }

    [Test]
    public void Movement_ExcludesOccupiedTile()
    {
        BoardPosition occupiedPosition =
            new BoardPosition(4, 3);

        List<BoardPosition> positions =
            BoardRules.GetValidMovementPositions(
                BoardSize,
                new BoardPosition(3, 3),
                (_, _) => false,
                (x, z) =>
                    x == occupiedPosition.X &&
                    z == occupiedPosition.Z
            );

        Assert.That(
            positions.Contains(occupiedPosition),
            Is.False
        );
        Assert.That(positions, Has.Count.EqualTo(7));
    }

    [Test]
    public void Movement_WhenSurrounded_ReturnsNoPositions()
    {
        List<BoardPosition> positions =
            BoardRules.GetValidMovementPositions(
                BoardSize,
                new BoardPosition(3, 3),
                (_, _) => true,
                (_, _) => false
            );

        Assert.That(positions, Is.Empty);
    }

    [Test]
    public void Obstacles_ExcludeBlockedAndOccupiedTiles()
    {
        BoardPosition blockedPosition =
            new BoardPosition(1, 1);

        BoardPosition occupiedPosition =
            new BoardPosition(5, 5);

        List<BoardPosition> positions =
            BoardRules.GetValidObstaclePositions(
                BoardSize,
                (x, z) =>
                    x == blockedPosition.X &&
                    z == blockedPosition.Z,
                (x, z) =>
                    x == occupiedPosition.X &&
                    z == occupiedPosition.Z
            );

        Assert.That(
            positions.Contains(blockedPosition),
            Is.False
        );
        Assert.That(
            positions.Contains(occupiedPosition),
            Is.False
        );
        Assert.That(
            positions,
            Has.Count.EqualTo(
                BoardSize * BoardSize - 2
            )
        );
    }

    [Test]
    public void ReachablePositions_WhenOpen_ReturnsEveryOtherTile()
    {
        int count = BoardRules.CountReachablePositions(
            3,
            new BoardPosition(1, 1),
            (_, _) => false,
            (_, _) => false
        );

        Assert.That(count, Is.EqualTo(8));
    }

    [Test]
    public void ReachablePositions_DoesNotCrossSolidWall()
    {
        int count = BoardRules.CountReachablePositions(
            3,
            new BoardPosition(0, 1),
            (x, _) => x == 1,
            (_, _) => false
        );

        Assert.That(count, Is.EqualTo(2));
    }

    private static List<BoardPosition> GetMovementPositions(
        BoardPosition currentPosition
    )
    {
        return BoardRules.GetValidMovementPositions(
            BoardSize,
            currentPosition,
            (_, _) => false,
            (_, _) => false
        );
    }
}
