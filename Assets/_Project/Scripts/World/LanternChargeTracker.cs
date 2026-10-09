using System.Collections.Generic;

namespace Lightbound
{
    /// <summary>
    /// Tracks time-in-zone for the Lantern Battery station, independently per player.
    /// No UnityEngine dependency so it can run under plain NUnit EditMode tests.
    /// </summary>
    public class LanternChargeTracker
    {
        private readonly float _chargeTime;
        private readonly Dictionary<object, float> _elapsed = new Dictionary<object, float>();

        public LanternChargeTracker(float chargeTime)
        {
            _chargeTime = chargeTime;
        }

        /// <summary>
        /// Advances the given player's charge by deltaTime. Returns true exactly once,
        /// on the tick that completes the charge, then clears that player's progress
        /// so it won't report completion again on subsequent ticks.
        /// </summary>
        public bool Tick(object playerId, float deltaTime)
        {
            float elapsed = _elapsed.TryGetValue(playerId, out var e) ? e : 0f;
            elapsed += deltaTime;

            if (elapsed >= _chargeTime)
            {
                _elapsed.Remove(playerId);
                return true;
            }

            _elapsed[playerId] = elapsed;
            return false;
        }

        public void CancelCharge(object playerId)
        {
            _elapsed.Remove(playerId);
        }

        public float GetProgress01(object playerId)
        {
            if (_chargeTime <= 0f) return 1f;

            float elapsed = _elapsed.TryGetValue(playerId, out var e) ? e : 0f;
            float progress = elapsed / _chargeTime;
            return progress < 0f ? 0f : progress > 1f ? 1f : progress;
        }
    }
}
