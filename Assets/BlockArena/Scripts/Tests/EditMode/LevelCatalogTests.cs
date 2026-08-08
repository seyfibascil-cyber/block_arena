using System.Collections.Generic;
using NUnit.Framework;

public class LevelCatalogTests
{
    [Test]
    public void FirstTenLevels_HaveValidDistinctStartingContent()
    {
        for (int levelNumber = 1;
             levelNumber <= 10;
             levelNumber++)
        {
            LevelDefinition level = LevelCatalog.GetLevel(levelNumber);
            HashSet<BoardPosition> obstacles =
                new HashSet<BoardPosition>();

            Assert.That(level.Number, Is.EqualTo(levelNumber));
            Assert.That(level.BoardSize, Is.InRange(5, 7));
            Assert.That(level.HumanStart, Is.Not.EqualTo(level.EnemyStart));

            foreach (BoardPosition obstacle in level.StartingObstacles)
            {
                Assert.That(
                    BoardRules.IsInsideBoard(
                        level.BoardSize,
                        obstacle.X,
                        obstacle.Z
                    ),
                    Is.True
                );
                Assert.That(obstacle, Is.Not.EqualTo(level.HumanStart));
                Assert.That(obstacle, Is.Not.EqualTo(level.EnemyStart));
                Assert.That(obstacles.Add(obstacle), Is.True);
            }
        }
    }

    [Test]
    public void EarlyLevels_IntroduceObstaclesGradually()
    {
        Assert.That(LevelCatalog.GetLevel(1).StartingObstacles, Is.Empty);
        Assert.That(LevelCatalog.GetLevel(2).StartingObstacles, Has.Count.EqualTo(1));
        Assert.That(LevelCatalog.GetLevel(3).StartingObstacles, Has.Count.EqualTo(2));
    }

    [Test]
    public void AllLevels_HaveSafeUniqueObstacleLayouts()
    {
        for (int levelNumber = 1;
             levelNumber <= GameProgression.MaximumLevel;
             levelNumber++)
        {
            LevelDefinition level = LevelCatalog.GetLevel(levelNumber);
            HashSet<BoardPosition> unique =
                new HashSet<BoardPosition>();

            foreach (BoardPosition obstacle in level.StartingObstacles)
            {
                Assert.That(
                    BoardRules.IsInsideBoard(
                        level.BoardSize,
                        obstacle.X,
                        obstacle.Z
                    ),
                    Is.True,
                    $"Bölüm {levelNumber}: tahta dışı engel {obstacle}"
                );
                Assert.That(obstacle, Is.Not.EqualTo(level.HumanStart));
                Assert.That(obstacle, Is.Not.EqualTo(level.EnemyStart));
                Assert.That(unique.Add(obstacle), Is.True);
            }

            List<BoardPosition> humanMoves =
                BoardRules.GetValidMovementPositions(
                    level.BoardSize,
                    level.HumanStart,
                    (x, z) => unique.Contains(new BoardPosition(x, z)),
                    (x, z) =>
                        level.EnemyStart.X == x &&
                        level.EnemyStart.Z == z
                );
            List<BoardPosition> enemyMoves =
                BoardRules.GetValidMovementPositions(
                    level.BoardSize,
                    level.EnemyStart,
                    (x, z) => unique.Contains(new BoardPosition(x, z)),
                    (x, z) =>
                        level.HumanStart.X == x &&
                        level.HumanStart.Z == z
                );

            Assert.That(humanMoves, Is.Not.Empty);
            Assert.That(enemyMoves, Is.Not.Empty);
        }
    }

    [Test]
    public void LaterLevels_ContainMoreStartingObstacles()
    {
        Assert.That(
            LevelCatalog.GetLevel(150).StartingObstacles.Count,
            Is.GreaterThan(LevelCatalog.GetLevel(11).StartingObstacles.Count)
        );
    }
}
