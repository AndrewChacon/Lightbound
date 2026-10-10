using UnityEngine;
using UnityEngine.UI;

namespace Lightbound
{
    [RequireComponent(typeof(Image))]
    public class EnergyBar : MonoBehaviour
    {
        [SerializeField] private PlayerRingEnergy playerRingEnergy;

        private Image _fillImage;

        private void Awake()
        {
            _fillImage = GetComponent<Image>();
        }

        private void OnEnable()
        {
            var ring = playerRingEnergy.Ring;
            UpdateFill(ring.Current, ring.Max);
            ring.OnEnergyChanged += HandleEnergyChanged;
        }

        private void OnDisable()
        {
            playerRingEnergy.Ring.OnEnergyChanged -= HandleEnergyChanged;
        }

        private void HandleEnergyChanged(float value)
        {
            UpdateFill(value, playerRingEnergy.Ring.Max);
        }

        private void UpdateFill(float current, float max)
        {
            _fillImage.fillAmount = max > 0f ? current / max : 0f;
        }
    }
}
