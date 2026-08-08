using NUnit.Framework;
using UnityEngine;

public class ChampionProgressTests
{
    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();
    }

    [Test]
    public void ClassicChampionIsUnlockedByDefault()
    {
        Assert.That(
            ChampionProgress.IsUnlocked(ChampionCatalog.Get(ChampionId.Classic)),
            Is.True
        );
    }

    [Test]
    public void NinjaUnlocksAtThirtyTotalStars()
    {
        ChampionTheme ninja = ChampionCatalog.Get(ChampionId.Ninja);
        Assert.That(ChampionProgress.IsUnlocked(ninja), Is.False);

        for (int level = 1; level <= 10; level++)
        {
            GameProgression.StartLevel(level);
            EconomyProgress.RewardCurrentLevelCompletion();
        }

        Assert.That(EconomyProgress.TotalStars, Is.EqualTo(30));
        Assert.That(ChampionProgress.IsUnlocked(ninja), Is.True);
    }

    [Test]
    public void PiratePurchaseSpendsCoinsAndAllowsSelection()
    {
        ChampionTheme pirate = ChampionCatalog.Get(ChampionId.Pirate);
        EconomyProgress.GrantCoins(1200);

        Assert.That(ChampionProgress.TryUnlock(pirate), Is.True);
        Assert.That(EconomyProgress.Coins, Is.EqualTo(200));
        Assert.That(ChampionProgress.TrySelect(pirate), Is.True);
        Assert.That(ChampionProgress.Selected.Id, Is.EqualTo(ChampionId.Pirate));
    }

    [Test]
    public void LockedChampionCannotBeSelected()
    {
        ChampionTheme bear = ChampionCatalog.Get(ChampionId.Bear);

        Assert.That(ChampionProgress.TrySelect(bear), Is.False);
        Assert.That(ChampionProgress.Selected.Id, Is.EqualTo(ChampionId.Classic));
    }
}
