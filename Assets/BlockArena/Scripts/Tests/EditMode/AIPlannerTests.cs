using System.Collections.Generic;
using NUnit.Framework;

public class AIPlannerTests
{
    [Test]
    public void HighestScoring_SelectsOptionWithMostFreedom()
    {
        List<int> options = new List<int> { 2, 7, 4 };

        int selected = AIPlanner.SelectHighestScoring(
            options,
            value => value
        );

        Assert.That(selected, Is.EqualTo(7));
    }

    [Test]
    public void LowestScoring_SelectsOptionThatRestrictsOpponentMost()
    {
        List<int> options = new List<int> { 5, 1, 3 };

        int selected = AIPlanner.SelectLowestScoring(
            options,
            value => value
        );

        Assert.That(selected, Is.EqualTo(1));
    }

    [Test]
    public void EqualScores_KeepFirstOptionForDeterministicTurns()
    {
        List<int> options = new List<int> { 10, 20 };

        int selected = AIPlanner.SelectHighestScoring(
            options,
            _ => 3
        );

        Assert.That(selected, Is.EqualTo(10));
    }
}
