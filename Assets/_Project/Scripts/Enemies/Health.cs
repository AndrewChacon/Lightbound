using System;

namespace Lightbound
{
    /// <summary>
    /// Generic depletable health pool (enemies, breakables). No UnityEngine
    /// dependency so it can run under plain NUnit EditMode tests.
    /// </summary>
    public class Health
    {
        public float Max { get; }
        public float Current { get; private set; }
        public bool IsDepleted => Current <= 0f;

        public event Action<float> OnChanged;
        public event Action OnDepleted;

        public Health(float max)
        {
            Max = max;
            Current = max;
        }

        public void TakeDamage(float amount)
        {
            if (amount <= 0f) return;

            bool wasDepleted = IsDepleted;
            Current = Current - amount < 0f ? 0f : Current - amount;
            OnChanged?.Invoke(Current);

            if (!wasDepleted && IsDepleted)
            {
                OnDepleted?.Invoke();
            }
        }

        public void Reset()
        {
            Current = Max;
            OnChanged?.Invoke(Current);
        }
    }
}
