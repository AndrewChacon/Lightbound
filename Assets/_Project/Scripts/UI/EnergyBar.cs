using UnityEngine;
using UnityEngine.UI;

namespace Lightbound
{
    [RequireComponent(typeof(Image))]
    public class EnergyBar : MonoBehaviour
    {
        private Image _fillImage;
        private RingEnergy _ring;

        private void Awake()
        {
            _fillImage = GetComponent<Image>();
        }

        private void OnEnable()
        {
            // Player is spawned at runtime by NetworkManager rather than being a
            // fixed scene object, so there's nothing to wire up via the Inspector
            // anymore — wait for NetworkPlayerSetup to announce the local player.
            NetworkPlayerSetup.LocalPlayerSpawned += HandleLocalPlayerSpawned;
        }

        private void OnDisable()
        {
            NetworkPlayerSetup.LocalPlayerSpawned -= HandleLocalPlayerSpawned;
            if (_ring != null)
            {
                _ring.OnEnergyChanged -= HandleEnergyChanged;
            }
        }

        private void HandleLocalPlayerSpawned(PlayerRingEnergy playerRingEnergy)
        {
            _ring = playerRingEnergy.Ring;
            UpdateFill(_ring.Current, _ring.Max);
            _ring.OnEnergyChanged += HandleEnergyChanged;
        }

        private void HandleEnergyChanged(float value)
        {
            UpdateFill(value, _ring.Max);
        }

        private void UpdateFill(float current, float max)
        {
            _fillImage.fillAmount = max > 0f ? current / max : 0f;
        }
    }
}
