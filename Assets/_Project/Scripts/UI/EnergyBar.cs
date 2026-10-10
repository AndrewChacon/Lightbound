using UnityEngine;
using UnityEngine.UI;

namespace Lightbound
{
    [RequireComponent(typeof(Image))]
    public class EnergyBar : MonoBehaviour
    {
        [SerializeField] private PlayerRingEnergy playerRingEnergy;

        private Image _fillImage;
        private RingEnergy _ring;

        private void Awake()
        {
            _fillImage = GetComponent<Image>();
        }

        private void Start()
        {
            // Not OnEnable: Unity only guarantees every object's Awake/OnEnable has
            // run before anyone's Start, not before another object's OnEnable —
            // PlayerRingEnergy.Ring is set in its own Awake, so reading it here
            // instead of OnEnable avoids a startup-order-dependent null reference
            // (this bit the energy bar in a standalone build, though not in the
            // Editor, where object init order happened to work out differently).
            _ring = playerRingEnergy.Ring;
            UpdateFill(_ring.Current, _ring.Max);
            _ring.OnEnergyChanged += HandleEnergyChanged;
        }

        private void OnDisable()
        {
            if (_ring != null)
            {
                _ring.OnEnergyChanged -= HandleEnergyChanged;
            }
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
