using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Lightbound
{
    [RequireComponent(typeof(Image), typeof(CanvasGroup))]
    public class ChargeIndicator : MonoBehaviour
    {
        [FormerlySerializedAs("player")]
        [SerializeField] private PlayerRingEnergy playerRingEnergy;
        [SerializeField] private LanternBatteryStation station;

        private Image _fillImage;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _fillImage = GetComponent<Image>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            float progress = station.GetProgress01(playerRingEnergy);
            _fillImage.fillAmount = progress;
            // Fade visibility via CanvasGroup rather than SetActive, since deactivating
            // this GameObject would also stop this script's own Update from running.
            _canvasGroup.alpha = progress > 0f ? 1f : 0f;
        }
    }
}
