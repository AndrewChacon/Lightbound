using System;

namespace SpaceLanterns
{
    /// <summary>
    /// Ring energy state machine: 0-100% with Low Power Mode hysteresis.
    /// No UnityEngine dependency so it can run under plain NUnit EditMode tests.
    /// </summary>
    public class RingEnergy
    {
        public float Max { get; }
        public float Current { get; private set; }
        public bool IsLowPower { get; private set; }

        public event Action<float> OnEnergyChanged;
        public event Action<bool> OnLowPowerChanged;

        private readonly float _lowPowerEnterThreshold;
        private readonly float _lowPowerExitThreshold;

        public RingEnergy(float max, float lowPowerEnterPercent, float lowPowerExitPercent)
        {
            Max = max;
            _lowPowerEnterThreshold = max * lowPowerEnterPercent;
            _lowPowerExitThreshold = max * lowPowerExitPercent;
            Current = max;
        }

        public void Drain(float amount)
        {
            if (amount <= 0f) return;
            SetCurrent(Current - amount);
        }

        public void Recharge(float amount)
        {
            if (amount <= 0f) return;
            SetCurrent(Current + amount);
        }

        public void SetFull()
        {
            Current = Max;
            SetLowPower(false);
            OnEnergyChanged?.Invoke(Current);
        }

        private void SetCurrent(float value)
        {
            Current = value < 0f ? 0f : value > Max ? Max : value;

            if (!IsLowPower && Current <= _lowPowerEnterThreshold)
            {
                SetLowPower(true);
            }
            else if (IsLowPower && Current >= _lowPowerExitThreshold)
            {
                SetLowPower(false);
            }

            OnEnergyChanged?.Invoke(Current);
        }

        private void SetLowPower(bool value)
        {
            if (IsLowPower == value) return;
            IsLowPower = value;
            OnLowPowerChanged?.Invoke(IsLowPower);
        }
    }
}
