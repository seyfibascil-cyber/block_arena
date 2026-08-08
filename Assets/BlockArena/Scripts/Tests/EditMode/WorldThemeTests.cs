using NUnit.Framework;

public class WorldThemeTests
{
    [TestCase(1, 1)]
    [TestCase(30, 1)]
    [TestCase(31, 2)]
    [TestCase(60, 2)]
    [TestCase(61, 3)]
    [TestCase(90, 3)]
    [TestCase(91, 4)]
    [TestCase(120, 4)]
    [TestCase(121, 5)]
    [TestCase(150, 5)]
    public void WorldChangesEveryThirtyLevels(int level, int expected)
    {
        Assert.That(
            WorldThemeCatalog.GetWorldNumber(level),
            Is.EqualTo(expected)
        );
    }
}
