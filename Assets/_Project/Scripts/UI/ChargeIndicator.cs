using UnityEngine;
using UnityEngine.UI;

namespace Lightbound
{
    [RequireComponent(typeof(Image), typeof(CanvasGroup))]
    public class ChargeIndicator : MonoBehaviour
    {
        [SerializeField] private LanternBatteryStation station;

        private Image _fillImage;
        private CanvasGroup _canvasGroup;
        private PlayerRingEnergy _playerRingEnergy;

        private void Awake()
        {
            _fillImage = GetComponent<Image>();
            _canvasGroup = GetComponent<CanvasGroup>();
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
        }

        private void HandleLocalPlayerSpawned(PlayerRingEnergy playerRingEnergy)
        {
            _playerRingEnergy = playerRingEnergy;
        }

        private void Update()
        {
            if (_playerRingEnergy == null) return;

            float progress = station.GetProgress01(_playerRingEnergy);
            _fillImage.fillAmount = progress;
            // Fade visibility via CanvasGroup rather than SetActive, since deactivating
            // this GameObject would also stop this script's own Update from running.
            _canvasGroup.alpha = progress > 0f ? 1f : 0f;
        }
    }
}
