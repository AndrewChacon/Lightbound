using UnityEngine;
using UnityEngine.UI;

namespace SpaceLanterns
{
    [RequireComponent(typeof(Slider))]
    public class EnergyBar : MonoBehaviour
    {
        [SerializeField] private PlayerRingEnergy playerRingEnergy;

        private Slider _slider;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        private void OnEnable()
        {
            var ring = playerRingEnergy.Ring;
            _slider.minValue = 0f;
            _slider.maxValue = ring.Max;
            _slider.value = ring.Current;
            ring.OnEnergyChanged += HandleEnergyChanged;
        }

        private void OnDisable()
        {
            playerRingEnergy.Ring.OnEnergyChanged -= HandleEnergyChanged;
        }

        private void HandleEnergyChanged(float value)
        {
            _slider.value = value;
        }
    }
}
