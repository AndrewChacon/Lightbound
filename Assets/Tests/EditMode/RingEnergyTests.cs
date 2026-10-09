using NUnit.Framework;
using SpaceLanterns;

public class RingEnergyTests
{
    private const float Max = 100f;
    private const float EnterPercent = 0.01f;
    private const float ExitPercent = 0.5f;

    private static RingEnergy NewRing() => new RingEnergy(Max, EnterPercent, ExitPercent);

    [Test]
    public void StartsFull()
    {
        var ring = NewRing();
        Assert.AreEqual(Max, ring.Current);
        Assert.IsFalse(ring.IsLowPower);
    }

    [Test]
    public void Drain_ReducesCurrent()
    {
        var ring = NewRing();
        ring.Drain(30f);
        Assert.AreEqual(70f, ring.Current);
    }

    [Test]
    public void Drain_ClampsAtZero()
    {
        var ring = NewRing();
        ring.Drain(1000f);
        Assert.AreEqual(0f, ring.Current);
    }

    [Test]
    public void Recharge_ClampsAtMax()
    {
        var ring = NewRing();
        ring.Drain(10f);
        ring.Recharge(1000f);
        Assert.AreEqual(Max, ring.Current);
    }

    [Test]
    public void EntersLowPower_AtOrBelowOnePercent()
    {
        var ring = NewRing();
        ring.Drain(99f); // Current = 1, which is exactly 1% of Max
        Assert.IsTrue(ring.IsLowPower);
    }

    [Test]
    public void StaysNormal_AboveOnePercent()
    {
        var ring = NewRing();
        ring.Drain(98f); // Current = 2, above the 1% threshold
        Assert.IsFalse(ring.IsLowPower);
    }

    [Test]
    public void Hysteresis_DoesNotExitBetweenOneAndFiftyPercent()
    {
        var ring = NewRing();
        ring.Drain(100f); // Current = 0, enters low power
        Assert.IsTrue(ring.IsLowPower);

        ring.Recharge(40f); // Current = 40, still below the 50% exit threshold
        Assert.IsTrue(ring.IsLowPower, "Should remain in low power below the 50% exit threshold");
    }

    [Test]
    public void ExitsLowPower_AtOrAboveFiftyPercent()
    {
        var ring = NewRing();
        ring.Drain(100f);
        ring.Recharge(50f); // Current = 50, exactly the exit threshold
        Assert.IsFalse(ring.IsLowPower);
    }

    [Test]
    public void SetFull_AlwaysExitsLowPowerAndRestoresMax()
    {
        var ring = NewRing();
        ring.Drain(100f);
        Assert.IsTrue(ring.IsLowPower);

        ring.SetFull();

        Assert.AreEqual(Max, ring.Current);
        Assert.IsFalse(ring.IsLowPower);
    }

    [Test]
    public void OnEnergyChanged_FiresWithNewValue()
    {
        var ring = NewRing();
        float? lastValue = null;
        ring.OnEnergyChanged += v => lastValue = v;

        ring.Drain(25f);

        Assert.AreEqual(75f, lastValue);
    }

    [Test]
    public void OnLowPowerChanged_FiresOnlyOnTransition()
    {
        var ring = NewRing();
        int changeCount = 0;
        ring.OnLowPowerChanged += _ => changeCount++;

        ring.Drain(50f); // Current = 50, still normal
        ring.Drain(49f); // Current = 1, enters low power -> fires once
        ring.Drain(1f);  // Current = 0, still low power -> should not fire again

        Assert.AreEqual(1, changeCount);
    }
}
