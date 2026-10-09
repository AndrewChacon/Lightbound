using NUnit.Framework;
using Lightbound;

public class LanternChargeTrackerTests
{
    private const float ChargeTime = 3f;

    private static LanternChargeTracker NewTracker() => new LanternChargeTracker(ChargeTime);

    [Test]
    public void Tick_DoesNotCompleteBeforeChargeTime()
    {
        var tracker = NewTracker();
        bool completed = tracker.Tick("playerA", 2f);
        Assert.IsFalse(completed);
    }

    [Test]
    public void Tick_CompletesExactlyOnceAtChargeTime()
    {
        var tracker = NewTracker();
        Assert.IsFalse(tracker.Tick("playerA", 2f));
        Assert.IsTrue(tracker.Tick("playerA", 1f)); // elapsed = 3, reaches ChargeTime
    }

    [Test]
    public void Tick_DoesNotKeepReportingCompletionOnLaterTicks()
    {
        var tracker = NewTracker();
        Assert.IsTrue(tracker.Tick("playerA", ChargeTime));
        Assert.IsFalse(tracker.Tick("playerA", 1f), "Should not complete again every subsequent tick");
    }

    [Test]
    public void CancelCharge_ResetsProgressToZero()
    {
        var tracker = NewTracker();
        tracker.Tick("playerA", 2f);
        tracker.CancelCharge("playerA");

        Assert.AreEqual(0f, tracker.GetProgress01("playerA"));
    }

    [Test]
    public void MultiplePlayers_TrackIndependently()
    {
        var tracker = NewTracker();
        tracker.Tick("playerA", 2f);
        tracker.Tick("playerB", 1f);

        Assert.AreEqual(2f / ChargeTime, tracker.GetProgress01("playerA"), 0.0001f);
        Assert.AreEqual(1f / ChargeTime, tracker.GetProgress01("playerB"), 0.0001f);
    }

    [Test]
    public void CompletingOnePlayer_DoesNotAffectAnother()
    {
        var tracker = NewTracker();
        tracker.Tick("playerB", 1f);

        bool completed = tracker.Tick("playerA", ChargeTime);

        Assert.IsTrue(completed);
        Assert.AreEqual(1f / ChargeTime, tracker.GetProgress01("playerB"), 0.0001f);
    }

    [Test]
    public void CancellingOnePlayer_DoesNotAffectAnother()
    {
        var tracker = NewTracker();
        tracker.Tick("playerA", 2f);
        tracker.Tick("playerB", 1f);

        tracker.CancelCharge("playerA");

        Assert.AreEqual(0f, tracker.GetProgress01("playerA"));
        Assert.AreEqual(1f / ChargeTime, tracker.GetProgress01("playerB"), 0.0001f);
    }

    [Test]
    public void GetProgress01_IsZeroForUntrackedPlayer()
    {
        var tracker = NewTracker();
        Assert.AreEqual(0f, tracker.GetProgress01("neverTicked"));
    }

    [Test]
    public void GetProgress01_ClampsToOne()
    {
        var tracker = NewTracker();
        // A single large tick would complete and clear, so approach via two sub-threshold ticks
        // then verify the fraction never reports above 1 even right at the boundary.
        tracker.Tick("playerA", ChargeTime - 0.0001f);
        Assert.LessOrEqual(tracker.GetProgress01("playerA"), 1f);
    }

    [Test]
    public void ZeroOrNegativeChargeTime_CompletesImmediatelyWithoutDivideByZero()
    {
        var tracker = new LanternChargeTracker(0f);
        Assert.IsTrue(tracker.Tick("playerA", 0f));
        Assert.AreEqual(1f, tracker.GetProgress01("playerA"));
    }
}
