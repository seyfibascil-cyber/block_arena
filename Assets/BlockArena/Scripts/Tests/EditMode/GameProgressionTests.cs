using NUnit.Framework;

public class GameProgressionTests
{
    [TestCase(1, AIController.Difficulty.Easy)]
    [TestCase(20, AIController.Difficulty.Easy)]
    [TestCase(21, AIController.Difficulty.Medium)]
    [TestCase(40, AIController.Difficulty.Medium)]
    [TestCase(41, AIController.Difficulty.Hard)]
    [TestCase(60, AIController.Difficulty.Hard)]
    [TestCase(61, AIController.Difficulty.Impossible)]
    [TestCase(150, AIController.Difficulty.Impossible)]
    public void LevelDifficulty_IncreasesAcrossCampaign(
        int level,
        AIController.Difficulty expectedDifficulty
    )
    {
        Assert.That(
            GameProgression.GetDifficultyForLevel(level),
            Is.EqualTo(expectedDifficulty)
        );
    }
}
