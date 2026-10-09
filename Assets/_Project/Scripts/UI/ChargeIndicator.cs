using UnityEngine;
using UnityEngine.UI;

namespace Lightbound
{
    [RequireComponent(typeof(Slider), typeof(CanvasGroup))]
    public class ChargeIndicator : MonoBehaviour
    {
        [SerializeField] private PlayerRingEnergy player;
        [SerializeField] private LanternBatteryStation station;

        private Slider _slider;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _slider.minValue = 0f;
            _slider.maxValue = 1f;
        }

        private void Update()
        {
            float progress = station.GetProgress01(player);
            _slider.value = progress;
            // Fade visibility via CanvasGroup rather than SetActive, since deactivating
            // this GameObject would also stop this script's own Update from running.
            _canvasGroup.alpha = progress > 0f ? 1f : 0f;
        }
    }
}
