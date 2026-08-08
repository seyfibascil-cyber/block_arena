using System.Collections.Generic;

public sealed class LevelDefinition
{
    public LevelDefinition(
        int number,
        int boardSize,
        BoardPosition humanStart,
        BoardPosition enemyStart,
        AIController.Difficulty difficulty,
        IReadOnlyList<BoardPosition> startingObstacles
    )
    {
        Number = number;
        BoardSize = boardSize;
        HumanStart = humanStart;
        EnemyStart = enemyStart;
        Difficulty = difficulty;
        StartingObstacles = startingObstacles;
    }

    public int Number { get; }
    public int BoardSize { get; }
    public BoardPosition HumanStart { get; }
    public BoardPosition EnemyStart { get; }
    public AIController.Difficulty Difficulty { get; }
    public IReadOnlyList<BoardPosition> StartingObstacles { get; }
}
