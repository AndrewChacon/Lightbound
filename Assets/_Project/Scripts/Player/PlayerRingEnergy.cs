using UnityEngine;

namespace SpaceLanterns
{
    public class PlayerRingEnergy : MonoBehaviour
    {
        [SerializeField] private RingConfig config;

        public RingEnergy Ring { get; private set; }

        private void Awake()
        {
            Ring = new RingEnergy(config.maxEnergy, config.lowPowerEnterPercent, config.lowPowerExitPercent);
        }
    }
}
