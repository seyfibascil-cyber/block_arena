using NUnit.Framework;

public class BoardStrategyTests
{
    [Test]
    public void BestObstacle_WhenTrapExists_FindsForcedWin()
    {
        bool[,] blocked = new bool[3, 3];
        blocked[0, 1] = true;
        blocked[1, 1] = true;

        int score = BoardStrategy.ScoreAfterBestEnemyObstacle(
            3,
            blocked,
            new BoardPosition(0, 0),
            new BoardPosition(2, 2)
        );

        Assert.That(score, Is.GreaterThanOrEqualTo(100000));
    }

    [Test]
    public void ObstacleSimulation_DoesNotModifyOriginalBoard()
    {
        bool[,] blocked = new bool[4, 4];
        BoardPosition obstacle = new BoardPosition(1, 1);

        BoardStrategy.ScoreEnemyObstacle(
            4,
            blocked,
            new BoardPosition(0, 0),
            new BoardPosition(3, 3),
            obstacle
        );

        Assert.That(blocked[obstacle.X, obstacle.Z], Is.False);
    }
}
