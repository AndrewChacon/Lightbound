using UnityEngine;

namespace Lightbound
{
    [CreateAssetMenu(fileName = "RingConfig", menuName = "Lightbound/Ring Config")]
    public class RingConfig : ScriptableObject
    {
        [Header("Core")]
        public float maxEnergy = 100f;

        [Header("Low Power Mode hysteresis")]
        [Range(0f, 1f)] public float lowPowerEnterPercent = 0.01f;
        [Range(0f, 1f)] public float lowPowerExitPercent = 0.5f;

        [Header("Pea shot (wired up in a later build-order step)")]
        public float peaShotEnergyCost = 5f;
        public float lowPowerChargePerHit = 2f;
    }
}
