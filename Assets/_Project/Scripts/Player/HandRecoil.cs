using System.Collections;
using UnityEngine;

namespace Lightbound
{
    /// <summary>
    /// Quick forward-overshoot / backward-overshoot / settle kick on the hand
    /// placeholder's local position, played once per shot by PeaShooter.
    /// </summary>
    public class HandRecoil : MonoBehaviour
    {
        [SerializeField] private float forwardOvershoot = 0.05f;
        [SerializeField] private float backwardOvershoot = 0.08f;
        [SerializeField] private float forwardTime = 0.04f;
        [SerializeField] private float backwardTime = 0.08f;
        [SerializeField] private float settleTime = 0.1f;

        private Vector3 _restLocalPosition;
        private Coroutine _recoilRoutine;

        private void Awake()
        {
            _restLocalPosition = transform.localPosition;
        }

        public void PlayRecoil()
        {
            if (_recoilRoutine != null)
            {
                StopCoroutine(_recoilRoutine);
            }
            _recoilRoutine = StartCoroutine(RecoilSequence());
        }

        private IEnumerator RecoilSequence()
        {
            Vector3 forwardPoint = _restLocalPosition + Vector3.forward * forwardOvershoot;
            Vector3 backwardPoint = _restLocalPosition - Vector3.forward * backwardOvershoot;

            yield return LerpLocalPosition(_restLocalPosition, forwardPoint, forwardTime);
            yield return LerpLocalPosition(forwardPoint, backwardPoint, backwardTime);
            yield return LerpLocalPosition(backwardPoint, _restLocalPosition, settleTime);

            transform.localPosition = _restLocalPosition;
            _recoilRoutine = null;
        }

        private IEnumerator LerpLocalPosition(Vector3 from, Vector3 to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }
            transform.localPosition = to;
        }
    }
}
