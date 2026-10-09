using NUnit.Framework;
using Lightbound;

public class HealthTests
{
    private const float Max = 100f;

    private static Health NewHealth() => new Health(Max);

    [Test]
    public void StartsFull()
    {
        var health = NewHealth();
        Assert.AreEqual(Max, health.Current);
        Assert.IsFalse(health.IsDepleted);
    }

    [Test]
    public void TakeDamage_ReducesCurrent()
    {
        var health = NewHealth();
        health.TakeDamage(30f);
        Assert.AreEqual(70f, health.Current);
    }

    [Test]
    public void TakeDamage_ClampsAtZero()
    {
        var health = NewHealth();
        health.TakeDamage(1000f);
        Assert.AreEqual(0f, health.Current);
    }

    [Test]
    public void IsDepleted_TrueOnlyAtZero()
    {
        var health = NewHealth();
        health.TakeDamage(Max - 1f);
        Assert.IsFalse(health.IsDepleted);

        health.TakeDamage(1f);
        Assert.IsTrue(health.IsDepleted);
    }

    [Test]
    public void OnDepleted_FiresExactlyOnceOnCrossing()
    {
        var health = NewHealth();
        int depletedCount = 0;
        health.OnDepleted += () => depletedCount++;

        health.TakeDamage(Max); // crosses into depleted
        health.TakeDamage(10f); // already depleted, should not fire again

        Assert.AreEqual(1, depletedCount);
    }

    [Test]
    public void Reset_RestoresFullAndClearsDepleted()
    {
        var health = NewHealth();
        health.TakeDamage(Max);
        Assert.IsTrue(health.IsDepleted);

        health.Reset();

        Assert.AreEqual(Max, health.Current);
        Assert.IsFalse(health.IsDepleted);
    }

    [Test]
    public void OnChanged_FiresWithNewValue()
    {
        var health = NewHealth();
        float? lastValue = null;
        health.OnChanged += v => lastValue = v;

        health.TakeDamage(25f);

        Assert.AreEqual(75f, lastValue);
    }
}
